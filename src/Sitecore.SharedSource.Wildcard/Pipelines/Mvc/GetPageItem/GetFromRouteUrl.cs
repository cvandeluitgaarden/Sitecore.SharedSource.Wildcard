namespace Sitecore.SharedSource.Wildcard.Pipelines.Mvc.GetPageItem
{
    using Sitecore.Mvc.Configuration;
    using Sitecore.Mvc.Pipelines.Response.GetPageItem;
    using System;
    using System.Web.Routing;
    using System.Web;
    using Sitecore.Data.Items;
    using Sitecore.Data.Fields;
    using Sitecore;
    using Sitecore.Diagnostics;
    using System.Linq;
    using Sitecore.SharedSource.Wildcard;
    using Sitecore.SharedSource.Wildcard.Extensions;

    public class GetFromRouteUrl : Sitecore.Mvc.Pipelines.Response.GetPageItem.GetFromRouteUrl
    {
        public override void Process(GetPageItemArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.Result != null)
            {
                return;
            }

            args.Result = this.ResolveItem(args);
        }

        protected virtual string GetPath(RouteData routeData)
        {
            Route route = routeData.Route as Route;
            if (route == null)
            {
                return null;
            }

            string url = route.Url;
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            char[] chrArray = new char[] { '/' };
            string[] strArrays = url.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
            url = this.GetPathFromParts(strArrays, routeData);
            url = url.Remove(0, Sitecore.Context.Site.SiteInfo.VirtualFolder.Length - 1);
            return url;
        }

        protected virtual string GetPathFromParts(string[] parts, RouteData routeData)
        {
            string ignoreKeyPrefix = MvcSettings.IgnoreKeyPrefix;
            string empty = string.Empty;
            string[] strArrays = parts;
            for (int i = 0; i < (int)strArrays.Length; i++)
            {
                string str = strArrays[i];
                GetFromRouteUrl getFromRouteUrl = this;
                string str1 = str;
                RouteData routeDt = routeData;
                string str2 = getFromRouteUrl.ReplaceToken(str1, routeDt, (string key) => !routeData.Values.ContainsKey(string.Concat(ignoreKeyPrefix, key)));
                if (!string.IsNullOrEmpty(str2))
                {
                    empty = string.Concat(empty, "/", str2);
                }
            }

            return empty;
        }

        protected new virtual Item ResolveItem(GetPageItemArgs args)
        {
            string path = this.GetPath(args.RouteData);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            Item wildcardItem = this.GetItem(path, args);
            if (wildcardItem == null)
            {
                return null;
            }

            if (!wildcardItem.IsWildcardItem())
            {
                return wildcardItem;
            }

            ReferenceField datasourceReference = wildcardItem.Fields[AppConstants.WildcardDatasourceField];
            if (datasourceReference == null || datasourceReference.TargetItem == null)
            {
                return wildcardItem;
            }

            if (HttpContext.Current.Items.Contains(AppConstants.ContextItemKey))
            {
                HttpContext.Current.Items[AppConstants.ContextItemKey] = wildcardItem;
            }
            else
            {
                HttpContext.Current.Items.Add(AppConstants.ContextItemKey, wildcardItem);
            }

            string itemRelativePath = StringUtil.EnsurePrefix('/', WildcardProvider.GetWildCardItemRelativeSitecorePathFromUrl(path, wildcardItem));
            string itemPath = string.Concat(datasourceReference.TargetItem.Paths.FullPath, itemRelativePath);

            string[] pathSegments = itemPath.Split('/');
            return WildcardProvider.GetDatasourceItem(string.Join("/", pathSegments.Take(pathSegments.Length - 1)), pathSegments.LastOrDefault());
        }
    }
}
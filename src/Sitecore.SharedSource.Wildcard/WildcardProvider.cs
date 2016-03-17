namespace Sitecore.SharedSource.Wildcard
{
    using Sitecore.Configuration;
    using Data.Items;
    using Web;
    using Xml;
    using Data;
    using System.Linq;
    using ContentSearch;
    using ContentSearch.SearchTypes;
    using Data.Fields;
    using System.Web;
    using System.Collections.Generic;
    using System;
    using Extensions;
    using Sitecore.Links;

    public class WildcardProvider
    {
        public static Item GetWildcardSettingsFolder(SiteInfo siteInfo)
        {
            if(Sitecore.Context.Database == null)
            {
                return null;
            }

            string xpath = $"/sitecore/wildcard/settings/*[@name = '{siteInfo.Name}']";
            var node = Factory.GetConfigNode(xpath);
            if (node == null)
            {
                node = Factory.GetConfigNode("/sitecore/wildcard/settings/*[@name = 'Default']");
            }

            var path = XmlUtil.GetAttribute("path", node, true);
            if(string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if(!path.StartsWith("/sitecore/", System.StringComparison.OrdinalIgnoreCase))
            {
                path = string.Concat(siteInfo.RootPath, StringUtil.EnsurePrefix('/', path));
            }

            return Sitecore.Context.Database.GetItem(path);
        }

        public static WildcardSetting GetSetting(ID templateId)
        {
            var folder = GetWildcardSettingsFolder(Sitecore.Context.Site.SiteInfo);
            if(folder == null)
            {
                return null;
            }

            var settings = folder.GetChildren(Collections.ChildListOptions.SkipSorting);
            return settings.Where(setting => setting[AppConstants.WildcardSettingDatasourceTemplateField] == templateId.ToString())
                .Select(setting => new WildcardSetting(setting))
                .FirstOrDefault();
        }

        public static bool IsWildcardItem(Item item)
        {
            return item?.Name == "*" && item?.TemplateID == AppConstants.WildcardItemTemplate;
        }

        public static Item GetDatasourceItem(Item wildcardItem, string name)
        {
            ReferenceField datasourceReference = wildcardItem.Fields[AppConstants.WildcardDatasourceField];
            return GetDatasourceItem(datasourceReference?.TargetItem?.Paths.FullPath, name);
        }

        public static Item GetDatasourceItem(string path, string name)
        {
            var searchContext = Sitecore.ContentSearch.ContentSearchManager.GetIndex(GetIndexName(Sitecore.Context.Item)).CreateSearchContext(Sitecore.ContentSearch.Security.SearchSecurityOptions.EnableSecurityCheck);
            var result = searchContext.GetQueryable<SearchResultItem>().Where(x =>
                x.Path.StartsWith(Sitecore.StringUtil.EnsurePostfix('/', path)) &&
                x.Path.EndsWith(name, StringComparison.OrdinalIgnoreCase) &&
                x.Language == Sitecore.Context.Item.Language.Name)
                .FirstOrDefault();

            return result?.GetItem();
        }

        private static string GetIndexName(Item item)
        {
            return Sitecore.ContentSearch.ContentSearchManager.GetContextIndexName(new SitecoreIndexableItem(item));
        }

        public static Item GetContextWildcardItem()
        {
            return HttpContext.Current.Items[AppConstants.ContextItemKey] as Item; 
        }

        public static string GetWildCardItemRelativeSitecorePathFromUrl(string path, Item wildcardItem)
        {
            List<string> itemPathParts = new List<string>();
            List<string> urlParts = path.Replace(".aspx", string.Empty)
                                        .Split(new char[] { '/' }, 100, StringSplitOptions.RemoveEmptyEntries)
                                        .Reverse()
                                        .ToList();

            Item wildcardAncestor = wildcardItem;
            if (wildcardAncestor.IsWildcardItem())
            {
                while (!string.IsNullOrEmpty(urlParts.LastOrDefault()))
                {
                    itemPathParts.Insert(0, urlParts.FirstOrDefault());
                    urlParts.RemoveAt(0);
                    wildcardAncestor = wildcardAncestor.Parent;
                }
            }

            string itemRelativePath = string.Join("/", itemPathParts);
            return itemRelativePath;
        }

        public static string GetWildcardItemUrl(Sitecore.Data.Items.Item wildcardItem, Item realItem, bool useDisplayName, UrlOptions urlOptions = null)
        {
            if (urlOptions == null)
            {
                urlOptions = UrlOptions.DefaultOptions;
            }

            urlOptions.AlwaysIncludeServerUrl = true;
            if (wildcardItem == null || realItem == null)
            {
                return string.Empty;
            }

            var scLinkProvider = new LinkProvider();
            string wildcardUrl = scLinkProvider.GetItemUrl(wildcardItem, urlOptions);
            int wildcardCount = wildcardUrl.Split('/').Where(x => x == ",-w-,").Count();
            wildcardUrl = wildcardUrl.Replace(",-w-,", string.Empty);

            Uri uri = new Uri(wildcardUrl);
            List<string> wildcardItemPathParts = uri.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ReferenceField wildcardDatasource = wildcardItem.Fields[AppConstants.WildcardDatasourceField];
            if (wildcardDatasource != null)
            {
                List<string> realItemPathParts = new List<string>();
                if (realItem.Axes.GetAncestors().Any(a => a.ID == wildcardDatasource.TargetID))
                {
                    Item ancestor = realItem.Parent;
                    int idx = 1;
                    while (ancestor.ID != wildcardDatasource.TargetID && idx < wildcardCount)
                    {
                        idx++;
                        realItemPathParts.Insert(0, useDisplayName ? ancestor.DisplayName : ancestor.Name);
                        ancestor = ancestor.Parent;
                    }
                }

                string itemUrlName = useDisplayName ? realItem.DisplayName : realItem.Name;
                realItemPathParts.Add(itemUrlName);
                wildcardItemPathParts.AddRange(realItemPathParts);
            }

            UriBuilder uriBuilder = new UriBuilder { Scheme = uri.Scheme, Host = uri.Host, Port = uri.Port, Path = string.Join("/", wildcardItemPathParts) };
            return uriBuilder.Uri.ToString();
        }

        public static string GetWildcardItemUrl(Sitecore.Data.Items.Item wildcardItem, Item realItem, bool useDisplayName)
        {
            return GetWildcardItemUrl(wildcardItem, realItem, useDisplayName, UrlOptions.DefaultOptions);
        }
    }
}
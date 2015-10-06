namespace Sitecore.SharedSource.Wildcard.Pipelines.HttpRequest
{
    using Sitecore.Pipelines.HttpRequest;
    using System;
    using System.Web;
    using System.Linq;

    public class WildcardItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if(!WildcardProvider.IsWildcardItem(Context.Item))
            {
                return;
            }

            string itemName = ResolveItemNameFromUrl(args.Context);
            var datasourceItem = WildcardProvider.GetDatasourceItem(Context.Item, itemName);
            if(datasourceItem == null)
            {
                return;
            }

            args.Context.Items[AppConstants.ContextItemKey] = Context.Item;
            Context.Item = datasourceItem; 
        }

        private string ResolveItemNameFromUrl(HttpContext context)
        {
            return context.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
        }
    }
}
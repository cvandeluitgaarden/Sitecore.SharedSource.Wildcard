namespace Sitecore.SharedSource.Wildcard.Pipelines.HttpRequest
{
    using Sitecore.Pipelines.HttpRequest;
    using System;
    using System.Web;

    public class WildcardItemResolver : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            if(!WildcardProvider.IsWildcardItem(Sitecore.Context.Item))
            {
                return;
            }

            string itemName = ResolveItemNameFromUrl(args.Context);
            var wildcardItem = WildcardProvider.GetDatasourceItem(Sitecore.Context.Item, itemName);
            if(wildcardItem == null)
            {
                return;
            }

            args.Context.Items[AppConstants.ContextItemKey] = Sitecore.Context.Item;
            Sitecore.Context.Item = wildcardItem; 
        }

        private string ResolveItemNameFromUrl(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
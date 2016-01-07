namespace Sitecore.SharedSource.Wildcard.Links
{
    using Diagnostics;
    using Data.Items;
    using Extensions;
    using Data;

    public class LinkProvider : Sitecore.Links.LinkProvider
    {
        public override string GetItemUrl(Sitecore.Data.Items.Item item, Sitecore.Links.UrlOptions options)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(options, "options");

            // Store real item for later use
            Item realItem = item;

            // Check if item is an wildcard item
            bool isWildcardItem = item.IsDatasourceItemForWildcard();
            if (isWildcardItem)
            {
                item = Context.Database.GetItem(WildcardProvider.GetSetting(item.TemplateID).ItemID);
            }

            if (item == null)
            {
                item = realItem;
            }

            string text = base.GetItemUrl(item, options);
            if (isWildcardItem)
            {
                text = WildcardProvider.GetWildcardItemUrl(item, realItem, UseDisplayName);
            }

            return text.ToLower();
        }
    }
}
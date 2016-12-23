namespace Sitecore.SharedSource.Wildcard.Links
{
    using Diagnostics;
    using Data.Items;
    using Extensions;
    using Data;
    using Sites;
    using System.Linq;
    using System;
    using Sitecore.Links;
    using Web;
    public class LinkProvider : Sitecore.Links.LinkProvider
    {
        public override string GetItemUrl(Sitecore.Data.Items.Item item, Sitecore.Links.UrlOptions options)
        {
            Assert.ArgumentNotNull(item, "item");
            Assert.ArgumentNotNull(options, "options");

            SiteInfo siteInfo = null;

            // Store real item for later use
            Item realItem = item;

            string text = string.Empty;

            // Check if item is an wildcard item
            bool isWildcardItem = item.IsDatasourceItemForWildcard();

            if (isWildcardItem)
            {
                Item homeItem = null;
                bool siteContextChanged = false;

                var currentItem = Sitecore.Context.Item;
                if (currentItem != null)
                {
                    if (currentItem.TemplateName == "Homepage" || currentItem.TemplateName == "Home")
                    {
                        homeItem = currentItem;
                    }
                    else
                    {
                        homeItem = currentItem.Axes.GetAncestors().Where(x => x.TemplateName == "Homepage" || x.TemplateName == "Home").FirstOrDefault();
                    }

                    // first make sure that we actually have the correct current context
                    if (homeItem != null)
                    {
                        siteInfo = Sitecore.Configuration.Factory.GetSiteInfoList().FirstOrDefault(x => string.Format("{0}{1}", x.RootPath, x.StartItem).Equals(homeItem.Paths.FullPath, StringComparison.OrdinalIgnoreCase));
                        using (new SiteContextSwitcher(new SiteContext(siteInfo)))
                        {
                            item = Context.Database.GetItem(WildcardProvider.GetSetting(item.TemplateID).ItemID);
                            siteContextChanged = true;
                        }
                    }
                }

                if(!siteContextChanged)
                {
                    item = Context.Database.GetItem(WildcardProvider.GetSetting(item.TemplateID).ItemID);
                }

                var wildcardHomeItem = realItem.RealItem().Axes.GetAncestors().Where(x => x.TemplateName == "Homepage" || x.TemplateName == "Home").FirstOrDefault();

                // if in case that the wildcard home item is different than the current context
                if (wildcardHomeItem != null && !wildcardHomeItem.Axes.IsAncestorOf(item))
                {
                    siteInfo = Sitecore.Configuration.Factory.GetSiteInfoList().FirstOrDefault(x => string.Format("{0}{1}", x.RootPath, x.StartItem).Equals(wildcardHomeItem.Paths.FullPath, StringComparison.OrdinalIgnoreCase));
                }
            }

            if (item == null)
            {
                item = realItem;
            }


            SiteContextSwitcher contextSwitcher = null;
            if (siteInfo != null)
            {
                contextSwitcher = new SiteContextSwitcher(new SiteContext(siteInfo));
            }


            text = base.GetItemUrl(item, options);
            if (isWildcardItem)
            {
                text = WildcardProvider.GetWildcardItemUrl(item, realItem, UseDisplayName);
            }

            if (siteInfo != null)
            {
                contextSwitcher.Dispose();
            }


            return text.ToLower();
        }
    }
}
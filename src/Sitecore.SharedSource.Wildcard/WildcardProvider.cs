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

    public class WildcardProvider
    {
        public static Item GetWildcardSettingsFolder(SiteInfo siteInfo)
        {
            string xpath = string.Concat("/sitecore/wildcard/settings/*[@name = '", siteInfo.Name, "']");
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
            var searchContext = Sitecore.ContentSearch.ContentSearchManager.GetIndex(GetIndexName(Sitecore.Context.Item)).CreateSearchContext(ContentSearch.Security.SearchSecurityOptions.EnableSecurityCheck);
            var result = searchContext.GetQueryable<SearchResultItem>()
                .Where(x => x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
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
    }
}
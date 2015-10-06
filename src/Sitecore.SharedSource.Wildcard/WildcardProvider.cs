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

    public class WildcardProvider
    {
        public static Item GetWildcardSettingsFolder(SiteInfo siteInfo)
        {
            var node = Factory.GetConfigNode(string.Concat("/wildcard/settings/setting/*[@name = '",siteInfo.Name, ",]"));
            if (node == null)
            {
                node = Factory.GetConfigNode("/wildcard/settings/setting/*[@name = '']");
            }

            var path = XmlUtil.GetAttribute("path", node, true);
            if(string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            if(!path.StartsWith("/sitecore/", System.StringComparison.OrdinalIgnoreCase))
            {
                path = string.Concat(siteInfo.ContentStartItem, StringUtil.EnsurePrefix('/', path));
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

            return folder.GetChildren(Collections.ChildListOptions.SkipSorting)
                .Where(setting => setting[AppConstants.WildcardSettingTemplateDatasourceTemplateField] == templateId.ToString())
                .Select(setting => new WildcardSetting(setting))
                .FirstOrDefault();
        }

        public static bool IsWildcardItem(Item item)
        {
            return item?.Name == "*" && GetSetting(item?.TemplateID) != null;
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
                .Where(x => x.Path.StartsWith(path, System.StringComparison.OrdinalIgnoreCase) && x.Name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            return result.GetItem();
        }

        private static string GetIndexName(Item item)
        {
            return Sitecore.ContentSearch.ContentSearchManager.GetContextIndexName(new SitecoreIndexableItem(item));
        }
    }
}
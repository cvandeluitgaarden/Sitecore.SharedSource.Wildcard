namespace Sitecore.SharedSource.Wildcard.Extensions
{
    using Data.Items;

    public static class ItemExtensions
    {
        public static Item RealItem(this Item item)
        {
            var wildcardItem =  WildcardProvider.GetContextWildcardItem();
            if(wildcardItem == null)
            {
                wildcardItem = item;
            }

            return wildcardItem;
        }

        public static bool IsWildcardItem(this Item item)
        {
            return WildcardProvider.IsWildcardItem(item);
        }

        public static bool IsDatasourceItemForWildcard(this Item item)
        {
            return WildcardProvider.GetSetting(item.TemplateID) != null;
        }
    }
}
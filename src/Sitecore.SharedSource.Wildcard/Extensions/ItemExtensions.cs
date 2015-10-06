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
    }
}
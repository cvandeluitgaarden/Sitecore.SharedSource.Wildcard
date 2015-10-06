namespace Sitecore.SharedSource.Wildcard
{
    using Sitecore.Data;
    using Data.Fields;
    using Sitecore.Data.Items;

    public class WildcardSetting
    {
        public WildcardSetting(Item setting)
        {
            ReferenceField itemReference = setting.Fields[AppConstants.WildcardSettingItemField];
            ItemID = itemReference?.TargetItem?.ID;
            ReferenceField datasourceReference = setting.Fields[AppConstants.WildcardSettingItemField];
            DatasourceTemplateID = datasourceReference?.TargetItem?.ID;
        }

        public ID ItemID { get; private set; }

        public ID DatasourceTemplateID { get; private set; }
    }
}
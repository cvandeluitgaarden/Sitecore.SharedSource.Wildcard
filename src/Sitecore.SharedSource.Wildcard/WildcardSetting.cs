namespace Sitecore.SharedSource.Wildcard
{
    using Sitecore.Data;
    using Data.Fields;
    using Sitecore.Data.Items;

    public class WildcardSetting
    {
        public WildcardSetting(Item setting)
        {
            ReferenceField itemReference = setting.Fields[AppConstants.WildcardSettingTemplateItemField];
            ItemID = itemReference?.TargetItem?.ID;
            ReferenceField datasourceReference = setting.Fields[AppConstants.WildcardSettingTemplateItemField];
            DatasourceTemplateID = datasourceReference?.TargetItem?.ID;
        }

        public ID ItemID { get; private set; }

        public ID DatasourceTemplateID { get; private set; }
    }
}
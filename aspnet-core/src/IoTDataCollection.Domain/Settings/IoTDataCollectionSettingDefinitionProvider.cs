using Volo.Abp.Settings;

namespace IoTDataCollection.Settings;

public class IoTDataCollectionSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(IoTDataCollectionSettings.MySetting1));
    }
}

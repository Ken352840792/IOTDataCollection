using IoTDataCollection.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IoTDataCollection.Permissions;

public class IoTDataCollectionPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(IoTDataCollectionPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(IoTDataCollectionPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IoTDataCollectionResource>(name);
    }
}

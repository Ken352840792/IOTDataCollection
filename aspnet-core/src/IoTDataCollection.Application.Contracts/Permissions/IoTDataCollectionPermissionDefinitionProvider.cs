using IoTDataCollection.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace IoTDataCollection.Permissions;

public class IoTDataCollectionPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(IoTDataCollectionPermissions.GroupName, L("Permission:IoTDataCollection"));

        // 企业管理权限
        var enterprisesPermission = myGroup.AddPermission(IoTDataCollectionPermissions.Enterprises.Default, L("Permission:Enterprises"));
        enterprisesPermission.AddChild(IoTDataCollectionPermissions.Enterprises.Create, L("Permission:Enterprises.Create"));
        enterprisesPermission.AddChild(IoTDataCollectionPermissions.Enterprises.Edit, L("Permission:Enterprises.Edit"));
        enterprisesPermission.AddChild(IoTDataCollectionPermissions.Enterprises.Delete, L("Permission:Enterprises.Delete"));

        // 站点管理权限
        var sitesPermission = myGroup.AddPermission(IoTDataCollectionPermissions.Sites.Default, L("Permission:Sites"));
        sitesPermission.AddChild(IoTDataCollectionPermissions.Sites.Create, L("Permission:Sites.Create"));
        sitesPermission.AddChild(IoTDataCollectionPermissions.Sites.Edit, L("Permission:Sites.Edit"));
        sitesPermission.AddChild(IoTDataCollectionPermissions.Sites.Delete, L("Permission:Sites.Delete"));

        // 设备管理权限
        var devicesPermission = myGroup.AddPermission(IoTDataCollectionPermissions.Devices.Default, L("Permission:Devices"));
        devicesPermission.AddChild(IoTDataCollectionPermissions.Devices.Create, L("Permission:Devices.Create"));
        devicesPermission.AddChild(IoTDataCollectionPermissions.Devices.Edit, L("Permission:Devices.Edit"));
        devicesPermission.AddChild(IoTDataCollectionPermissions.Devices.Delete, L("Permission:Devices.Delete"));
        devicesPermission.AddChild(IoTDataCollectionPermissions.Devices.Debug, L("Permission:Devices.Debug"));

        // 数据点管理权限
        var dataPointsPermission = myGroup.AddPermission(IoTDataCollectionPermissions.DataPoints.Default, L("Permission:DataPoints"));
        dataPointsPermission.AddChild(IoTDataCollectionPermissions.DataPoints.Create, L("Permission:DataPoints.Create"));
        dataPointsPermission.AddChild(IoTDataCollectionPermissions.DataPoints.Edit, L("Permission:DataPoints.Edit"));
        dataPointsPermission.AddChild(IoTDataCollectionPermissions.DataPoints.Delete, L("Permission:DataPoints.Delete"));

        // 规则管理权限
        var rulesPermission = myGroup.AddPermission(IoTDataCollectionPermissions.Rules.Default, L("Permission:Rules"));
        rulesPermission.AddChild(IoTDataCollectionPermissions.Rules.Create, L("Permission:Rules.Create"));
        rulesPermission.AddChild(IoTDataCollectionPermissions.Rules.Edit, L("Permission:Rules.Edit"));
        rulesPermission.AddChild(IoTDataCollectionPermissions.Rules.Delete, L("Permission:Rules.Delete"));
        rulesPermission.AddChild(IoTDataCollectionPermissions.Rules.Execute, L("Permission:Rules.Execute"));

        // 系统监控权限
        var monitoringPermission = myGroup.AddPermission(IoTDataCollectionPermissions.Monitoring.Default, L("Permission:Monitoring"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.Create, L("Permission:Monitoring.Create"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.Edit, L("Permission:Monitoring.Edit"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.Delete, L("Permission:Monitoring.Delete"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.ViewStats, L("Permission:Monitoring.ViewStats"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.HealthCheck, L("Permission:Monitoring.HealthCheck"));
        monitoringPermission.AddChild(IoTDataCollectionPermissions.Monitoring.NodeControl, L("Permission:Monitoring.NodeControl"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<IoTDataCollectionResource>(name);
    }
} 
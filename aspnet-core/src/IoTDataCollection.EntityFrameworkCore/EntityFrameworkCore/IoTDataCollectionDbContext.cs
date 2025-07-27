using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using IoTDataCollection.Enterprises;
using IoTDataCollection.Sites;
using IoTDataCollection.Devices;
using IoTDataCollection.DataPoints;
using IoTDataCollection.Rules;
using IoTDataCollection.Monitoring;

namespace IoTDataCollection.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class IoTDataCollectionDbContext :
    AbpDbContext<IoTDataCollectionDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region IoT Business Entities

    // 组织管理相关实体
    public DbSet<Enterprises.Enterprise> Enterprises { get; set; }
    public DbSet<Sites.Site> Sites { get; set; }

    // 设备管理相关实体
    public DbSet<Devices.Device> Devices { get; set; }
    public DbSet<DataPoints.DataPoint> DataPoints { get; set; }

    // 规则引擎相关实体
    public DbSet<Rules.JavaScriptRule> JavaScriptRules { get; set; }
    public DbSet<Rules.NodeRedFlow> NodeRedFlows { get; set; }

    // 系统监控相关实体
    public DbSet<Monitoring.CollectorNode> CollectorNodes { get; set; }

    #endregion

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }
    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public IoTDataCollectionDbContext(DbContextOptions<IoTDataCollectionDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        #region IoT Business Entity Configurations

        // 配置企业实体
        builder.Entity<Enterprises.Enterprise>(b =>
        {
            b.ToTable("T_ENTERPRISES", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("企业信息表 - 存储企业基本信息和联系方式"));
            b.ConfigureByConvention(); // 自动配置ABP基类属性
            
            // 创建索引
            b.HasIndex(x => x.F_EnterpriseCode).IsUnique();
            b.HasIndex(x => x.F_EnterpriseName);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_EnterpriseCode).HasComment("企业编码 - 全局唯一标识");
            b.Property(x => x.F_EnterpriseName).HasComment("企业名称 - 企业全称");
            b.Property(x => x.F_ShortName).HasComment("企业简称 - 企业简短名称");
            b.Property(x => x.F_EnterpriseType).HasComment("企业类型 - 制造业、服务业等");
            b.Property(x => x.F_ContactPerson).HasComment("联系人 - 企业主要联系人姓名");
            b.Property(x => x.F_ContactPhone).HasComment("联系电话 - 企业联系电话");
            b.Property(x => x.F_Address).HasComment("企业地址 - 企业详细地址");
            b.Property(x => x.F_Description).HasComment("企业描述 - 企业简介和说明");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
            b.Property(x => x.F_SortOrder).HasComment("排序号 - 用于显示排序");
            b.Property(x => x.F_IsEnabled).HasComment("是否启用 - 业务启用标识");
        });

        // 配置站点实体
        builder.Entity<Sites.Site>(b =>
        {
            b.ToTable("T_SITES", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("站点信息表 - 存储企业下属站点/工厂信息"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_EnterpriseId, x.F_SiteCode }).IsUnique();
            b.HasIndex(x => x.F_SiteName);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_EnterpriseId).HasComment("企业ID - 关联企业表外键");
            b.Property(x => x.F_SiteCode).HasComment("站点编码 - 企业内唯一标识");
            b.Property(x => x.F_SiteName).HasComment("站点名称 - 站点全称");
            b.Property(x => x.F_ShortName).HasComment("站点简称 - 站点简短名称");
            b.Property(x => x.F_SiteType).HasComment("站点类型 - 工厂、车间、仓库等");
            b.Property(x => x.F_Address).HasComment("站点地址 - 站点详细地址");
            b.Property(x => x.F_Manager).HasComment("负责人 - 站点负责人姓名");
            b.Property(x => x.F_ContactPhone).HasComment("联系电话 - 站点联系电话");
            b.Property(x => x.F_Description).HasComment("站点描述 - 站点简介和说明");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
            b.Property(x => x.F_SortOrder).HasComment("排序号 - 用于显示排序");
        });

        // 配置设备实体
        builder.Entity<Devices.Device>(b =>
        {
            b.ToTable("T_DEVICES", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("设备信息表 - 存储工业设备基本信息和连接参数"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_SiteId, x.F_DeviceCode }).IsUnique();
            b.HasIndex(x => x.F_DeviceName);
            b.HasIndex(x => x.F_ConnectionStatus);
            b.HasIndex(x => x.F_IsCollectionEnabled);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_SiteId).HasComment("站点ID - 关联站点表外键");
            b.Property(x => x.F_DeviceCode).HasComment("设备编码 - 站点内唯一标识");
            b.Property(x => x.F_DeviceName).HasComment("设备名称 - 设备显示名称");
            b.Property(x => x.F_DeviceType).HasComment("设备类型 - PLC、Modbus、OPC-UA等");
            b.Property(x => x.F_IpAddress).HasComment("IP地址 - 设备网络地址");
            b.Property(x => x.F_Port).HasComment("端口号 - 设备通信端口");
            b.Property(x => x.F_CommunicationProtocol).HasComment("通信协议 - Modbus、OPC-UA、TCP等");
            b.Property(x => x.F_ConnectionStatus).HasComment("连接状态 - 在线、离线、异常");
            b.Property(x => x.F_IsCollectionEnabled).HasComment("启用采集 - 是否启用数据采集");
            b.Property(x => x.F_CollectionInterval).HasComment("采集间隔 - 数据采集间隔秒数");
            b.Property(x => x.F_Description).HasComment("设备描述 - 设备详细说明");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
            b.Property(x => x.F_SortOrder).HasComment("排序号 - 用于显示排序");
        });

        // 配置数据点实体
        builder.Entity<DataPoints.DataPoint>(b =>
        {
            b.ToTable("T_DATA_POINTS", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("数据点信息表 - 存储设备数据点定义和采集配置"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_DeviceId, x.F_PointCode }).IsUnique();
            b.HasIndex(x => x.F_PointName);
            b.HasIndex(x => x.F_IsCollectionEnabled);
            b.HasIndex(x => x.F_CollectionPriority);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_DeviceId).HasComment("设备ID - 关联设备表外键");
            b.Property(x => x.F_PointCode).HasComment("数据点编码 - 设备内唯一标识");
            b.Property(x => x.F_PointName).HasComment("数据点名称 - 数据点显示名称");
            b.Property(x => x.F_DataType).HasComment("数据类型 - Int16、Int32、Float、Bool、String等");
            b.Property(x => x.F_RegisterAddress).HasComment("寄存器地址 - Modbus寄存器地址");
            b.Property(x => x.F_RegisterType).HasComment("寄存器类型 - 保持寄存器、输入寄存器等");
            b.Property(x => x.F_DataLength).HasComment("数据长度 - 字节数或位数");
            b.Property(x => x.F_ScaleFactor).HasComment("缩放比例 - 数据值缩放因子");
            b.Property(x => x.F_Offset).HasComment("偏移量 - 数据值偏移量");
            b.Property(x => x.F_Unit).HasComment("单位 - 数据值单位");
            b.Property(x => x.F_IsCollectionEnabled).HasComment("启用采集 - 是否启用此数据点采集");
            b.Property(x => x.F_CollectionPriority).HasComment("采集优先级 - 1:高 2:中 3:低");
            b.Property(x => x.F_Description).HasComment("数据点描述 - 数据点详细说明");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
            b.Property(x => x.F_SortOrder).HasComment("排序号 - 用于显示排序");
        });

        // 配置JavaScript规则实体
        builder.Entity<Rules.JavaScriptRule>(b =>
        {
            b.ToTable("T_JAVASCRIPT_RULES", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("JavaScript规则表 - 存储采集端边缘计算规则脚本"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => x.F_RuleCode).IsUnique();
            b.HasIndex(x => x.F_RuleName);
            b.HasIndex(x => x.F_DeviceId);
            b.HasIndex(x => x.F_IsEnabled);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_DeviceId).HasComment("设备ID - 关联设备表外键，为空表示全局规则");
            b.Property(x => x.F_RuleCode).HasComment("规则编码 - 全局唯一标识");
            b.Property(x => x.F_RuleName).HasComment("规则名称 - 规则显示名称");
            b.Property(x => x.F_RuleType).HasComment("规则类型 - 数据处理、告警、控制等");
            b.Property(x => x.F_TriggerCondition).HasComment("触发条件 - 时间触发、数据变化触发等");
            b.Property(x => x.F_ScriptCode).HasComment("脚本代码 - JavaScript代码内容");
            b.Property(x => x.F_ExecutionPriority).HasComment("执行优先级 - 1:高 2:中 3:低");
            b.Property(x => x.F_IsHotUpdateEnabled).HasComment("启用热更新 - 是否支持热更新");
            b.Property(x => x.F_ExecutionCount).HasComment("执行次数 - 规则执行统计");
            b.Property(x => x.F_LastExecutionTime).HasComment("最后执行时间 - 规则最后执行时间");
            b.Property(x => x.F_Description).HasComment("规则描述 - 规则详细说明");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
        });

        // 配置Node-RED流程实体
        builder.Entity<Rules.NodeRedFlow>(b =>
        {
            b.ToTable("T_NODERED_FLOWS", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("Node-RED流程表 - 存储服务端可视化数据处理流程"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => x.F_FlowCode).IsUnique();
            b.HasIndex(x => x.F_FlowName);
            b.HasIndex(x => x.F_FlowType);
            b.HasIndex(x => x.F_IsEnabled);
            b.HasIndex(x => x.F_Status);
            
            // 配置字段注释
            b.Property(x => x.F_FlowCode).HasComment("流程编码 - 全局唯一标识");
            b.Property(x => x.F_FlowName).HasComment("流程名称 - 流程显示名称");
            b.Property(x => x.F_FlowType).HasComment("流程类型 - 数据汇集、数据转换、告警处理等");
            b.Property(x => x.F_FlowCategory).HasComment("流程分类 - 实时处理、批量处理、定时任务等");
            b.Property(x => x.F_FlowConfig).HasComment("流程配置 - Node-RED流程JSON配置");
            b.Property(x => x.F_Version).HasComment("版本号 - 流程版本标识");
            b.Property(x => x.F_TabId).HasComment("标签页ID - Node-RED标签页标识");
            b.Property(x => x.F_ExecutionPriority).HasComment("执行优先级 - 1:高 2:中 3:低");
            b.Property(x => x.F_MaxProcessingRate).HasComment("最大处理速率 - 每秒处理条数");
            b.Property(x => x.F_IsHotDeployEnabled).HasComment("启用热部署 - 是否支持热部署");
            b.Property(x => x.F_LastDeployTime).HasComment("最后部署时间 - 流程最后部署时间");
            b.Property(x => x.F_LastExecutionTime).HasComment("最后执行时间 - 流程最后执行时间");
            b.Property(x => x.F_ExecutionCount).HasComment("执行次数 - 流程执行统计");
            b.Property(x => x.F_IsEnabled).HasComment("是否启用 - 流程启用状态");
            b.Property(x => x.F_Status).HasComment("状态 - 0:禁用 1:启用");
        });

        // 配置采集节点实体
        builder.Entity<Monitoring.CollectorNode>(b =>
        {
            b.ToTable("T_COLLECTOR_NODES", IoTDataCollectionConsts.DbSchema, 
                t => t.HasComment("采集节点表 - 存储数据采集端节点信息和状态"));
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => x.F_NodeCode).IsUnique();
            b.HasIndex(x => x.F_IpAddress).IsUnique();
            b.HasIndex(x => x.F_NodeName);
            b.HasIndex(x => x.F_NodeType);
            b.HasIndex(x => x.F_NodeStatus);
            b.HasIndex(x => x.F_ConnectionStatus);
            b.HasIndex(x => x.F_IsMonitoringEnabled);
            b.HasIndex(x => x.F_LastHeartbeatTime);
            
            // 配置字段注释
            b.Property(x => x.F_NodeCode).HasComment("节点编码 - 全局唯一标识");
            b.Property(x => x.F_NodeName).HasComment("节点名称 - 节点显示名称");
            b.Property(x => x.F_NodeType).HasComment("节点类型 - 采集节点、监控节点、边缘节点等");
            b.Property(x => x.F_IpAddress).HasComment("IP地址 - 节点网络地址");
            b.Property(x => x.F_DataPort).HasComment("数据端口 - 节点数据通信端口");
            b.Property(x => x.F_NodeStatus).HasComment("节点状态 - 0:离线 1:在线 2:故障 3:维护中");
            b.Property(x => x.F_ConnectionStatus).HasComment("连接状态 - 0:断开 1:已连接 2:连接中");
            b.Property(x => x.F_IsMonitoringEnabled).HasComment("启用监控 - 是否启用系统监控");
            b.Property(x => x.F_CpuUsage).HasComment("CPU使用率 - 当前CPU使用百分比");
            b.Property(x => x.F_MemoryUsage).HasComment("内存使用率 - 当前内存使用百分比");
            b.Property(x => x.F_DiskUsage).HasComment("磁盘使用率 - 当前磁盘使用百分比");
            b.Property(x => x.F_HeartbeatInterval).HasComment("心跳间隔 - 心跳发送间隔秒数");
            b.Property(x => x.F_LastHeartbeatTime).HasComment("最后心跳时间 - 节点最后心跳时间");
            b.Property(x => x.F_Description).HasComment("节点描述 - 节点详细说明");
        });

        #endregion
    }
}

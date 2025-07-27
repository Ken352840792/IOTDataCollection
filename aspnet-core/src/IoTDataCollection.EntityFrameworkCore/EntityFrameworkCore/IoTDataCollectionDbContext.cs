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
            b.ToTable("T_ENTERPRISES", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention(); // 自动配置ABP基类属性
            
            // 创建索引
            b.HasIndex(x => x.F_EnterpriseCode).IsUnique();
            b.HasIndex(x => x.F_EnterpriseName);
            b.HasIndex(x => x.F_Status);
        });

        // 配置站点实体
        builder.Entity<Sites.Site>(b =>
        {
            b.ToTable("T_SITES", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_EnterpriseId, x.F_SiteCode }).IsUnique();
            b.HasIndex(x => x.F_SiteName);
            b.HasIndex(x => x.F_Status);
        });

        // 配置设备实体
        builder.Entity<Devices.Device>(b =>
        {
            b.ToTable("T_DEVICES", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_SiteId, x.F_DeviceCode }).IsUnique();
            b.HasIndex(x => x.F_DeviceName);
            b.HasIndex(x => x.F_ConnectionStatus);
            b.HasIndex(x => x.F_IsCollectionEnabled);
            b.HasIndex(x => x.F_Status);
        });

        // 配置数据点实体
        builder.Entity<DataPoints.DataPoint>(b =>
        {
            b.ToTable("T_DATA_POINTS", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => new { x.F_DeviceId, x.F_PointCode }).IsUnique();
            b.HasIndex(x => x.F_PointName);
            b.HasIndex(x => x.F_IsCollectionEnabled);
            b.HasIndex(x => x.F_CollectionPriority);
            b.HasIndex(x => x.F_Status);
        });

        // 配置JavaScript规则实体
        builder.Entity<Rules.JavaScriptRule>(b =>
        {
            b.ToTable("T_JAVASCRIPT_RULES", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => x.F_RuleCode).IsUnique();
            b.HasIndex(x => x.F_RuleName);
            b.HasIndex(x => x.F_DeviceId);
            b.HasIndex(x => x.F_IsEnabled);
            b.HasIndex(x => x.F_Status);
        });

        // 配置Node-RED流程实体
        builder.Entity<Rules.NodeRedFlow>(b =>
        {
            b.ToTable("T_NODERED_FLOWS", IoTDataCollectionConsts.DbSchema);
            b.ConfigureByConvention();
            
            // 创建索引
            b.HasIndex(x => x.F_FlowCode).IsUnique();
            b.HasIndex(x => x.F_FlowName);
            b.HasIndex(x => x.F_FlowType);
            b.HasIndex(x => x.F_IsEnabled);
            b.HasIndex(x => x.F_Status);
        });

        // 配置采集节点实体
        builder.Entity<Monitoring.CollectorNode>(b =>
        {
            b.ToTable("T_COLLECTOR_NODES", IoTDataCollectionConsts.DbSchema);
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
        });

        #endregion
    }
}

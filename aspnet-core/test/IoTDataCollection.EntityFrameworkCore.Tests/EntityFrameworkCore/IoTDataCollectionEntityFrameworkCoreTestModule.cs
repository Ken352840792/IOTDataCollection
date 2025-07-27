using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Sqlite;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.Uow;

namespace IoTDataCollection.EntityFrameworkCore;

[DependsOn(
    typeof(IoTDataCollectionApplicationTestModule),
    typeof(IoTDataCollectionEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreSqliteModule)
    )]
public class IoTDataCollectionEntityFrameworkCoreTestModule : AbpModule
{
    private SqliteConnection? _sqliteConnection;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<FeatureManagementOptions>(options =>
        {
            options.SaveStaticFeaturesToDatabase = false;
            options.IsDynamicFeatureStoreEnabled = false;
        });
        Configure<PermissionManagementOptions>(options =>
        {
            options.SaveStaticPermissionsToDatabase = false;
            options.IsDynamicPermissionStoreEnabled = false;
        });
        Configure<SettingManagementOptions>(options =>
        {
            options.SaveStaticSettingsToDatabase = false;
            options.IsDynamicSettingStoreEnabled = false;
        });
        context.Services.AddAlwaysDisableUnitOfWorkTransaction();

        // 添加测试配置
        ConfigureTestConfiguration(context.Services);

        ConfigureInMemorySqlite(context.Services);
    }

    private void ConfigureTestConfiguration(IServiceCollection services)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                // MySQL连接字符串（测试环境）
                {"ConnectionStrings:Default", "Server=localhost;Port=3306;Database=IoTDataCollection_Test;Uid=root;Pwd=iot123456;CharSet=utf8mb4;SslMode=None;"},
                
                // InfluxDB连接字符串（测试环境）
                {"ConnectionStrings:InfluxDB", "http://localhost:8086"},
                
                // Redis连接字符串（测试环境）
                {"ConnectionStrings:Redis", "127.0.0.1"},
                {"Redis:Configuration", "127.0.0.1"},
                
                // InfluxDB配置 - 使用与生产环境相同的bucket名称
                {"InfluxDB:Token", "iot-admin-token-12345678901234567890"},
                {"InfluxDB:Organization", "IOTDataCollection"},
                {"InfluxDB:Bucket", "iot_data"},
                
                // 其他测试配置
                {"App:CorsOrigins", "https://localhost:44356"},
                {"AuthServer:Authority", "https://localhost:44331"},
                {"AuthServer:RequireHttpsMetadata", "true"},
                {"AuthServer:SwaggerClientId", "IoTDataCollection_Swagger"},
                {"StringEncryption:DefaultPassPhrase", "9nyPOJ86U1lfL1F1"}
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
    }

    private void ConfigureInMemorySqlite(IServiceCollection services)
    {
        _sqliteConnection = CreateDatabaseAndGetConnection();

        services.Configure<AbpDbContextOptions>(options =>
        {
            options.Configure(context =>
            {
                context.DbContextOptions.UseSqlite(_sqliteConnection);
            });
        });
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _sqliteConnection?.Dispose();
    }

    private static SqliteConnection CreateDatabaseAndGetConnection()
    {
        var connection = new AbpUnitTestSqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<IoTDataCollectionDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new IoTDataCollectionDbContext(options))
        {
            context.GetService<IRelationalDatabaseCreator>().CreateTables();
        }

        return connection;
    }
}

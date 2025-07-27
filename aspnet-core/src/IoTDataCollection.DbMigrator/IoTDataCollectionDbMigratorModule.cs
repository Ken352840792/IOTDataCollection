using IoTDataCollection.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Modularity;

namespace IoTDataCollection.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpCachingStackExchangeRedisModule),
    typeof(IoTDataCollectionEntityFrameworkCoreModule),
    typeof(IoTDataCollectionApplicationContractsModule)
    )]
public class IoTDataCollectionDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedCacheOptions>(options => { options.KeyPrefix = "IoTDataCollection:"; });
    }
}

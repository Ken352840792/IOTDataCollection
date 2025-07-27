using Volo.Abp.Modularity;

namespace IoTDataCollection;

[DependsOn(
    typeof(IoTDataCollectionApplicationModule),
    typeof(IoTDataCollectionDomainTestModule)
)]
public class IoTDataCollectionApplicationTestModule : AbpModule
{

}

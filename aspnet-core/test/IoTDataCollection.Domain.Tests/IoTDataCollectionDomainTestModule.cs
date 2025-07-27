using Volo.Abp.Modularity;

namespace IoTDataCollection;

[DependsOn(
    typeof(IoTDataCollectionDomainModule),
    typeof(IoTDataCollectionTestBaseModule)
)]
public class IoTDataCollectionDomainTestModule : AbpModule
{

}

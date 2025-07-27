using Volo.Abp.Modularity;

namespace IoTDataCollection;

public abstract class IoTDataCollectionApplicationTestBase<TStartupModule> : IoTDataCollectionTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

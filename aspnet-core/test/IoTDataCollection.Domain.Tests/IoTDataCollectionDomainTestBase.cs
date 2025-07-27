using Volo.Abp.Modularity;

namespace IoTDataCollection;

/* Inherit from this class for your domain layer tests. */
public abstract class IoTDataCollectionDomainTestBase<TStartupModule> : IoTDataCollectionTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}

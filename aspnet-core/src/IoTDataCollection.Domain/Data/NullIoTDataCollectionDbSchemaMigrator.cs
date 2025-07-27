using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace IoTDataCollection.Data;

/* This is used if database provider does't define
 * IIoTDataCollectionDbSchemaMigrator implementation.
 */
public class NullIoTDataCollectionDbSchemaMigrator : IIoTDataCollectionDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}

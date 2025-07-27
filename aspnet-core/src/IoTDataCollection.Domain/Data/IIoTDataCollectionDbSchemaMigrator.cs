using System.Threading.Tasks;

namespace IoTDataCollection.Data;

public interface IIoTDataCollectionDbSchemaMigrator
{
    Task MigrateAsync();
}

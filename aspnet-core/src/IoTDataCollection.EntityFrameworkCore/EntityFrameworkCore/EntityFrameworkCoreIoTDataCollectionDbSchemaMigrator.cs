using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using IoTDataCollection.Data;
using Volo.Abp.DependencyInjection;

namespace IoTDataCollection.EntityFrameworkCore;

public class EntityFrameworkCoreIoTDataCollectionDbSchemaMigrator
    : IIoTDataCollectionDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreIoTDataCollectionDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the IoTDataCollectionDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<IoTDataCollectionDbContext>()
            .Database
            .MigrateAsync();
    }
}

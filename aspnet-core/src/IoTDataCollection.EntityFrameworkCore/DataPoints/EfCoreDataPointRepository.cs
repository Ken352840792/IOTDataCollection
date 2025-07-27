using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using IoTDataCollection.EntityFrameworkCore;

namespace IoTDataCollection.DataPoints;

/// <summary>
/// 数据点仓储EF Core实现
/// </summary>
public class EfCoreDataPointRepository : EfCoreRepository<IoTDataCollectionDbContext, DataPoint, Guid>, IDataPointRepository
{
    public EfCoreDataPointRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<DataPoint?> FindByCodeAsync(
        Guid deviceId,
        string pointCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_DeviceId == deviceId && x.F_PointCode == pointCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataPoint>> GetListByDeviceAsync(
        Guid deviceId, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_DeviceId == deviceId);
        query = ApplyFilter(query, filter);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataPoint>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        Guid? deviceId = null, 
        bool? isCollectionEnabled = null, 
        int? collectionPriority = null, 
        string? dataType = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter, deviceId, isCollectionEnabled, collectionPriority, dataType);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataPoint>> GetCollectionEnabledDataPointsAsync(
        Guid? deviceId = null, 
        int? priority = null, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_CollectionPriority, F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsCollectionEnabled == true);
        
        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }
        
        if (priority.HasValue)
        {
            query = query.Where(x => x.F_CollectionPriority == priority.Value);
        }
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_CollectionPriority, F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataPoint>> GetHighPriorityDataPointsAsync(
        Guid? deviceId = null, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsCollectionEnabled == true && x.F_CollectionPriority <= 2);
        
        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }
        
        return await query
            .OrderBy("F_CollectionPriority, F_SortOrder")
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<DataPoint>> GetWritableDataPointsAsync(
        Guid? deviceId = null, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        // Note: DataPoint doesn't have F_IsWritable property
        // For now, return all data points (this logic should be refined based on business rules)
        
        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null, 
        Guid? deviceId = null, 
        bool? isCollectionEnabled = null, 
        int? collectionPriority = null, 
        string? dataType = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filter, deviceId, isCollectionEnabled, collectionPriority, dataType);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        Guid deviceId, 
        string pointCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_DeviceId == deviceId && x.F_PointCode == pointCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task UpdateLastCollectionAsync(
        Guid dataPointId, 
        string lastValue, 
        DateTime? lastCollectionTime = null, 
        CancellationToken cancellationToken = default)
    {
        var dataPoint = await GetAsync(dataPointId, cancellationToken: GetCancellationToken(cancellationToken));
        
        dataPoint.F_LastValue = lastValue;
        dataPoint.F_LastCollectionTime = lastCollectionTime ?? DateTime.UtcNow;
        
        await UpdateAsync(dataPoint, autoSave: true, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public async Task BatchUpdateLastCollectionAsync(
        Dictionary<Guid, (string lastValue, DateTime lastCollectionTime)> dataPointUpdates, 
        CancellationToken cancellationToken = default)
    {
        var dbContext = await GetDbContextAsync();
        
        var dataPointIds = dataPointUpdates.Keys.ToList();
        var dataPoints = await (await GetQueryableAsync())
            .Where(x => dataPointIds.Contains(x.Id))
            .ToListAsync(GetCancellationToken(cancellationToken));
        
        foreach (var dataPoint in dataPoints)
        {
            if (dataPointUpdates.TryGetValue(dataPoint.Id, out var update))
            {
                dataPoint.F_LastValue = update.lastValue;
                dataPoint.F_LastCollectionTime = update.lastCollectionTime;
            }
        }
        
        await dbContext.SaveChangesAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<DataPoint> ApplyFilter(
        IQueryable<DataPoint> query,
        string? filter,
        Guid? deviceId = null,
        bool? isCollectionEnabled = null,
        int? collectionPriority = null,
        string? dataType = null)
    {
        if (!filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_PointName.Contains(filter) || 
                                     x.F_PointCode.Contains(filter) ||
                                     (x.F_Description != null && x.F_Description.Contains(filter)));
        }

        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }

        if (isCollectionEnabled.HasValue)
        {
            query = query.Where(x => x.F_IsCollectionEnabled == isCollectionEnabled.Value);
        }

        if (collectionPriority.HasValue)
        {
            query = query.Where(x => x.F_CollectionPriority == collectionPriority.Value);
        }

        if (!dataType.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_DataType == dataType);
        }

        return query;
    }
} 
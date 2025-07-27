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

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备仓储EF Core实现
/// </summary>
public class EfCoreDeviceRepository : EfCoreRepository<IoTDataCollectionDbContext, Device, Guid>, IDeviceRepository
{
    public EfCoreDeviceRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Device?> FindByCodeAsync(
        Guid siteId,
        string deviceCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_SiteId == siteId && x.F_DeviceCode == deviceCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Device>> GetListBySiteAsync(
        Guid siteId, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_SiteId == siteId);
        query = ApplyFilter(query, filter);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Device>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        Guid? siteId = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, siteId, connectionStatus: connectionStatus, isCollectionEnabled: isCollectionEnabled);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Device>> GetOnlineDevicesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_ConnectionStatus == 1); // 在线状态
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Device>> GetCollectionEnabledDevicesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsCollectionEnabled == true && x.F_Status == 1);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null,
        Guid? siteId = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, siteId, connectionStatus: connectionStatus, isCollectionEnabled: isCollectionEnabled);
        
        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        Guid siteId,
        string deviceCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_SiteId == siteId && x.F_DeviceCode == deviceCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task UpdateConnectionStatusAsync(
        Guid deviceId, 
        int connectionStatus, 
        DateTime? lastCommunicationTime = null, 
        CancellationToken cancellationToken = default)
    {
        var device = await GetAsync(deviceId, cancellationToken: GetCancellationToken(cancellationToken));
        
        device.F_ConnectionStatus = connectionStatus;
        if (lastCommunicationTime.HasValue)
        {
            device.F_LastCommunicationTime = lastCommunicationTime.Value;
        }
        
        await UpdateAsync(device, cancellationToken: GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Device> ApplyFilter(
        IQueryable<Device> query,
        string? filter)
    {
        return query
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                e => e.F_DeviceName.Contains(filter!) ||
                     e.F_DeviceCode.Contains(filter!) ||
                     (e.F_DeviceType != null && e.F_DeviceType.Contains(filter!)) ||
                     (e.F_Manufacturer != null && e.F_Manufacturer.Contains(filter!)) ||
                     (e.F_CommunicationProtocol != null && e.F_CommunicationProtocol.Contains(filter!))
            );
    }

    protected virtual IQueryable<Device> ApplyDetailedFilter(
        IQueryable<Device> query,
        Guid? siteId = null,
        string? deviceType = null,
        string? communicationProtocol = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        int? status = null)
    {
        return query
            .WhereIf(siteId.HasValue, e => e.F_SiteId == siteId.Value)
            .WhereIf(!deviceType.IsNullOrWhiteSpace(), e => e.F_DeviceType == deviceType)
            .WhereIf(!communicationProtocol.IsNullOrWhiteSpace(), e => e.F_CommunicationProtocol == communicationProtocol)
            .WhereIf(connectionStatus.HasValue, e => e.F_ConnectionStatus == connectionStatus.Value)
            .WhereIf(isCollectionEnabled.HasValue, e => e.F_IsCollectionEnabled == isCollectionEnabled.Value)
            .WhereIf(status.HasValue, e => e.F_Status == status.Value);
    }

    public async Task<List<Device>> GetListWithFiltersAsync(
        Guid? siteId = null,
        string? deviceType = null,
        string? communicationProtocol = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        int? status = null,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, siteId, deviceType, communicationProtocol, connectionStatus, isCollectionEnabled, status);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountWithFiltersAsync(
        Guid? siteId = null,
        string? deviceType = null,
        string? communicationProtocol = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        int? status = null,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, siteId, deviceType, communicationProtocol, connectionStatus, isCollectionEnabled, status);
        
        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }
} 
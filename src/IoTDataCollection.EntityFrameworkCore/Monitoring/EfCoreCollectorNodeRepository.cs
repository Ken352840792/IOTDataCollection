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

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点仓储EF Core实现
/// </summary>
public class EfCoreCollectorNodeRepository : EfCoreRepository<IoTDataCollectionDbContext, CollectorNode, Guid>, ICollectorNodeRepository
{
    public EfCoreCollectorNodeRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<CollectorNode?> FindByCodeAsync(
        string nodeCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_NodeCode == nodeCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<CollectorNode?> FindByIpAddressAsync(
        string ipAddress, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_IpAddress == ipAddress)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetOnlineNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_NodeStatus == 1); // 在线状态
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetFaultNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_NodeStatus == 2); // 故障状态
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetMonitoringEnabledNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsMonitoringEnabled == true);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetTimeoutNodesAsync(
        int? timeoutThreshold = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        var currentTime = DateTime.Now;
        
        return await query
            .Where(x => x.F_LastHeartbeatTime.HasValue && 
                       x.F_IsMonitoringEnabled &&
                       EF.Functions.DateDiffSecond(x.F_LastHeartbeatTime.Value, currentTime) > 
                       (timeoutThreshold ?? x.F_TimeoutThreshold))
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        
        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        string nodeCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_NodeCode == nodeCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsIpAddressExistAsync(
        string ipAddress, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IpAddress == ipAddress);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task UpdateHeartbeatAsync(
        Guid nodeId, 
        DateTime heartbeatTime, 
        NodePerformanceData? performanceData = null, 
        CancellationToken cancellationToken = default)
    {
        var node = await GetAsync(nodeId, cancellationToken: GetCancellationToken(cancellationToken));
        
        node.UpdateHeartbeat(heartbeatTime, performanceData);
        
        await UpdateAsync(node, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public async Task BatchUpdateNodeStatusAsync(
        List<NodeStatusUpdate> nodeStatusUpdates, 
        CancellationToken cancellationToken = default)
    {
        foreach (var update in nodeStatusUpdates)
        {
            var node = await GetAsync(update.NodeId, cancellationToken: GetCancellationToken(cancellationToken));
            
            node.F_NodeStatus = update.NodeStatus;
            node.F_ConnectionStatus = update.ConnectionStatus;
            
            if (update.LastHeartbeatTime.HasValue)
            {
                node.F_LastHeartbeatTime = update.LastHeartbeatTime.Value;
            }
            
            if (update.LastOfflineTime.HasValue)
            {
                node.F_LastOfflineTime = update.LastOfflineTime.Value;
            }
            
            if (update.PerformanceData != null)
            {
                node.F_CpuUsage = update.PerformanceData.CpuUsage;
                node.F_MemoryUsage = update.PerformanceData.MemoryUsage;
                node.F_DiskUsage = update.PerformanceData.DiskUsage;
                node.F_NetworkTraffic = update.PerformanceData.NetworkTraffic;
                node.F_DataCollectionRate = update.PerformanceData.DataCollectionRate;
            }
            
            await UpdateAsync(node, cancellationToken: GetCancellationToken(cancellationToken));
        }
    }

    public async Task UpdateDeviceStatisticsAsync(
        Guid nodeId, 
        int deviceCount, 
        int onlineDeviceCount, 
        int errorDataCount, 
        CancellationToken cancellationToken = default)
    {
        var node = await GetAsync(nodeId, cancellationToken: GetCancellationToken(cancellationToken));
        
        node.UpdateDeviceStatistics(deviceCount, onlineDeviceCount, errorDataCount);
        
        await UpdateAsync(node, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public async Task<List<CollectorNode>> GetListWithFiltersAsync(
        string? nodeType = null,
        int? nodeStatus = null,
        int? connectionStatus = null,
        bool? isMonitoringEnabled = null,
        bool? isAlertEnabled = null,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, nodeType, nodeStatus, connectionStatus, isMonitoringEnabled, isAlertEnabled);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .PageBy(skipCount, maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountWithFiltersAsync(
        string? nodeType = null,
        int? nodeStatus = null,
        int? connectionStatus = null,
        bool? isMonitoringEnabled = null,
        bool? isAlertEnabled = null,
        string? filter = null,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        query = ApplyDetailedFilter(query, nodeType, nodeStatus, connectionStatus, isMonitoringEnabled, isAlertEnabled);
        
        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<CollectorNode> ApplyFilter(
        IQueryable<CollectorNode> query,
        string? filter)
    {
        return query
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                e => e.F_NodeName.Contains(filter!) ||
                     e.F_NodeCode.Contains(filter!) ||
                     e.F_IpAddress.Contains(filter!) ||
                     (e.F_Description != null && e.F_Description.Contains(filter!)) ||
                     (e.F_ResponsiblePerson != null && e.F_ResponsiblePerson.Contains(filter!))
            );
    }

    protected virtual IQueryable<CollectorNode> ApplyDetailedFilter(
        IQueryable<CollectorNode> query,
        string? nodeType = null,
        int? nodeStatus = null,
        int? connectionStatus = null,
        bool? isMonitoringEnabled = null,
        bool? isAlertEnabled = null)
    {
        return query
            .WhereIf(!nodeType.IsNullOrWhiteSpace(), e => e.F_NodeType == nodeType)
            .WhereIf(nodeStatus.HasValue, e => e.F_NodeStatus == nodeStatus.Value)
            .WhereIf(connectionStatus.HasValue, e => e.F_ConnectionStatus == connectionStatus.Value)
            .WhereIf(isMonitoringEnabled.HasValue, e => e.F_IsMonitoringEnabled == isMonitoringEnabled.Value)
            .WhereIf(isAlertEnabled.HasValue, e => e.F_IsAlertEnabled == isAlertEnabled.Value);
    }
} 
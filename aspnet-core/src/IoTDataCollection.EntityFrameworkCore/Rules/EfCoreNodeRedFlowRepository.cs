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

namespace IoTDataCollection.Rules;

/// <summary>
/// Node-RED流程仓储EF Core实现
/// </summary>
public class EfCoreNodeRedFlowRepository : EfCoreRepository<IoTDataCollectionDbContext, NodeRedFlow, Guid>, INodeRedFlowRepository
{
    public EfCoreNodeRedFlowRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<NodeRedFlow?> FindByCodeAsync(
        string flowCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_FlowCode == flowCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<NodeRedFlow>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_Priority DESC, F_FlowName", 
        string? filter = null, 
        string? flowType = null, 
        bool? isEnabled = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter, flowType, isEnabled);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_Priority DESC, F_FlowName" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<NodeRedFlow>> GetEnabledFlowsAsync(
        string? flowType = null, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_Priority DESC, F_FlowName", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsEnabled == true);
        
        if (!flowType.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_FlowType == flowType);
        }
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_Priority DESC, F_FlowName" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null, 
        string? flowType = null, 
        bool? isEnabled = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filter, flowType, isEnabled);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        string flowCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_FlowCode == flowCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task UpdateExecutionStatusAsync(
        Guid flowId, 
        DateTime? lastExecutionTime = null, 
        long? executionCount = null, 
        string? lastExecutionResult = null, 
        CancellationToken cancellationToken = default)
    {
        var flow = await GetAsync(flowId, cancellationToken: GetCancellationToken(cancellationToken));
        
        if (lastExecutionTime.HasValue)
        {
            flow.F_LastExecutionTime = lastExecutionTime.Value;
        }
        
        if (executionCount.HasValue)
        {
            flow.F_ExecutionCount = executionCount.Value;
        }
        
        // Note: NodeRedFlow doesn't have F_LastExecutionResult property
        // This parameter is ignored for now
        
        await UpdateAsync(flow, autoSave: true, cancellationToken: GetCancellationToken(cancellationToken));
    }

    public async Task UpdateVersionAsync(
        Guid flowId, 
        int version, 
        string flowDefinition, 
        CancellationToken cancellationToken = default)
    {
        var flow = await GetAsync(flowId, cancellationToken: GetCancellationToken(cancellationToken));
        
        flow.F_Version = version.ToString();
        flow.F_FlowConfig = flowDefinition;
        // Note: NodeRedFlow doesn't have F_FlowHash property
        
        await UpdateAsync(flow, autoSave: true, cancellationToken: GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<NodeRedFlow> ApplyFilter(
        IQueryable<NodeRedFlow> query,
        string? filter,
        string? flowType = null,
        bool? isEnabled = null)
    {
        if (!filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_FlowName.Contains(filter) || 
                                     x.F_FlowCode.Contains(filter) ||
                                     (x.F_Description != null && x.F_Description.Contains(filter)));
        }

        if (!flowType.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_FlowType == flowType);
        }

        if (isEnabled.HasValue)
        {
            query = query.Where(x => x.F_IsEnabled == isEnabled.Value);
        }

        return query;
    }
}

/// <summary>
/// 哈希辅助类
/// </summary>
public static class HashHelper
{
    public static string GenerateHash(string input)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
} 
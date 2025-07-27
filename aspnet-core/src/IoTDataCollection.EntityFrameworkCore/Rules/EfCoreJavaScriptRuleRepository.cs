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
/// JavaScript规则仓储EF Core实现
/// </summary>
public class EfCoreJavaScriptRuleRepository : EfCoreRepository<IoTDataCollectionDbContext, JavaScriptRule, Guid>, IJavaScriptRuleRepository
{
    public EfCoreJavaScriptRuleRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<JavaScriptRule?> FindByCodeAsync(
        string ruleCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_RuleCode == ruleCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<JavaScriptRule>> GetListByDeviceAsync(
        Guid? deviceId, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_Priority DESC, F_RuleName", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_DeviceId == deviceId);
        query = ApplyFilter(query, filter);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_Priority DESC, F_RuleName" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<JavaScriptRule>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_Priority DESC, F_RuleName", 
        string? filter = null, 
        Guid? deviceId = null, 
        string? ruleType = null, 
        bool? isEnabled = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter, deviceId, ruleType, isEnabled);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_Priority DESC, F_RuleName" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<JavaScriptRule>> GetEnabledRulesAsync(
        Guid? deviceId = null, 
        string? ruleType = null, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_Priority DESC, F_RuleName", 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_IsEnabled == true);
        
        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }
        
        if (!ruleType.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_RuleType == ruleType);
        }
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_Priority DESC, F_RuleName" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null, 
        Guid? deviceId = null, 
        string? ruleType = null, 
        bool? isEnabled = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filter, deviceId, ruleType, isEnabled);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        string ruleCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_RuleCode == ruleCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    public async Task UpdateExecutionStatusAsync(
        Guid ruleId, 
        DateTime? lastExecutionTime = null, 
        long? executionCount = null, 
        string? lastExecutionResult = null, 
        CancellationToken cancellationToken = default)
    {
        var rule = await GetAsync(ruleId, cancellationToken: GetCancellationToken(cancellationToken));
        
        if (lastExecutionTime.HasValue)
        {
            rule.F_LastExecutionTime = lastExecutionTime.Value;
        }
        
        if (executionCount.HasValue)
        {
            rule.F_ExecutionCount = executionCount.Value;
        }
        
        if (!lastExecutionResult.IsNullOrWhiteSpace())
        {
            rule.F_LastExecutionResult = lastExecutionResult;
        }
        
        await UpdateAsync(rule, autoSave: true, cancellationToken: GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<JavaScriptRule> ApplyFilter(
        IQueryable<JavaScriptRule> query,
        string? filter,
        Guid? deviceId = null,
        string? ruleType = null,
        bool? isEnabled = null)
    {
        if (!filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_RuleName.Contains(filter) || 
                                     x.F_RuleCode.Contains(filter) ||
                                     (x.F_Description != null && x.F_Description.Contains(filter)));
        }

        if (deviceId.HasValue)
        {
            query = query.Where(x => x.F_DeviceId == deviceId.Value);
        }

        if (!ruleType.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_RuleType == ruleType);
        }

        if (isEnabled.HasValue)
        {
            query = query.Where(x => x.F_IsEnabled == isEnabled.Value);
        }

        return query;
    }
} 
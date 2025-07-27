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

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业仓储EF Core实现
/// </summary>
public class EfCoreEnterpriseRepository : EfCoreRepository<IoTDataCollectionDbContext, Enterprise, Guid>, IEnterpriseRepository
{
    public EfCoreEnterpriseRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Enterprise?> FindByCodeAsync(
        string enterpriseCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_EnterpriseCode == enterpriseCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<Enterprise?> FindByNameAsync(
        string enterpriseName, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_EnterpriseName == enterpriseName)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Enterprise>> GetListAsync(
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

    public async Task<long> GetCountAsync(
        string? filter = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter);
        
        return await query.CountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        string enterpriseCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_EnterpriseCode == enterpriseCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Enterprise> ApplyFilter(
        IQueryable<Enterprise> query,
        string? filter)
    {
        return query
            .WhereIf(
                !filter.IsNullOrWhiteSpace(),
                e => e.F_EnterpriseName.Contains(filter!) ||
                     e.F_EnterpriseCode.Contains(filter!) ||
                     (e.F_ShortName != null && e.F_ShortName.Contains(filter!)) ||
                     (e.F_Description != null && e.F_Description.Contains(filter!))
            );
    }
} 
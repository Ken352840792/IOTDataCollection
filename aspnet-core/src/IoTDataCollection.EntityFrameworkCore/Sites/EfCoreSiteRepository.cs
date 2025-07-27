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

namespace IoTDataCollection.Sites;

/// <summary>
/// 站点仓储EF Core实现
/// </summary>
public class EfCoreSiteRepository : EfCoreRepository<IoTDataCollectionDbContext, Site, Guid>, ISiteRepository
{
    public EfCoreSiteRepository(IDbContextProvider<IoTDataCollectionDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public async Task<Site?> FindByCodeAsync(
        Guid enterpriseId,
        string siteCode, 
        bool includeDetails = true, 
        CancellationToken cancellationToken = default)
    {
        return await (await GetQueryableAsync())
            .Where(x => x.F_EnterpriseId == enterpriseId && x.F_SiteCode == siteCode)
            .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Site>> GetListByEnterpriseAsync(
        Guid enterpriseId, 
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_EnterpriseId == enterpriseId);
        query = ApplyFilter(query, filter);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<List<Site>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        Guid? enterpriseId = null, 
        bool includeDetails = false,
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = ApplyFilter(query, filter, enterpriseId);
        
        return await query
            .OrderBy(sorting.IsNullOrWhiteSpace() ? "F_SortOrder" : sorting)
            .Skip(skipCount)
            .Take(maxResultCount)
            .ToListAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<long> GetCountAsync(
        string? filter = null, 
        Guid? enterpriseId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        query = ApplyFilter(query, filter, enterpriseId);
        return await query.LongCountAsync(GetCancellationToken(cancellationToken));
    }

    public async Task<bool> IsCodeExistAsync(
        Guid enterpriseId,
        string siteCode, 
        Guid? excludeId = null, 
        CancellationToken cancellationToken = default)
    {
        var query = await GetQueryableAsync();
        
        query = query.Where(x => x.F_EnterpriseId == enterpriseId && x.F_SiteCode == siteCode);
        
        if (excludeId.HasValue)
        {
            query = query.Where(x => x.Id != excludeId.Value);
        }
        
        return await query.AnyAsync(GetCancellationToken(cancellationToken));
    }

    protected virtual IQueryable<Site> ApplyFilter(
        IQueryable<Site> query,
        string? filter,
        Guid? enterpriseId = null)
    {
        if (!filter.IsNullOrWhiteSpace())
        {
            query = query.Where(x => x.F_SiteName.Contains(filter) || 
                                     x.F_SiteCode.Contains(filter) ||
                                     (x.F_Address != null && x.F_Address.Contains(filter)));
        }

        if (enterpriseId.HasValue)
        {
            query = query.Where(x => x.F_EnterpriseId == enterpriseId.Value);
        }

        return query;
    }
} 
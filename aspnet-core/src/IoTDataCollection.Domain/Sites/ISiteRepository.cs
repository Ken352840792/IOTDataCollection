using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Sites;

/// <summary>
/// 站点仓储接口
/// </summary>
public interface ISiteRepository : IRepository<Site, Guid>
{
    /// <summary>
    /// 根据站点编码和企业ID查找站点
    /// </summary>
    /// <param name="enterpriseId">企业ID</param>
    /// <param name="siteCode">站点编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>站点实体</returns>
    Task<Site?> FindByCodeAsync(
        Guid enterpriseId,
        string siteCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据企业ID获取站点列表
    /// </summary>
    /// <param name="enterpriseId">企业ID</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>站点列表</returns>
    Task<List<Site>> GetListByEnterpriseAsync(
        Guid enterpriseId,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取站点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="enterpriseId">企业ID过滤</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>站点列表</returns>
    Task<List<Site>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        Guid? enterpriseId = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取站点总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="enterpriseId">企业ID过滤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? enterpriseId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查站点编码在企业内是否已存在
    /// </summary>
    /// <param name="enterpriseId">企业ID</param>
    /// <param name="siteCode">站点编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        Guid enterpriseId,
        string siteCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
} 
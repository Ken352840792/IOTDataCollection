using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业仓储接口
/// </summary>
public interface IEnterpriseRepository : IRepository<Enterprise, Guid>
{
    /// <summary>
    /// 根据企业编码查找企业
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>企业实体</returns>
    Task<Enterprise?> FindByCodeAsync(
        string enterpriseCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据企业名称查找企业
    /// </summary>
    /// <param name="enterpriseName">企业名称</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>企业实体</returns>
    Task<Enterprise?> FindByNameAsync(
        string enterpriseName, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>企业列表</returns>
    Task<List<Enterprise>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取企业总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查企业编码是否已存在
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        string enterpriseCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
} 
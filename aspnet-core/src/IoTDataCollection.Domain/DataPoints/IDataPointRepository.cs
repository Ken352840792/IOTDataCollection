using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.DataPoints;

/// <summary>
/// 数据点仓储接口
/// </summary>
public interface IDataPointRepository : IRepository<DataPoint, Guid>
{
    /// <summary>
    /// 根据数据点编码和设备ID查找数据点
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="pointCode">数据点编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点实体</returns>
    Task<DataPoint?> FindByCodeAsync(
        Guid deviceId,
        string pointCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据设备ID获取数据点列表
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点列表</returns>
    Task<List<DataPoint>> GetListByDeviceAsync(
        Guid deviceId,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="isCollectionEnabled">是否启用采集过滤</param>
    /// <param name="collectionPriority">采集优先级过滤</param>
    /// <param name="dataType">数据类型过滤</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点列表</returns>
    Task<List<DataPoint>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        Guid? deviceId = null,
        bool? isCollectionEnabled = null,
        int? collectionPriority = null,
        string? dataType = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取启用采集的数据点列表
    /// </summary>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="priority">优先级过滤</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>启用采集的数据点列表</returns>
    Task<List<DataPoint>> GetCollectionEnabledDataPointsAsync(
        Guid? deviceId = null,
        int? priority = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_CollectionPriority, F_SortOrder",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取高优先级数据点列表
    /// </summary>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>高优先级数据点列表</returns>
    Task<List<DataPoint>> GetHighPriorityDataPointsAsync(
        Guid? deviceId = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取可写数据点列表
    /// </summary>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可写数据点列表</returns>
    Task<List<DataPoint>> GetWritableDataPointsAsync(
        Guid? deviceId = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取数据点总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="isCollectionEnabled">是否启用采集过滤</param>
    /// <param name="collectionPriority">采集优先级过滤</param>
    /// <param name="dataType">数据类型过滤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? deviceId = null,
        bool? isCollectionEnabled = null,
        int? collectionPriority = null,
        string? dataType = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查数据点编码在设备内是否已存在
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="pointCode">数据点编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        Guid deviceId,
        string pointCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新数据点最后采集信息
    /// </summary>
    /// <param name="dataPointId">数据点ID</param>
    /// <param name="lastValue">最后采集值</param>
    /// <param name="lastCollectionTime">最后采集时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateLastCollectionAsync(
        Guid dataPointId,
        string lastValue,
        DateTime? lastCollectionTime = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新数据点最后采集信息
    /// </summary>
    /// <param name="dataPointUpdates">数据点更新信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task BatchUpdateLastCollectionAsync(
        Dictionary<Guid, (string lastValue, DateTime lastCollectionTime)> dataPointUpdates,
        CancellationToken cancellationToken = default);
} 
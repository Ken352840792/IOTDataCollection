using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备仓储接口
/// </summary>
public interface IDeviceRepository : IRepository<Device, Guid>
{
    /// <summary>
    /// 根据设备编码和站点ID查找设备
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备实体</returns>
    Task<Device?> FindByCodeAsync(
        Guid siteId,
        string deviceCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据站点ID获取设备列表
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备列表</returns>
    Task<List<Device>> GetListBySiteAsync(
        Guid siteId,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="siteId">站点ID过滤</param>
    /// <param name="connectionStatus">连接状态过滤</param>
    /// <param name="isCollectionEnabled">是否启用采集过滤</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备列表</returns>
    Task<List<Device>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        string? filter = null,
        Guid? siteId = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取在线设备列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>在线设备列表</returns>
    Task<List<Device>> GetOnlineDevicesAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_LastCommunicationTime DESC",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取启用采集的设备列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>启用采集的设备列表</returns>
    Task<List<Device>> GetCollectionEnabledDevicesAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="siteId">站点ID过滤</param>
    /// <param name="connectionStatus">连接状态过滤</param>
    /// <param name="isCollectionEnabled">是否启用采集过滤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? siteId = null,
        int? connectionStatus = null,
        bool? isCollectionEnabled = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查设备编码在站点内是否已存在
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        Guid siteId,
        string deviceCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新设备连接状态
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="connectionStatus">连接状态</param>
    /// <param name="lastCommunicationTime">最后通信时间</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateConnectionStatusAsync(
        Guid deviceId,
        int connectionStatus,
        DateTime? lastCommunicationTime = null,
        CancellationToken cancellationToken = default);
} 
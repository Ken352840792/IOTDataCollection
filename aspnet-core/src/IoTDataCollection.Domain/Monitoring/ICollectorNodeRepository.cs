using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点仓储接口
/// </summary>
public interface ICollectorNodeRepository : IRepository<CollectorNode, Guid>
{
    /// <summary>
    /// 根据节点编码查找节点
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <param name="includeDetails">是否包含详情</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采集节点</returns>
    Task<CollectorNode?> FindByCodeAsync(string nodeCode, bool includeDetails = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据IP地址查找节点
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="includeDetails">是否包含详情</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采集节点</returns>
    Task<CollectorNode?> FindByIpAddressAsync(string ipAddress, bool includeDetails = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取节点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详情</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点列表</returns>
    Task<List<CollectorNode>> GetListAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        string? filter = null, 
        bool includeDetails = false, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取在线节点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>在线节点列表</returns>
    Task<List<CollectorNode>> GetOnlineNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取故障节点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>故障节点列表</returns>
    Task<List<CollectorNode>> GetFaultNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取启用监控的节点列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>启用监控的节点列表</returns>
    Task<List<CollectorNode>> GetMonitoringEnabledNodesAsync(
        int skipCount = 0, 
        int maxResultCount = int.MaxValue, 
        string sorting = "F_SortOrder", 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取超时节点列表（心跳超时）
    /// </summary>
    /// <param name="timeoutThreshold">超时阈值（秒）</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>超时节点列表</returns>
    Task<List<CollectorNode>> GetTimeoutNodesAsync(
        int? timeoutThreshold = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取节点数量
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点数量</returns>
    Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查节点编码是否已存在
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <param name="excludeId">排除的节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(string nodeCode, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查IP地址是否已存在
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="excludeId">排除的节点ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsIpAddressExistAsync(string ipAddress, Guid? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新节点心跳信息
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="heartbeatTime">心跳时间</param>
    /// <param name="performanceData">性能数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateHeartbeatAsync(
        Guid nodeId, 
        DateTime heartbeatTime, 
        NodePerformanceData? performanceData = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 批量更新节点状态
    /// </summary>
    /// <param name="nodeStatusUpdates">节点状态更新列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task BatchUpdateNodeStatusAsync(
        List<NodeStatusUpdate> nodeStatusUpdates, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新节点设备统计信息
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="deviceCount">总设备数</param>
    /// <param name="onlineDeviceCount">在线设备数</param>
    /// <param name="errorDataCount">错误数据点数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateDeviceStatisticsAsync(
        Guid nodeId, 
        int deviceCount, 
        int onlineDeviceCount, 
        int errorDataCount, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带过滤条件的节点列表
    /// </summary>
    /// <param name="nodeType">节点类型</param>
    /// <param name="nodeStatus">节点状态</param>
    /// <param name="connectionStatus">连接状态</param>
    /// <param name="isMonitoringEnabled">是否启用监控</param>
    /// <param name="isAlertEnabled">是否启用告警</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点列表</returns>
    Task<List<CollectorNode>> GetListWithFiltersAsync(
        string? nodeType = null,
        int? nodeStatus = null,
        int? connectionStatus = null,
        bool? isMonitoringEnabled = null,
        bool? isAlertEnabled = null,
        string? filter = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_SortOrder",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取带过滤条件的节点数量
    /// </summary>
    /// <param name="nodeType">节点类型</param>
    /// <param name="nodeStatus">节点状态</param>
    /// <param name="connectionStatus">连接状态</param>
    /// <param name="isMonitoringEnabled">是否启用监控</param>
    /// <param name="isAlertEnabled">是否启用告警</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>节点数量</returns>
    Task<long> GetCountWithFiltersAsync(
        string? nodeType = null,
        int? nodeStatus = null,
        int? connectionStatus = null,
        bool? isMonitoringEnabled = null,
        bool? isAlertEnabled = null,
        string? filter = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// 节点状态更新
/// </summary>
public class NodeStatusUpdate
{
    public Guid NodeId { get; set; }
    public int NodeStatus { get; set; }
    public int ConnectionStatus { get; set; }
    public DateTime? LastHeartbeatTime { get; set; }
    public DateTime? LastOfflineTime { get; set; }
    public NodePerformanceData? PerformanceData { get; set; }
} 
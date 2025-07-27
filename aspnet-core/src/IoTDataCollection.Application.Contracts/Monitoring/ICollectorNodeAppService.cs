using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点应用服务接口
/// </summary>
public interface ICollectorNodeAppService : ICrudAppService<
    CollectorNodeDto,
    Guid,
    GetCollectorNodeListDto,
    CreateCollectorNodeDto,
    UpdateCollectorNodeDto>
{
    /// <summary>
    /// 根据节点编码查找节点
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <returns>节点信息</returns>
    Task<CollectorNodeDto?> FindByCodeAsync(string nodeCode);

    /// <summary>
    /// 根据IP地址查找节点
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <returns>节点信息</returns>
    Task<CollectorNodeDto?> FindByIpAddressAsync(string ipAddress);

    /// <summary>
    /// 获取在线节点列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>在线节点列表</returns>
    Task<PagedResultDto<CollectorNodeDto>> GetOnlineNodesAsync(GetCollectorNodeListDto input);

    /// <summary>
    /// 获取故障节点列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>故障节点列表</returns>
    Task<PagedResultDto<CollectorNodeDto>> GetFaultNodesAsync(GetCollectorNodeListDto input);

    /// <summary>
    /// 获取超时节点列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>超时节点列表</returns>
    Task<PagedResultDto<CollectorNodeDto>> GetTimeoutNodesAsync(GetCollectorNodeListDto input);

    /// <summary>
    /// 获取启用监控的节点列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>启用监控的节点列表</returns>
    Task<PagedResultDto<CollectorNodeDto>> GetMonitoringEnabledNodesAsync(GetCollectorNodeListDto input);

    /// <summary>
    /// 检查节点编码是否已存在
    /// </summary>
    /// <param name="nodeCode">节点编码</param>
    /// <param name="excludeId">排除的节点ID</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(string nodeCode, Guid? excludeId = null);

    /// <summary>
    /// 检查IP地址是否已存在
    /// </summary>
    /// <param name="ipAddress">IP地址</param>
    /// <param name="excludeId">排除的节点ID</param>
    /// <returns>是否存在</returns>
    Task<bool> IsIpAddressExistAsync(string ipAddress, Guid? excludeId = null);

    /// <summary>
    /// 启用节点监控
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task EnableMonitoringAsync(Guid id);

    /// <summary>
    /// 禁用节点监控
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task DisableMonitoringAsync(Guid id);

    /// <summary>
    /// 启用节点告警
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task EnableAlertAsync(Guid id);

    /// <summary>
    /// 禁用节点告警
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task DisableAlertAsync(Guid id);

    /// <summary>
    /// 设置节点为维护状态
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task SetMaintenanceAsync(Guid id);

    /// <summary>
    /// 设置节点为故障状态
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task SetFaultAsync(Guid id);

    /// <summary>
    /// 设置节点为离线状态
    /// </summary>
    /// <param name="id">节点ID</param>
    /// <returns></returns>
    Task SetOfflineAsync(Guid id);

    /// <summary>
    /// 获取节点统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    Task<CollectorNodeStatsDto> GetStatsAsync();

    /// <summary>
    /// 获取按节点类型分组的统计信息
    /// </summary>
    /// <param name="nodeType">节点类型</param>
    /// <returns>统计信息</returns>
    Task<CollectorNodeStatsDto> GetStatsByNodeTypeAsync(string nodeType);

    #region 心跳和监控功能

    /// <summary>
    /// 接收节点心跳
    /// </summary>
    /// <param name="request">心跳请求</param>
    /// <returns></returns>
    Task ReceiveHeartbeatAsync(NodeHeartbeatRequestDto request);

    /// <summary>
    /// 批量更新节点状态
    /// </summary>
    /// <param name="request">批量更新请求</param>
    /// <returns></returns>
    Task BatchUpdateNodeStatusAsync(BatchNodeStatusUpdateDto request);

    /// <summary>
    /// 更新节点监控配置
    /// </summary>
    /// <param name="config">监控配置</param>
    /// <returns></returns>
    Task UpdateMonitoringConfigAsync(NodeMonitoringConfigDto config);

    /// <summary>
    /// 执行节点健康检查
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <returns>健康检查结果</returns>
    Task<NodeHealthCheckResponseDto> PerformHealthCheckAsync(Guid nodeId);

    /// <summary>
    /// 批量执行健康检查
    /// </summary>
    /// <param name="nodeIds">节点ID列表</param>
    /// <returns>健康检查结果列表</returns>
    Task<List<NodeHealthCheckResponseDto>> BatchPerformHealthCheckAsync(List<Guid> nodeIds);

    /// <summary>
    /// 获取节点性能历史数据
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>性能数据列表</returns>
    Task<List<NodePerformanceDto>> GetNodePerformanceHistoryAsync(Guid nodeId, DateTime startTime, DateTime endTime);

    /// <summary>
    /// 检测超时节点并更新状态
    /// </summary>
    /// <returns>检测到的超时节点数量</returns>
    Task<int> DetectAndUpdateTimeoutNodesAsync();

    #endregion

    #region 节点控制功能

    /// <summary>
    /// 重启节点
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <returns>操作结果</returns>
    Task<bool> RestartNodeAsync(Guid nodeId);

    /// <summary>
    /// 停止节点
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <returns>操作结果</returns>
    Task<bool> StopNodeAsync(Guid nodeId);

    /// <summary>
    /// 启动节点
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <returns>操作结果</returns>
    Task<bool> StartNodeAsync(Guid nodeId);

    /// <summary>
    /// 向节点发送配置更新命令
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="configData">配置数据</param>
    /// <returns>操作结果</returns>
    Task<bool> SendConfigUpdateAsync(Guid nodeId, string configData);

    /// <summary>
    /// 向节点发送软件更新命令
    /// </summary>
    /// <param name="nodeId">节点ID</param>
    /// <param name="updatePackageUrl">更新包URL</param>
    /// <returns>操作结果</returns>
    Task<bool> SendSoftwareUpdateAsync(Guid nodeId, string updatePackageUrl);

    #endregion
} 
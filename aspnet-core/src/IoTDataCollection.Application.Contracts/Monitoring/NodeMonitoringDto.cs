using System;
using System.Collections.Generic;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 节点心跳请求DTO
/// </summary>
public class NodeHeartbeatRequestDto
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public Guid NodeId { get; set; }

    /// <summary>
    /// 心跳时间
    /// </summary>
    public DateTime HeartbeatTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 性能数据
    /// </summary>
    public NodePerformanceDto? PerformanceData { get; set; }

    /// <summary>
    /// 设备统计数据
    /// </summary>
    public NodeDeviceStatsDto? DeviceStats { get; set; }
}

/// <summary>
/// 节点性能数据DTO
/// </summary>
public class NodePerformanceDto
{
    /// <summary>
    /// CPU使用率 (百分比)
    /// </summary>
    public decimal? CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率 (百分比)
    /// </summary>
    public decimal? MemoryUsage { get; set; }

    /// <summary>
    /// 磁盘使用率 (百分比)
    /// </summary>
    public decimal? DiskUsage { get; set; }

    /// <summary>
    /// 网络流量 (KB/s)
    /// </summary>
    public decimal? NetworkTraffic { get; set; }

    /// <summary>
    /// 数据采集速率 (点/秒)
    /// </summary>
    public decimal? DataCollectionRate { get; set; }

    /// <summary>
    /// 内存总量 (MB)
    /// </summary>
    public decimal? TotalMemory { get; set; }

    /// <summary>
    /// 磁盘总量 (GB)
    /// </summary>
    public decimal? TotalDisk { get; set; }

    /// <summary>
    /// CPU核心数
    /// </summary>
    public int? CpuCores { get; set; }

    /// <summary>
    /// 系统运行时间 (小时)
    /// </summary>
    public decimal? SystemUptime { get; set; }

    /// <summary>
    /// 进程数量
    /// </summary>
    public int? ProcessCount { get; set; }
}

/// <summary>
/// 节点设备统计DTO
/// </summary>
public class NodeDeviceStatsDto
{
    /// <summary>
    /// 管理的设备数量
    /// </summary>
    public int DeviceCount { get; set; }

    /// <summary>
    /// 在线设备数量
    /// </summary>
    public int OnlineDeviceCount { get; set; }

    /// <summary>
    /// 错误数据点数量
    /// </summary>
    public int ErrorDataCount { get; set; }

    /// <summary>
    /// 数据点总数
    /// </summary>
    public int TotalDataPointCount { get; set; }

    /// <summary>
    /// 采集成功的数据点数
    /// </summary>
    public int SuccessDataPointCount { get; set; }
}

/// <summary>
/// 批量节点状态更新请求DTO
/// </summary>
public class BatchNodeStatusUpdateDto
{
    /// <summary>
    /// 节点状态更新列表
    /// </summary>
    public List<NodeStatusUpdateDto> NodeStatusUpdates { get; set; } = new();
}

/// <summary>
/// 节点状态更新DTO
/// </summary>
public class NodeStatusUpdateDto
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public Guid NodeId { get; set; }

    /// <summary>
    /// 节点状态 - 0:离线 1:在线 2:故障 3:维护中
    /// </summary>
    public int NodeStatus { get; set; }

    /// <summary>
    /// 连接状态 - 0:断开 1:已连接 2:连接中
    /// </summary>
    public int ConnectionStatus { get; set; }

    /// <summary>
    /// 最后心跳时间
    /// </summary>
    public DateTime? LastHeartbeatTime { get; set; }

    /// <summary>
    /// 最后离线时间
    /// </summary>
    public DateTime? LastOfflineTime { get; set; }

    /// <summary>
    /// 性能数据
    /// </summary>
    public NodePerformanceDto? PerformanceData { get; set; }
}

/// <summary>
/// 节点监控配置更新DTO
/// </summary>
public class NodeMonitoringConfigDto
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public Guid NodeId { get; set; }

    /// <summary>
    /// 心跳间隔（秒）
    /// </summary>
    public int HeartbeatInterval { get; set; }

    /// <summary>
    /// 超时阈值（秒）
    /// </summary>
    public int TimeoutThreshold { get; set; }

    /// <summary>
    /// 是否启用监控
    /// </summary>
    public bool IsMonitoringEnabled { get; set; }

    /// <summary>
    /// 是否启用告警
    /// </summary>
    public bool IsAlertEnabled { get; set; }
}

/// <summary>
/// 节点健康检查响应DTO
/// </summary>
public class NodeHealthCheckResponseDto
{
    /// <summary>
    /// 节点ID
    /// </summary>
    public Guid NodeId { get; set; }

    /// <summary>
    /// 节点编码
    /// </summary>
    public string NodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 是否健康
    /// </summary>
    public bool IsHealthy { get; set; }

    /// <summary>
    /// 健康度评分（0-100）
    /// </summary>
    public decimal HealthScore { get; set; }

    /// <summary>
    /// 检查时间
    /// </summary>
    public DateTime CheckTime { get; set; } = DateTime.Now;

    /// <summary>
    /// 详细信息
    /// </summary>
    public List<string> Details { get; set; } = new();

    /// <summary>
    /// 告警信息
    /// </summary>
    public List<string> Alerts { get; set; } = new();

    /// <summary>
    /// 性能数据
    /// </summary>
    public NodePerformanceDto? PerformanceData { get; set; }
} 
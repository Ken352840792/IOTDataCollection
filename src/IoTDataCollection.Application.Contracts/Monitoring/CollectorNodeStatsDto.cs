using System.Collections.Generic;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点统计信息DTO
/// </summary>
public class CollectorNodeStatsDto
{
    /// <summary>
    /// 节点总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 在线节点数
    /// </summary>
    public int OnlineCount { get; set; }

    /// <summary>
    /// 离线节点数
    /// </summary>
    public int OfflineCount { get; set; }

    /// <summary>
    /// 故障节点数
    /// </summary>
    public int FaultCount { get; set; }

    /// <summary>
    /// 维护中节点数
    /// </summary>
    public int MaintenanceCount { get; set; }

    /// <summary>
    /// 超时节点数（心跳超时）
    /// </summary>
    public int TimeoutCount { get; set; }

    /// <summary>
    /// 启用监控的节点数
    /// </summary>
    public int MonitoringEnabledCount { get; set; }

    /// <summary>
    /// 启用告警的节点数
    /// </summary>
    public int AlertEnabledCount { get; set; }

    /// <summary>
    /// 按节点类型统计
    /// </summary>
    public Dictionary<string, int> CountByNodeType { get; set; } = new();

    /// <summary>
    /// 按操作系统统计
    /// </summary>
    public Dictionary<string, int> CountByOperatingSystem { get; set; } = new();

    /// <summary>
    /// 按软件版本统计
    /// </summary>
    public Dictionary<string, int> CountBySoftwareVersion { get; set; } = new();

    /// <summary>
    /// 最近24小时新增节点数
    /// </summary>
    public int RecentlyCreatedCount { get; set; }

    /// <summary>
    /// 总设备数（所有节点管理的设备总和）
    /// </summary>
    public int TotalDeviceCount { get; set; }

    /// <summary>
    /// 总在线设备数
    /// </summary>
    public int TotalOnlineDeviceCount { get; set; }

    /// <summary>
    /// 总错误数据点数
    /// </summary>
    public int TotalErrorDataCount { get; set; }

    /// <summary>
    /// 平均CPU使用率
    /// </summary>
    public decimal AverageCpuUsage { get; set; }

    /// <summary>
    /// 平均内存使用率
    /// </summary>
    public decimal AverageMemoryUsage { get; set; }

    /// <summary>
    /// 平均磁盘使用率
    /// </summary>
    public decimal AverageDiskUsage { get; set; }

    /// <summary>
    /// 总数据采集速率 (点/秒)
    /// </summary>
    public decimal TotalDataCollectionRate { get; set; }

    /// <summary>
    /// 节点在线率（百分比）
    /// </summary>
    public decimal NodeOnlineRate => TotalCount == 0 ? 0 : (decimal)OnlineCount / TotalCount * 100;

    /// <summary>
    /// 设备在线率（百分比）
    /// </summary>
    public decimal DeviceOnlineRate => TotalDeviceCount == 0 ? 0 : (decimal)TotalOnlineDeviceCount / TotalDeviceCount * 100;

    /// <summary>
    /// 监控启用率（百分比）
    /// </summary>
    public decimal MonitoringEnabledRate => TotalCount == 0 ? 0 : (decimal)MonitoringEnabledCount / TotalCount * 100;

    /// <summary>
    /// 系统整体健康度评分（0-100）
    /// </summary>
    public decimal OverallHealthScore { get; set; }

    /// <summary>
    /// 高健康度节点数（健康度>80）
    /// </summary>
    public int HighHealthNodeCount { get; set; }

    /// <summary>
    /// 中等健康度节点数（健康度50-80）
    /// </summary>
    public int MediumHealthNodeCount { get; set; }

    /// <summary>
    /// 低健康度节点数（健康度<50）
    /// </summary>
    public int LowHealthNodeCount { get; set; }

    /// <summary>
    /// 性能告警节点数（CPU>90% 或 内存>90% 或 磁盘>95%）
    /// </summary>
    public int PerformanceAlertNodeCount { get; set; }
} 
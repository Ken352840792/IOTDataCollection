using System;
using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点数据传输对象
/// </summary>
public class CollectorNodeDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 节点编码
    /// </summary>
    public string NodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 节点描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 节点类型
    /// </summary>
    public string? NodeType { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 管理端口
    /// </summary>
    public int ManagementPort { get; set; }

    /// <summary>
    /// 数据采集端口
    /// </summary>
    public int DataPort { get; set; }

    /// <summary>
    /// 操作系统信息
    /// </summary>
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// 软件版本
    /// </summary>
    public string? SoftwareVersion { get; set; }

    /// <summary>
    /// 硬件配置信息
    /// </summary>
    public string? HardwareInfo { get; set; }

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
    /// 心跳间隔（秒）
    /// </summary>
    public int HeartbeatInterval { get; set; }

    /// <summary>
    /// 超时阈值（秒）
    /// </summary>
    public int TimeoutThreshold { get; set; }

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
    /// 管理的设备数量
    /// </summary>
    public int DeviceCount { get; set; }

    /// <summary>
    /// 在线设备数量
    /// </summary>
    public int OnlineDeviceCount { get; set; }

    /// <summary>
    /// 数据采集速率 (点/秒)
    /// </summary>
    public decimal? DataCollectionRate { get; set; }

    /// <summary>
    /// 错误数据点数量
    /// </summary>
    public int ErrorDataCount { get; set; }

    /// <summary>
    /// 地理位置
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 是否启用监控
    /// </summary>
    public bool IsMonitoringEnabled { get; set; }

    /// <summary>
    /// 是否启用告警
    /// </summary>
    public bool IsAlertEnabled { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 节点状态文本
    /// </summary>
    public string NodeStatusText => NodeStatus switch
    {
        0 => "离线",
        1 => "在线",
        2 => "故障",
        3 => "维护中",
        _ => "未知"
    };

    /// <summary>
    /// 连接状态文本
    /// </summary>
    public string ConnectionStatusText => ConnectionStatus switch
    {
        0 => "断开",
        1 => "已连接",
        2 => "连接中",
        _ => "未知"
    };

    /// <summary>
    /// 节点类型文本
    /// </summary>
    public string NodeTypeText => NodeType switch
    {
        "Edge" => "边缘节点",
        "Gateway" => "网关节点",
        "Central" => "中心节点",
        _ => NodeType ?? "未知"
    };

    /// <summary>
    /// 设备在线率（百分比）
    /// </summary>
    public decimal DeviceOnlineRate => DeviceCount == 0 ? 0 : (decimal)OnlineDeviceCount / DeviceCount * 100;

    /// <summary>
    /// 是否超时（基于心跳时间和超时阈值）
    /// </summary>
    public bool IsTimeout => 
        LastHeartbeatTime.HasValue && 
        DateTime.Now.Subtract(LastHeartbeatTime.Value).TotalSeconds > TimeoutThreshold;

    /// <summary>
    /// 健康度评分（0-100，基于多个指标综合计算）
    /// </summary>
    public decimal HealthScore
    {
        get
        {
            if (NodeStatus != 1) return 0; // 离线或故障时健康度为0

            decimal score = 100;

            // CPU使用率影响（权重25%）
            if (CpuUsage.HasValue)
            {
                if (CpuUsage > 90) score -= 25;
                else if (CpuUsage > 80) score -= 15;
                else if (CpuUsage > 70) score -= 8;
            }

            // 内存使用率影响（权重25%）
            if (MemoryUsage.HasValue)
            {
                if (MemoryUsage > 90) score -= 25;
                else if (MemoryUsage > 80) score -= 15;
                else if (MemoryUsage > 70) score -= 8;
            }

            // 磁盘使用率影响（权重15%）
            if (DiskUsage.HasValue)
            {
                if (DiskUsage > 95) score -= 15;
                else if (DiskUsage > 85) score -= 8;
                else if (DiskUsage > 75) score -= 4;
            }

            // 设备在线率影响（权重20%）
            if (DeviceCount > 0)
            {
                var deviceRate = DeviceOnlineRate;
                if (deviceRate < 50) score -= 20;
                else if (deviceRate < 70) score -= 12;
                else if (deviceRate < 90) score -= 6;
            }

            // 错误数据影响（权重15%）
            if (ErrorDataCount > 0)
            {
                var errorRate = DeviceCount > 0 ? (decimal)ErrorDataCount / DeviceCount : 0;
                if (errorRate > 0.2m) score -= 15;
                else if (errorRate > 0.1m) score -= 8;
                else if (errorRate > 0.05m) score -= 4;
            }

            return Math.Max(0, Math.Min(100, score));
        }
    }
} 
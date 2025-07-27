using System;
using InfluxDB.Client.Core;

namespace IoTDataCollection.TimeSeriesData;

/// <summary>
/// 系统监控数据时序模型 - 用于InfluxDB存储
/// </summary>
[Measurement("system_monitoring")]
public class SystemMonitoringData
{
    /// <summary>
    /// 时间戳 - InfluxDB时间字段
    /// </summary>
    [Column(IsTimestamp = true)]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 采集节点编码 - Tag字段，标识监控来源
    /// </summary>
    [Column("node_code", IsTag = true)]
    public string NodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称 - Tag字段，便于查询
    /// </summary>
    [Column("node_name", IsTag = true)]
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 节点类型 - Tag字段，区分边缘节点、网关节点等
    /// </summary>
    [Column("node_type", IsTag = true)]
    public string NodeType { get; set; } = string.Empty;

    /// <summary>
    /// 主机名 - Tag字段，主机标识
    /// </summary>
    [Column("hostname", IsTag = true)]
    public string? Hostname { get; set; }

    /// <summary>
    /// CPU使用率 - Field字段，百分比
    /// </summary>
    [Column("cpu_usage")]
    public double CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率 - Field字段，百分比
    /// </summary>
    [Column("memory_usage")]
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 磁盘使用率 - Field字段，百分比
    /// </summary>
    [Column("disk_usage")]
    public double DiskUsage { get; set; }

    /// <summary>
    /// 网络接收流量 - Field字段，KB/s
    /// </summary>
    [Column("network_rx")]
    public double NetworkRx { get; set; }

    /// <summary>
    /// 网络发送流量 - Field字段，KB/s
    /// </summary>
    [Column("network_tx")]
    public double NetworkTx { get; set; }

    /// <summary>
    /// 总内存大小 - Field字段，MB
    /// </summary>
    [Column("total_memory")]
    public long TotalMemory { get; set; }

    /// <summary>
    /// 可用内存大小 - Field字段，MB
    /// </summary>
    [Column("available_memory")]
    public long AvailableMemory { get; set; }

    /// <summary>
    /// 总磁盘空间 - Field字段，GB
    /// </summary>
    [Column("total_disk")]
    public long TotalDisk { get; set; }

    /// <summary>
    /// 可用磁盘空间 - Field字段，GB
    /// </summary>
    [Column("available_disk")]
    public long AvailableDisk { get; set; }

    /// <summary>
    /// 在线设备数量 - Field字段
    /// </summary>
    [Column("online_devices")]
    public int OnlineDevices { get; set; }

    /// <summary>
    /// 总设备数量 - Field字段
    /// </summary>
    [Column("total_devices")]
    public int TotalDevices { get; set; }

    /// <summary>
    /// 数据采集速率 - Field字段，点/秒
    /// </summary>
    [Column("data_collection_rate")]
    public double DataCollectionRate { get; set; }

    /// <summary>
    /// 错误数据点数量 - Field字段
    /// </summary>
    [Column("error_data_count")]
    public int ErrorDataCount { get; set; }
} 
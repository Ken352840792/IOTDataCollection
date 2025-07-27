using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 采集节点实体 - 表名：T_COLLECTOR_NODES
/// </summary>
[Table("T_COLLECTOR_NODES")]
public class CollectorNode : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 节点编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_NODE_CODE")]
    public string F_NodeCode { get; set; }

    /// <summary>
    /// 节点名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_NODE_NAME")]
    public string F_NodeName { get; set; }

    /// <summary>
    /// 节点描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 节点类型 - Edge边缘节点、Gateway网关节点、Central中心节点
    /// </summary>
    [StringLength(64)]
    [Column("F_NODE_TYPE")]
    public string? F_NodeType { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [Required]
    [StringLength(45)]
    [Column("F_IP_ADDRESS")]
    public string F_IpAddress { get; set; }

    /// <summary>
    /// 管理端口
    /// </summary>
    [Column("F_MANAGEMENT_PORT")]
    public int F_ManagementPort { get; set; } = 8080;

    /// <summary>
    /// 数据采集端口
    /// </summary>
    [Column("F_DATA_PORT")]
    public int F_DataPort { get; set; } = 8081;

    /// <summary>
    /// 操作系统信息
    /// </summary>
    [StringLength(128)]
    [Column("F_OPERATING_SYSTEM")]
    public string? F_OperatingSystem { get; set; }

    /// <summary>
    /// 软件版本
    /// </summary>
    [StringLength(64)]
    [Column("F_SOFTWARE_VERSION")]
    public string? F_SoftwareVersion { get; set; }

    /// <summary>
    /// 硬件配置信息
    /// </summary>
    [StringLength(512)]
    [Column("F_HARDWARE_INFO")]
    public string? F_HardwareInfo { get; set; }

    /// <summary>
    /// 节点状态 - 0:离线 1:在线 2:故障 3:维护中
    /// </summary>
    [Column("F_NODE_STATUS")]
    public int F_NodeStatus { get; set; } = 0;

    /// <summary>
    /// 连接状态 - 0:断开 1:已连接 2:连接中
    /// </summary>
    [Column("F_CONNECTION_STATUS")]
    public int F_ConnectionStatus { get; set; } = 0;

    /// <summary>
    /// 最后心跳时间
    /// </summary>
    [Column("F_LAST_HEARTBEAT_TIME")]
    public DateTime? F_LastHeartbeatTime { get; set; }

    /// <summary>
    /// 最后离线时间
    /// </summary>
    [Column("F_LAST_OFFLINE_TIME")]
    public DateTime? F_LastOfflineTime { get; set; }

    /// <summary>
    /// 心跳间隔（秒）
    /// </summary>
    [Column("F_HEARTBEAT_INTERVAL")]
    public int F_HeartbeatInterval { get; set; } = 30;

    /// <summary>
    /// 超时阈值（秒）
    /// </summary>
    [Column("F_TIMEOUT_THRESHOLD")]
    public int F_TimeoutThreshold { get; set; } = 180;

    /// <summary>
    /// CPU使用率 (百分比)
    /// </summary>
    [Column("F_CPU_USAGE")]
    public decimal? F_CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率 (百分比)
    /// </summary>
    [Column("F_MEMORY_USAGE")]
    public decimal? F_MemoryUsage { get; set; }

    /// <summary>
    /// 磁盘使用率 (百分比)
    /// </summary>
    [Column("F_DISK_USAGE")]
    public decimal? F_DiskUsage { get; set; }

    /// <summary>
    /// 网络流量 (KB/s)
    /// </summary>
    [Column("F_NETWORK_TRAFFIC")]
    public decimal? F_NetworkTraffic { get; set; }

    /// <summary>
    /// 管理的设备数量
    /// </summary>
    [Column("F_DEVICE_COUNT")]
    public int F_DeviceCount { get; set; } = 0;

    /// <summary>
    /// 在线设备数量
    /// </summary>
    [Column("F_ONLINE_DEVICE_COUNT")]
    public int F_OnlineDeviceCount { get; set; } = 0;

    /// <summary>
    /// 数据采集速率 (点/秒)
    /// </summary>
    [Column("F_DATA_COLLECTION_RATE")]
    public decimal? F_DataCollectionRate { get; set; }

    /// <summary>
    /// 错误数据点数量
    /// </summary>
    [Column("F_ERROR_DATA_COUNT")]
    public int F_ErrorDataCount { get; set; } = 0;

    /// <summary>
    /// 地理位置 - 经纬度
    /// </summary>
    [StringLength(128)]
    [Column("F_LOCATION")]
    public string? F_Location { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [StringLength(128)]
    [Column("F_RESPONSIBLE_PERSON")]
    public string? F_ResponsiblePerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(32)]
    [Column("F_CONTACT_PHONE")]
    public string? F_ContactPhone { get; set; }

    /// <summary>
    /// 是否启用监控 - 0:禁用 1:启用
    /// </summary>
    [Column("F_IS_MONITORING_ENABLED")]
    public bool F_IsMonitoringEnabled { get; set; } = true;

    /// <summary>
    /// 是否启用告警 - 0:禁用 1:启用
    /// </summary>
    [Column("F_IS_ALERT_ENABLED")]
    public bool F_IsAlertEnabled { get; set; } = true;

    /// <summary>
    /// 排序号
    /// </summary>
    [Column("F_SORT_ORDER")]
    public int F_SortOrder { get; set; }

    #region 扩展字段 - 预留10个扩展字段
    [StringLength(500)]
    [Column("F_EXP_01")]
    public string? F_Exp_01 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_02")]
    public string? F_Exp_02 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_03")]
    public string? F_Exp_03 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_04")]
    public string? F_Exp_04 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_05")]
    public string? F_Exp_05 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_06")]
    public string? F_Exp_06 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_07")]
    public string? F_Exp_07 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_08")]
    public string? F_Exp_08 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_09")]
    public string? F_Exp_09 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_10")]
    public string? F_Exp_10 { get; set; }
    #endregion

    protected CollectorNode()
    {
        F_NodeCode = string.Empty;
        F_NodeName = string.Empty;
        F_IpAddress = string.Empty;
    }

    public CollectorNode(
        Guid id,
        string nodeCode,
        string nodeName,
        string ipAddress,
        string? nodeType = null) : base(id)
    {
        F_NodeCode = nodeCode;
        F_NodeName = nodeName;
        F_IpAddress = ipAddress;
        F_NodeType = nodeType;
        F_ManagementPort = 8080;
        F_DataPort = 8081;
        F_NodeStatus = 0;
        F_ConnectionStatus = 0;
        F_HeartbeatInterval = 30;
        F_TimeoutThreshold = 180;
        F_DeviceCount = 0;
        F_OnlineDeviceCount = 0;
        F_ErrorDataCount = 0;
        F_IsMonitoringEnabled = true;
        F_IsAlertEnabled = true;
        F_SortOrder = 0;
    }

    /// <summary>
    /// 更新心跳信息
    /// </summary>
    /// <param name="heartbeatTime">心跳时间</param>
    /// <param name="performanceData">性能数据</param>
    public void UpdateHeartbeat(DateTime heartbeatTime, NodePerformanceData? performanceData = null)
    {
        F_LastHeartbeatTime = heartbeatTime;
        F_ConnectionStatus = 1; // 已连接
        F_NodeStatus = 1; // 在线

        if (performanceData != null)
        {
            F_CpuUsage = performanceData.CpuUsage;
            F_MemoryUsage = performanceData.MemoryUsage;
            F_DiskUsage = performanceData.DiskUsage;
            F_NetworkTraffic = performanceData.NetworkTraffic;
            F_DataCollectionRate = performanceData.DataCollectionRate;
        }
    }

    /// <summary>
    /// 设置离线状态
    /// </summary>
    public void SetOffline()
    {
        F_NodeStatus = 0; // 离线
        F_ConnectionStatus = 0; // 断开
        F_LastOfflineTime = DateTime.Now;
    }

    /// <summary>
    /// 设置故障状态
    /// </summary>
    public void SetFault()
    {
        F_NodeStatus = 2; // 故障
        F_ConnectionStatus = 0; // 断开
    }

    /// <summary>
    /// 设置维护状态
    /// </summary>
    public void SetMaintenance()
    {
        F_NodeStatus = 3; // 维护中
        F_ConnectionStatus = 0; // 断开
    }

    /// <summary>
    /// 更新设备统计信息
    /// </summary>
    /// <param name="deviceCount">总设备数</param>
    /// <param name="onlineDeviceCount">在线设备数</param>
    /// <param name="errorDataCount">错误数据点数</param>
    public void UpdateDeviceStatistics(int deviceCount, int onlineDeviceCount, int errorDataCount)
    {
        F_DeviceCount = deviceCount;
        F_OnlineDeviceCount = onlineDeviceCount;
        F_ErrorDataCount = errorDataCount;
    }
}

/// <summary>
/// 节点性能数据
/// </summary>
public class NodePerformanceData
{
    public decimal? CpuUsage { get; set; }
    public decimal? MemoryUsage { get; set; }
    public decimal? DiskUsage { get; set; }
    public decimal? NetworkTraffic { get; set; }
    public decimal? DataCollectionRate { get; set; }
} 
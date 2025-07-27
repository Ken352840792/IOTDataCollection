using System.ComponentModel.DataAnnotations;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 创建采集节点数据传输对象
/// </summary>
public class CreateCollectorNodeDto
{
    /// <summary>
    /// 节点编码
    /// </summary>
    [Required(ErrorMessage = "节点编码不能为空")]
    [StringLength(64, ErrorMessage = "节点编码长度不能超过64个字符")]
    public string NodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 节点名称
    /// </summary>
    [Required(ErrorMessage = "节点名称不能为空")]
    [StringLength(256, ErrorMessage = "节点名称长度不能超过256个字符")]
    public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// 节点描述
    /// </summary>
    [StringLength(1000, ErrorMessage = "节点描述长度不能超过1000个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 节点类型 - Edge边缘节点、Gateway网关节点、Central中心节点
    /// </summary>
    [StringLength(64, ErrorMessage = "节点类型长度不能超过64个字符")]
    public string? NodeType { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [Required(ErrorMessage = "IP地址不能为空")]
    [StringLength(45, ErrorMessage = "IP地址长度不能超过45个字符")]
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 管理端口
    /// </summary>
    [Range(1, 65535, ErrorMessage = "管理端口必须在1-65535之间")]
    public int ManagementPort { get; set; } = 8080;

    /// <summary>
    /// 数据采集端口
    /// </summary>
    [Range(1, 65535, ErrorMessage = "数据采集端口必须在1-65535之间")]
    public int DataPort { get; set; } = 8081;

    /// <summary>
    /// 操作系统信息
    /// </summary>
    [StringLength(128, ErrorMessage = "操作系统信息长度不能超过128个字符")]
    public string? OperatingSystem { get; set; }

    /// <summary>
    /// 软件版本
    /// </summary>
    [StringLength(64, ErrorMessage = "软件版本长度不能超过64个字符")]
    public string? SoftwareVersion { get; set; }

    /// <summary>
    /// 硬件配置信息
    /// </summary>
    [StringLength(512, ErrorMessage = "硬件配置信息长度不能超过512个字符")]
    public string? HardwareInfo { get; set; }

    /// <summary>
    /// 心跳间隔（秒）
    /// </summary>
    [Range(10, 3600, ErrorMessage = "心跳间隔必须在10秒到1小时之间")]
    public int HeartbeatInterval { get; set; } = 30;

    /// <summary>
    /// 超时阈值（秒）
    /// </summary>
    [Range(60, 7200, ErrorMessage = "超时阈值必须在1分钟到2小时之间")]
    public int TimeoutThreshold { get; set; } = 180;

    /// <summary>
    /// 地理位置
    /// </summary>
    [StringLength(128, ErrorMessage = "地理位置长度不能超过128个字符")]
    public string? Location { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [StringLength(128, ErrorMessage = "负责人长度不能超过128个字符")]
    public string? ResponsiblePerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(32, ErrorMessage = "联系电话长度不能超过32个字符")]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 是否启用监控
    /// </summary>
    public bool IsMonitoringEnabled { get; set; } = true;

    /// <summary>
    /// 是否启用告警
    /// </summary>
    public bool IsAlertEnabled { get; set; } = true;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
} 
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备实体 - 表名：T_DEVICES
/// </summary>
[Table("T_DEVICES")]
public class Device : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 站点ID - 外键关联到T_SITES
    /// </summary>
    [Required]
    [Column("F_SITE_ID")]
    public Guid F_SiteId { get; set; }

    /// <summary>
    /// 设备编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_DEVICE_CODE")]
    public string F_DeviceCode { get; set; }

    /// <summary>
    /// 设备名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_DEVICE_NAME")]
    public string F_DeviceName { get; set; }

    /// <summary>
    /// 设备类型 - PLC、传感器、仪表等
    /// </summary>
    [StringLength(64)]
    [Column("F_DEVICE_TYPE")]
    public string? F_DeviceType { get; set; }

    /// <summary>
    /// 设备型号
    /// </summary>
    [StringLength(128)]
    [Column("F_DEVICE_MODEL")]
    public string? F_DeviceModel { get; set; }

    /// <summary>
    /// 设备制造商
    /// </summary>
    [StringLength(128)]
    [Column("F_MANUFACTURER")]
    public string? F_Manufacturer { get; set; }

    /// <summary>
    /// 通信协议 - Modbus、OPC UA、TCP等
    /// </summary>
    [StringLength(64)]
    [Column("F_COMMUNICATION_PROTOCOL")]
    public string? F_CommunicationProtocol { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [StringLength(45)]
    [Column("F_IP_ADDRESS")]
    public string? F_IpAddress { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    [Column("F_PORT")]
    public int? F_Port { get; set; }

    /// <summary>
    /// 从站地址/设备地址
    /// </summary>
    [Column("F_SLAVE_ADDRESS")]
    public int? F_SlaveAddress { get; set; }

    /// <summary>
    /// 采集间隔（毫秒）
    /// </summary>
    [Column("F_COLLECTION_INTERVAL")]
    public int F_CollectionInterval { get; set; } = 1000;

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    [Column("F_TIMEOUT")]
    public int F_Timeout { get; set; } = 5000;

    /// <summary>
    /// 重试次数
    /// </summary>
    [Column("F_RETRY_COUNT")]
    public int F_RetryCount { get; set; } = 3;

    /// <summary>
    /// 设备位置描述
    /// </summary>
    [StringLength(512)]
    [Column("F_LOCATION")]
    public string? F_Location { get; set; }

    /// <summary>
    /// 设备描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 连接状态 - 0:离线 1:在线 2:故障
    /// </summary>
    [Column("F_CONNECTION_STATUS")]
    public int F_ConnectionStatus { get; set; } = 0;

    /// <summary>
    /// 最后通信时间
    /// </summary>
    [Column("F_LAST_COMMUNICATION_TIME")]
    public DateTime? F_LastCommunicationTime { get; set; }

    /// <summary>
    /// 是否启用采集 - 0:禁用 1:启用
    /// </summary>
    [Column("F_IS_COLLECTION_ENABLED")]
    public bool F_IsCollectionEnabled { get; set; } = true;

    /// <summary>
    /// 状态 - 0:禁用 1:启用
    /// </summary>
    [Column("F_STATUS")]
    public int F_Status { get; set; } = 1;

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

    protected Device()
    {
        F_DeviceCode = string.Empty;
        F_DeviceName = string.Empty;
    }

    public Device(
        Guid id,
        Guid siteId,
        string deviceCode,
        string deviceName,
        string? deviceType = null,
        string? communicationProtocol = null) : base(id)
    {
        F_SiteId = siteId;
        F_DeviceCode = deviceCode;
        F_DeviceName = deviceName;
        F_DeviceType = deviceType;
        F_CommunicationProtocol = communicationProtocol;
        F_CollectionInterval = 1000;
        F_Timeout = 5000;
        F_RetryCount = 3;
        F_ConnectionStatus = 0;
        F_IsCollectionEnabled = true;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
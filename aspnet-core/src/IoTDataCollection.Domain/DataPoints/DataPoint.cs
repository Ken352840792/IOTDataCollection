using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.DataPoints;

/// <summary>
/// 数据点实体 - 表名：T_DATA_POINTS
/// </summary>
[Table("T_DATA_POINTS")]
public class DataPoint : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 设备ID - 外键关联到T_DEVICES
    /// </summary>
    [Required]
    [Column("F_DEVICE_ID")]
    public Guid F_DeviceId { get; set; }

    /// <summary>
    /// 数据点编码 - 在设备内唯一
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_POINT_CODE")]
    public string F_PointCode { get; set; }

    /// <summary>
    /// 数据点名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_POINT_NAME")]
    public string F_PointName { get; set; }

    /// <summary>
    /// 数据类型 - Int16、Int32、Float、Double、Bool、String等
    /// </summary>
    [Required]
    [StringLength(32)]
    [Column("F_DATA_TYPE")]
    public string F_DataType { get; set; }

    /// <summary>
    /// 寄存器地址 - 支持各种协议的地址格式，如Modbus的"40001"、Siemens S7的"DB1.DBD0"等
    /// </summary>
    [StringLength(100)]
    [Column("F_REGISTER_ADDRESS")]
    public string? F_RegisterAddress { get; set; }

    /// <summary>
    /// 寄存器类型 - 保持寄存器、输入寄存器、线圈、离散输入
    /// </summary>
    [StringLength(32)]
    [Column("F_REGISTER_TYPE")]
    public string? F_RegisterType { get; set; }

    /// <summary>
    /// 数据长度 - 字节数或位数
    /// </summary>
    [Column("F_DATA_LENGTH")]
    public int? F_DataLength { get; set; }

    /// <summary>
    /// 是否有符号
    /// </summary>
    [Column("F_IS_SIGNED")]
    public bool F_IsSigned { get; set; } = false;

    /// <summary>
    /// 字节序 - Big、Little
    /// </summary>
    [StringLength(16)]
    [Column("F_BYTE_ORDER")]
    public string? F_ByteOrder { get; set; }

    /// <summary>
    /// 缩放比例
    /// </summary>
    [Column("F_SCALE_FACTOR")]
    public double F_ScaleFactor { get; set; } = 1.0;

    /// <summary>
    /// 偏移量
    /// </summary>
    [Column("F_OFFSET")]
    public double F_Offset { get; set; } = 0.0;

    /// <summary>
    /// 单位
    /// </summary>
    [StringLength(32)]
    [Column("F_UNIT")]
    public string? F_Unit { get; set; }

    /// <summary>
    /// 数据点描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 最小值 - 用于数据验证
    /// </summary>
    [Column("F_MIN_VALUE")]
    public double? F_MinValue { get; set; }

    /// <summary>
    /// 最大值 - 用于数据验证
    /// </summary>
    [Column("F_MAX_VALUE")]
    public double? F_MaxValue { get; set; }

    /// <summary>
    /// 默认值
    /// </summary>
    [StringLength(100)]
    [Column("F_DEFAULT_VALUE")]
    public string? F_DefaultValue { get; set; }

    /// <summary>
    /// 是否只读 - true:只读 false:可写
    /// </summary>
    [Column("F_IS_READ_ONLY")]
    public bool F_IsReadOnly { get; set; } = true;

    /// <summary>
    /// 是否启用采集 - 0:禁用 1:启用
    /// </summary>
    [Column("F_IS_COLLECTION_ENABLED")]
    public bool F_IsCollectionEnabled { get; set; } = true;

    /// <summary>
    /// 采集优先级 - 1:高 2:中 3:低
    /// </summary>
    [Column("F_COLLECTION_PRIORITY")]
    public int F_CollectionPriority { get; set; } = 2;

    /// <summary>
    /// 最后采集时间
    /// </summary>
    [Column("F_LAST_COLLECTION_TIME")]
    public DateTime? F_LastCollectionTime { get; set; }

    /// <summary>
    /// 最后采集值
    /// </summary>
    [StringLength(500)]
    [Column("F_LAST_VALUE")]
    public string? F_LastValue { get; set; }

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

    protected DataPoint()
    {
        F_PointCode = string.Empty;
        F_PointName = string.Empty;
        F_DataType = string.Empty;
    }

    public DataPoint(
        Guid id,
        Guid deviceId,
        string pointCode,
        string pointName,
        string dataType,
        string? registerAddress = null,
        string? registerType = null) : base(id)
    {
        F_DeviceId = deviceId;
        F_PointCode = pointCode;
        F_PointName = pointName;
        F_DataType = dataType;
        F_RegisterAddress = registerAddress;
        F_RegisterType = registerType;
        F_IsSigned = false;
        F_ScaleFactor = 1.0;
        F_Offset = 0.0;
        F_IsReadOnly = true;
        F_IsCollectionEnabled = true;
        F_CollectionPriority = 2;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
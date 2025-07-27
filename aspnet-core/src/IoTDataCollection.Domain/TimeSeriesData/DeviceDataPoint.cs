using System;
using InfluxDB.Client.Core;

namespace IoTDataCollection.TimeSeriesData;

/// <summary>
/// 设备数据点时序数据模型 - 用于InfluxDB存储
/// </summary>
[Measurement("device_data")]
public class DeviceDataPoint
{
    /// <summary>
    /// 时间戳 - InfluxDB时间字段
    /// </summary>
    [Column(IsTimestamp = true)]
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 设备编码 - Tag字段，用于索引查询
    /// </summary>
    [Column("device_code", IsTag = true)]
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 站点编码 - Tag字段，用于按站点查询
    /// </summary>
    [Column("site_code", IsTag = true)]
    public string SiteCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据点编码 - Tag字段，标识具体的数据点
    /// </summary>
    [Column("point_code", IsTag = true)]
    public string PointCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据点名称 - Tag字段，便于查询
    /// </summary>
    [Column("point_name", IsTag = true)]
    public string PointName { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型 - Tag字段，标识数据类型
    /// </summary>
    [Column("data_type", IsTag = true)]
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 数值类型的值 - Field字段，存储数值
    /// </summary>
    [Column("numeric_value")]
    public double? NumericValue { get; set; }

    /// <summary>
    /// 字符串类型的值 - Field字段，存储文本
    /// </summary>
    [Column("string_value")]
    public string? StringValue { get; set; }

    /// <summary>
    /// 布尔类型的值 - Field字段，存储布尔值
    /// </summary>
    [Column("boolean_value")]
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// 原始值 - Field字段，存储原始采集值
    /// </summary>
    [Column("raw_value")]
    public string? RawValue { get; set; }

    /// <summary>
    /// 质量标记 - Field字段，数据质量标识
    /// </summary>
    [Column("quality")]
    public int Quality { get; set; } = 192; // 192表示数据良好

    /// <summary>
    /// 单位 - Tag字段，数据单位
    /// </summary>
    [Column("unit", IsTag = true)]
    public string? Unit { get; set; }

    /// <summary>
    /// 采集节点编码 - Tag字段，标识采集来源
    /// </summary>
    [Column("collector_node", IsTag = true)]
    public string? CollectorNode { get; set; }
} 
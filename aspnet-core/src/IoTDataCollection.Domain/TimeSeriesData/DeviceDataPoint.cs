using System;
using InfluxDB.Client.Core;

namespace IoTDataCollection.TimeSeriesData;

/// <summary>
/// 设备数据点时序数据模型 - 用于InfluxDB存储
/// 每个设备一个独立的Measurement
/// </summary>
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
    /// 数据单位 - Tag字段，数据单位
    /// </summary>
    [Column("unit", IsTag = true)]
    public string? Unit { get; set; }

    /// <summary>
    /// 采集节点编码 - Tag字段，标识采集来源
    /// </summary>
    [Column("collector_node", IsTag = true)]
    public string? CollectorNode { get; set; }

    /// <summary>
    /// 原始值 - Field字段，存储从设备直接采集的原始值
    /// </summary>
    [Column("raw_value")]
    public string RawValue { get; set; } = string.Empty;

    /// <summary>
    /// 计算后的值 - Field字段，存储经过计算处理后的值
    /// </summary>
    [Column("calculated_value")]
    public string CalculatedValue { get; set; } = string.Empty;

    /// <summary>
    /// 质量标记 - Field字段，数据质量标识
    /// </summary>
    [Column("quality")]
    public int Quality { get; set; } = 192; // 192表示数据良好

    /// <summary>
    /// 处理时间 - Field字段，数据处理时间戳
    /// </summary>
    [Column("processed_time")]
    public DateTime ProcessedTime { get; set; }

    /// <summary>
    /// 获取Measurement名称 - 基于设备编码动态生成
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <returns>Measurement名称</returns>
    public static string GetMeasurementName(string deviceCode)
    {
        // 确保设备编码符合InfluxDB命名规范
        // 替换特殊字符为下划线，确保名称合法
        var measurementName = deviceCode
            .Replace("-", "_")
            .Replace(".", "_")
            .Replace(" ", "_")
            .ToLowerInvariant();
        
        // 添加前缀以区分设备数据
        return $"device_{measurementName}";
    }

    /// <summary>
    /// 获取当前实例的Measurement名称
    /// </summary>
    /// <returns>Measurement名称</returns>
    public string GetMeasurementName()
    {
        return GetMeasurementName(DeviceCode);
    }
} 
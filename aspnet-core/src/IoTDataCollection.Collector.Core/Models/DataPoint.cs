using System;

namespace IoTDataCollection.Collector.Core.Models;

/// <summary>
/// 数据点模型
/// </summary>
public class DataPoint
{
    /// <summary>
    /// 数据点ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据点编码
    /// </summary>
    public string PointCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据点名称
    /// </summary>
    public string PointName { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型
    /// </summary>
    public DataType DataType { get; set; }

    /// <summary>
    /// 数值
    /// </summary>
    public double? NumericValue { get; set; }

    /// <summary>
    /// 字符串值
    /// </summary>
    public string? StringValue { get; set; }

    /// <summary>
    /// 布尔值
    /// </summary>
    public bool? BooleanValue { get; set; }

    /// <summary>
    /// 原始值
    /// </summary>
    public string? RawValue { get; set; }

    /// <summary>
    /// 质量标记
    /// </summary>
    public int Quality { get; set; } = 192;

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}

/// <summary>
/// 数据类型枚举
/// </summary>
public enum DataType
{
    /// <summary>
    /// 布尔型
    /// </summary>
    Bool,

    /// <summary>
    /// 16位整数
    /// </summary>
    Int16,

    /// <summary>
    /// 32位整数
    /// </summary>
    Int32,

    /// <summary>
    /// 浮点数
    /// </summary>
    Float,

    /// <summary>
    /// 字符串
    /// </summary>
    String
} 
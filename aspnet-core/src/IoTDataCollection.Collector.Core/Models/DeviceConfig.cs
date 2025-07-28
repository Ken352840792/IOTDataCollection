using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace IoTDataCollection.Collector.Core.Models;

/// <summary>
/// 设备配置模型
/// </summary>
public class DeviceConfig
{
    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    public DeviceType DeviceType { get; set; }

    /// <summary>
    /// 连接配置
    /// </summary>
    public ConnectionConfig ConnectionConfig { get; set; } = new();

    /// <summary>
    /// 数据点配置列表
    /// </summary>
    public List<DataPointConfig> DataPoints { get; set; } = new();

    /// <summary>
    /// 采集间隔（秒）
    /// </summary>
    public int SamplingInterval { get; set; } = 1;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备类型枚举
/// </summary>
public enum DeviceType
{
    /// <summary>
    /// PLC设备
    /// </summary>
    PLC,

    /// <summary>
    /// Modbus设备
    /// </summary>
    Modbus,

    /// <summary>
    /// OPC-UA设备
    /// </summary>
    OPCUA,

    /// <summary>
    /// Socket设备
    /// </summary>
    Socket
}

/// <summary>
/// 连接配置
/// </summary>
public class ConnectionConfig
{
    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 连接超时时间（秒）
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30;

    /// <summary>
    /// 读取超时时间（秒）
    /// </summary>
    public int ReadTimeout { get; set; } = 10;

    /// <summary>
    /// 写入超时时间（秒）
    /// </summary>
    public int WriteTimeout { get; set; } = 10;

    /// <summary>
    /// 额外配置参数（JSON格式）
    /// </summary>
    public string? ExtraConfig { get; set; }
}

/// <summary>
/// 数据点配置
/// </summary>
public class DataPointConfig
{
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
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 缩放因子
    /// </summary>
    public double ScaleFactor { get; set; } = 1.0;

    /// <summary>
    /// 偏移量
    /// </summary>
    public double Offset { get; set; } = 0.0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
} 
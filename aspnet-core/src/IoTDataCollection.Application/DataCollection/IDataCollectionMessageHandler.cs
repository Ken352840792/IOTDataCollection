using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// 数据采集消息处理服务接口
/// </summary>
public interface IDataCollectionMessageHandler
{
    /// <summary>
    /// 处理设备数据采集消息
    /// </summary>
    /// <param name="message">采集数据消息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理结果</returns>
    Task<MessageProcessingResult> ProcessDeviceDataMessageAsync(DeviceDataMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 处理采集端状态消息
    /// </summary>
    /// <param name="message">状态消息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理结果</returns>
    Task<MessageProcessingResult> ProcessCollectorStatusMessageAsync(CollectorStatusMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// 处理设备心跳消息
    /// </summary>
    /// <param name="message">心跳消息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>处理结果</returns>
    Task<MessageProcessingResult> ProcessDeviceHeartbeatMessageAsync(DeviceHeartbeatMessage message, CancellationToken cancellationToken = default);
}

/// <summary>
/// 消息处理结果
/// </summary>
public class MessageProcessingResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 处理的数据点数量
    /// </summary>
    public int ProcessedDataPoints { get; set; }

    /// <summary>
    /// 处理时间（毫秒）
    /// </summary>
    public long ProcessingTimeMs { get; set; }
}

/// <summary>
/// 设备数据采集消息
/// </summary>
public class DeviceDataMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// 采集端节点标识
    /// </summary>
    public string CollectorNode { get; set; } = string.Empty;

    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 数据点列表
    /// </summary>
    public List<DataPointMessage> DataPoints { get; set; } = new();

    /// <summary>
    /// 元数据
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();
}

/// <summary>
/// 数据点消息
/// </summary>
public class DataPointMessage
{
    /// <summary>
    /// 数据点编码
    /// </summary>
    public string PointCode { get; set; } = string.Empty;

    /// <summary>
    /// 通用值字段 - 支持所有数据类型
    /// </summary>
    public object? Value { get; set; }

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
    /// 数据质量
    /// </summary>
    public int Quality { get; set; } = 192;

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }
}

/// <summary>
/// 采集端状态消息
/// </summary>
public class CollectorStatusMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public string MessageId { get; set; } = string.Empty;

    /// <summary>
    /// 采集端节点标识
    /// </summary>
    public string CollectorNode { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 状态类型
    /// </summary>
    public string StatusType { get; set; } = string.Empty; // Online, Offline, Error, Warning

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusDescription { get; set; } = string.Empty;

    /// <summary>
    /// 设备连接状态
    /// </summary>
    public Dictionary<string, bool> DeviceConnections { get; set; } = new();

    /// <summary>
    /// 系统资源信息
    /// </summary>
    public SystemResourceInfo? SystemResources { get; set; }
}

/// <summary>
/// 系统资源信息
/// </summary>
public class SystemResourceInfo
{
    /// <summary>
    /// CPU使用率
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 磁盘使用率
    /// </summary>
    public double DiskUsage { get; set; }

    /// <summary>
    /// 网络连接状态
    /// </summary>
    public bool NetworkConnected { get; set; }
}

/// <summary>
/// 设备心跳消息
/// </summary>
public class DeviceHeartbeatMessage
{
    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备状态
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }
} 
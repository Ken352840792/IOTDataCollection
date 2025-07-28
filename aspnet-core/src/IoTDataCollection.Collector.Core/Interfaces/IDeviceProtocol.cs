using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 设备协议接口
/// </summary>
public interface IDeviceProtocol
{
    /// <summary>
    /// 协议名称
    /// </summary>
    string ProtocolName { get; }

    /// <summary>
    /// 连接设备
    /// </summary>
    /// <param name="config">设备配置</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接结果</returns>
    Task<bool> ConnectAsync(DeviceConfig config, CancellationToken cancellationToken = default);

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>断开结果</returns>
    Task<bool> DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取数据
    /// </summary>
    /// <param name="addresses">地址列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点列表</returns>
    Task<List<DataPoint>> ReadDataAsync(string[] addresses, CancellationToken cancellationToken = default);

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="address">地址</param>
    /// <param name="value">值</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>写入结果</returns>
    Task<bool> WriteDataAsync(string address, object value, CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接状态</returns>
    Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取连接状态
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 设备状态变化事件
    /// </summary>
    event EventHandler<DeviceStatusChangedEventArgs> StatusChanged;
}

/// <summary>
/// 设备状态变化事件参数
/// </summary>
public class DeviceStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备状态
    /// </summary>
    public DeviceStatus Status { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// 设备状态枚举
/// </summary>
public enum DeviceStatus
{
    /// <summary>
    /// 离线
    /// </summary>
    Offline,

    /// <summary>
    /// 在线
    /// </summary>
    Online,

    /// <summary>
    /// 错误
    /// </summary>
    Error,

    /// <summary>
    /// 维护中
    /// </summary>
    Maintenance
} 
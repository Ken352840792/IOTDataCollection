using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 通信服务接口
/// </summary>
public interface ICommunicationService
{
    /// <summary>
    /// 连接状态
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 连接到MQTT服务器
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接结果</returns>
    Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 断开连接
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>断开结果</returns>
    Task<bool> DisconnectAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布消息
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="payload">消息内容</param>
    /// <param name="qos">服务质量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发布结果</returns>
    Task<bool> PublishAsync(string topic, object payload, int qos = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 订阅主题
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="handler">消息处理函数</param>
    /// <param name="qos">服务质量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>订阅结果</returns>
    Task<bool> SubscribeAsync(string topic, Func<string, Task> handler, int qos = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="topic">主题</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>取消订阅结果</returns>
    Task<bool> UnsubscribeAsync(string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送设备数据
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="dataPoints">数据点列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发送结果</returns>
    Task<bool> SendDeviceDataAsync(string deviceCode, List<DataPoint> dataPoints, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送心跳消息
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="status">设备状态</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发送结果</returns>
    Task<bool> SendHeartbeatAsync(string deviceCode, DeviceStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// 连接状态变化事件
    /// </summary>
    event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

    /// <summary>
    /// 消息接收事件
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;
}

/// <summary>
/// 连接状态变化事件参数
/// </summary>
public class ConnectionStatusChangedEventArgs : EventArgs
{
    /// <summary>
    /// 是否已连接
    /// </summary>
    public bool IsConnected { get; set; }

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
/// 消息接收事件参数
/// </summary>
public class MessageReceivedEventArgs : EventArgs
{
    /// <summary>
    /// 主题
    /// </summary>
    public string Topic { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容
    /// </summary>
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// 服务质量
    /// </summary>
    public int QoS { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 
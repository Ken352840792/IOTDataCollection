using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using System.Linq;

namespace IoTDataCollection.Collector.Communication;

/// <summary>
/// MQTT通信服务
/// </summary>
public class MqttCommunicationService : ICommunicationService, IDisposable
{
    private readonly ILogger<MqttCommunicationService> _logger;
    private readonly MqttConfiguration _config;
    private readonly IMqttClient _mqttClient;
    private readonly Dictionary<string, Func<string, Task>> _messageHandlers;
    private readonly Timer _reconnectTimer;
    private readonly object _lockObject = new object();
    private bool _isDisposed = false;
    private bool _isReconnecting = false;
    private int _reconnectAttempts = 0;
    private readonly int _maxReconnectAttempts = 10;

    public MqttCommunicationService(
        ILogger<MqttCommunicationService> logger,
        IOptions<MqttConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
        _messageHandlers = new Dictionary<string, Func<string, Task>>();

        // 创建MQTT客户端
        var mqttFactory = new MqttFactory();
        _mqttClient = mqttFactory.CreateMqttClient();

        // 创建重连定时器
        _reconnectTimer = new Timer(AttemptReconnect, null, Timeout.Infinite, Timeout.Infinite);

        // 配置客户端事件
        ConfigureClientEvents();
    }

    /// <summary>
    /// 连接状态
    /// </summary>
    public bool IsConnected => _mqttClient.IsConnected;

    /// <summary>
    /// 连接状态变化事件
    /// </summary>
    public event EventHandler<ConnectionStatusChangedEventArgs>? ConnectionStatusChanged;

    /// <summary>
    /// 消息接收事件
    /// </summary>
    public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

    /// <summary>
    /// 连接到MQTT服务器
    /// </summary>
    public async Task<bool> ConnectAsync(CancellationToken cancellationToken = default)
    {
        lock (_lockObject)
        {
            if (_isDisposed)
            {
                return false;
            }
        }

        try
        {
            _logger.LogInformation("正在连接到MQTT服务器: {Server}:{Port}", _config.Server, _config.Port);

            // 创建客户端选项
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(_config.Server, _config.Port)
                .WithClientId(_config.ClientId)
                .WithCredentials(_config.Username, _config.Password)
                .WithCleanSession()
                .WithKeepAlivePeriod(TimeSpan.FromSeconds(_config.KeepAliveSeconds))
                .Build();

            // 启动客户端
            await _mqttClient.ConnectAsync(options, cancellationToken);

            _logger.LogInformation("MQTT客户端连接成功");
            _reconnectAttempts = 0; // 重置重连计数
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT连接失败");
            return false;
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("正在断开MQTT连接");

            // 停止重连定时器
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);

            await _mqttClient.DisconnectAsync();
            _logger.LogInformation("MQTT连接已断开");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "断开MQTT连接失败");
            return false;
        }
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    public async Task<bool> PublishAsync(string topic, object payload, int qos = 1, CancellationToken cancellationToken = default)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogWarning("MQTT未连接，无法发布消息: {Topic}", topic);
            return false;
        }

        try
        {
            var jsonPayload = JsonConvert.SerializeObject(payload);
            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(jsonPayload)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(false)
                .Build();

            await _mqttClient.PublishAsync(message, cancellationToken);

            _logger.LogDebug("MQTT消息发布成功: {Topic}, QoS: {QoS}", topic, qos);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT消息发布失败: {Topic}", topic);
            return false;
        }
    }

    /// <summary>
    /// 订阅主题
    /// </summary>
    public async Task<bool> SubscribeAsync(string topic, Func<string, Task> handler, int qos = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT未连接，无法订阅主题: {Topic}", topic);
                return false;
            }

            var subscribeOptions = new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build();

            await _mqttClient.SubscribeAsync(subscribeOptions, cancellationToken);

            _messageHandlers[topic] = handler;

            _logger.LogInformation("MQTT主题订阅成功: {Topic}, QoS: {QoS}", topic, qos);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT主题订阅失败: {Topic}", topic);
            return false;
        }
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    public async Task<bool> UnsubscribeAsync(string topic, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT未连接，无法取消订阅: {Topic}", topic);
                return false;
            }

            await _mqttClient.UnsubscribeAsync(topic, cancellationToken);

            _messageHandlers.Remove(topic);

            _logger.LogInformation("MQTT主题取消订阅成功: {Topic}", topic);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT主题取消订阅失败: {Topic}", topic);
            return false;
        }
    }

    /// <summary>
    /// 发送设备数据
    /// </summary>
    public async Task<bool> SendDeviceDataAsync(string deviceCode, List<DataPoint> dataPoints, CancellationToken cancellationToken = default)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogWarning("MQTT未连接，设备数据将缓存到本地: {DeviceCode}", deviceCode);
            return false;
        }

        try
        {
            var topic = $"iot/device/{deviceCode}/data";
            var payload = new
            {
                DeviceCode = deviceCode,
                Timestamp = DateTime.UtcNow,
                DataPoints = dataPoints.Select(dp => new
                {
                    PointCode = dp.PointCode,
                    Value = GetDataPointValue(dp),
                    Quality = dp.Quality,
                    Unit = dp.Unit,
                    Timestamp = dp.Timestamp
                }).ToList()
            };

            return await PublishAsync(topic, payload, 1, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送设备数据失败: {DeviceCode}", deviceCode);
            return false;
        }
    }

    /// <summary>
    /// 发送心跳消息
    /// </summary>
    public async Task<bool> SendHeartbeatAsync(string deviceCode, DeviceStatus status, CancellationToken cancellationToken = default)
    {
        if (!_mqttClient.IsConnected)
        {
            _logger.LogDebug("MQTT未连接，跳过心跳发送: {DeviceCode}", deviceCode);
            return false;
        }

        try
        {
            var topic = $"iot/device/{deviceCode}/heartbeat";
            var payload = new
            {
                DeviceCode = deviceCode,
                Status = status.ToString(),
                Timestamp = DateTime.UtcNow
            };

            return await PublishAsync(topic, payload, 0, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送心跳消息失败: {DeviceCode}", deviceCode);
            return false;
        }
    }

    /// <summary>
    /// 配置客户端事件
    /// </summary>
    private void ConfigureClientEvents()
    {
        // 连接状态变化事件
        _mqttClient.ConnectedAsync += async args =>
        {
            _logger.LogInformation("MQTT客户端已连接");
            _reconnectAttempts = 0; // 重置重连计数
            
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
            {
                IsConnected = true,
                Timestamp = DateTime.UtcNow
            });
            await Task.CompletedTask;
        };

        _mqttClient.DisconnectedAsync += async args =>
        {
            _logger.LogWarning("MQTT客户端已断开连接: {Reason}", args.Reason);
            
            ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedEventArgs
            {
                IsConnected = false,
                ErrorMessage = args.Reason.ToString(),
                Timestamp = DateTime.UtcNow
            });

            // 启动自动重连
            if (!_isDisposed && !_isReconnecting)
            {
                StartReconnectTimer();
            }

            await Task.CompletedTask;
        };

        // 消息接收事件
        _mqttClient.ApplicationMessageReceivedAsync += async args =>
        {
            try
            {
                var topic = args.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
                var qos = (int)args.ApplicationMessage.QualityOfServiceLevel;

                _logger.LogDebug("收到MQTT消息: {Topic}, QoS: {QoS}", topic, qos);

                // 触发消息接收事件
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs
                {
                    Topic = topic,
                    Payload = payload,
                    QoS = qos,
                    Timestamp = DateTime.UtcNow
                });

                // 查找并执行消息处理器
                foreach (var handler in _messageHandlers)
                {
                    if (IsTopicMatch(topic, handler.Key))
                    {
                        await handler.Value(payload);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "处理MQTT消息失败");
            }
        };
    }

    /// <summary>
    /// 启动重连定时器
    /// </summary>
    private void StartReconnectTimer()
    {
        if (_isDisposed || _isReconnecting)
        {
            return;
        }

        _isReconnecting = true;
        var delay = Math.Min(_config.ReconnectDelaySeconds * (1 << _reconnectAttempts), 300); // 指数退避，最大5分钟
        
        _logger.LogInformation("MQTT将在 {Delay} 秒后尝试重连 (第 {Attempt} 次)", delay, _reconnectAttempts + 1);
        _reconnectTimer.Change((int)(delay * 1000), Timeout.Infinite);
    }

    /// <summary>
    /// 尝试重连
    /// </summary>
    private async void AttemptReconnect(object? state)
    {
        if (_isDisposed)
        {
            return;
        }

        try
        {
            _reconnectAttempts++;

            if (_reconnectAttempts > _maxReconnectAttempts)
            {
                _logger.LogError("MQTT重连次数超过最大限制 ({MaxAttempts})，停止重连", _maxReconnectAttempts);
                _isReconnecting = false;
                return;
            }

            _logger.LogInformation("正在尝试MQTT重连 (第 {Attempt} 次)", _reconnectAttempts);

            var success = await ConnectAsync();
            if (success)
            {
                _logger.LogInformation("MQTT重连成功");
                _isReconnecting = false;
                
                // 重新订阅之前的主题
                await ResubscribeTopics();
            }
            else
            {
                _logger.LogWarning("MQTT重连失败，将继续尝试");
                StartReconnectTimer(); // 继续重连
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT重连过程中发生异常");
            StartReconnectTimer(); // 继续重连
        }
    }

    /// <summary>
    /// 重新订阅之前的主题
    /// </summary>
    private async Task ResubscribeTopics()
    {
        try
        {
            var topics = _messageHandlers.Keys.ToList();
            foreach (var topic in topics)
            {
                try
                {
                    var subscribeOptions = new MqttTopicFilterBuilder()
                        .WithTopic(topic)
                        .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                        .Build();

                    await _mqttClient.SubscribeAsync(subscribeOptions);
                    _logger.LogDebug("重新订阅主题成功: {Topic}", topic);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "重新订阅主题失败: {Topic}", topic);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "重新订阅主题过程中发生异常");
        }
    }

    /// <summary>
    /// 检查主题是否匹配
    /// </summary>
    private bool IsTopicMatch(string topic, string pattern)
    {
        // 简单的通配符匹配，支持 + 和 #
        var topicParts = topic.Split('/');
        var patternParts = pattern.Split('/');

        if (patternParts.Length > topicParts.Length && !pattern.EndsWith("/#"))
        {
            return false;
        }

        for (int i = 0; i < patternParts.Length; i++)
        {
            if (i >= topicParts.Length)
            {
                return patternParts[i] == "#";
            }

            if (patternParts[i] == "+" || patternParts[i] == "#")
            {
                continue;
            }

            if (patternParts[i] != topicParts[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 获取数据点值
    /// </summary>
    private object GetDataPointValue(DataPoint dp)
    {
        if (dp.NumericValue.HasValue)
        {
            return dp.NumericValue.Value;
        }
        if (!string.IsNullOrEmpty(dp.StringValue))
        {
            return dp.StringValue;
        }
        if (dp.BooleanValue.HasValue)
        {
            return dp.BooleanValue.Value;
        }
        if (!string.IsNullOrEmpty(dp.RawValue))
        {
            return dp.RawValue;
        }
        return null;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        lock (_lockObject)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
        }

        try
        {
            _reconnectTimer?.Dispose();
            _mqttClient?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放MQTT资源时发生异常");
        }
    }
}

/// <summary>
/// MQTT配置
/// </summary>
public class MqttConfiguration
{
    /// <summary>
    /// 服务器地址
    /// </summary>
    public string Server { get; set; } = "localhost";

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; } = 1883;

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = $"iot_collector_{Guid.NewGuid():N}";

    /// <summary>
    /// 用户名
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// 密码
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// 保活时间（秒）
    /// </summary>
    public int KeepAliveSeconds { get; set; } = 60;

    /// <summary>
    /// 重连延迟（秒）
    /// </summary>
    public int ReconnectDelaySeconds { get; set; } = 5;
} 
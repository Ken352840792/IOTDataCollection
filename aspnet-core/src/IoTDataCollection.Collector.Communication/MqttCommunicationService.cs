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

            _logger.LogInformation("MQTT客户端启动成功");
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

            await _mqttClient.DisconnectAsync();
            _logger.LogInformation("MQTT连接已断开");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT断开连接失败");
            return false;
        }
    }

    /// <summary>
    /// 发布消息
    /// </summary>
    public async Task<bool> PublishAsync(string topic, object payload, int qos = 1, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                _logger.LogWarning("MQTT客户端未连接，无法发布消息");
                return false;
            }

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(JsonConvert.SerializeObject(payload))
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .WithRetainFlag(false)
                .Build();

            await _mqttClient.PublishAsync(message, cancellationToken);

            _logger.LogDebug("消息发布成功: {Topic}, QoS: {QoS}", topic, qos);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "消息发布失败: {Topic}", topic);
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
                _logger.LogWarning("MQTT客户端未连接，无法订阅主题");
                return false;
            }

            // 注册消息处理器
            _messageHandlers[topic] = handler;

            // 订阅主题
            var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .WithQualityOfServiceLevel((MQTTnet.Protocol.MqttQualityOfServiceLevel)qos)
                .Build();

            await _mqttClient.SubscribeAsync(topicFilter);

            _logger.LogInformation("主题订阅成功: {Topic}, QoS: {QoS}", topic, qos);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "主题订阅失败: {Topic}", topic);
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
                _logger.LogWarning("MQTT客户端未连接，无法取消订阅");
                return false;
            }

            // 移除消息处理器
            _messageHandlers.Remove(topic);

            // 取消订阅
            await _mqttClient.UnsubscribeAsync(topic);

            _logger.LogInformation("主题取消订阅成功: {Topic}", topic);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "主题取消订阅失败: {Topic}", topic);
            return false;
        }
    }

    /// <summary>
    /// 发送设备数据
    /// </summary>
    public async Task<bool> SendDeviceDataAsync(string deviceCode, List<DataPoint> dataPoints, CancellationToken cancellationToken = default)
    {
        try
        {
            var topic = $"iot/device/{deviceCode}/data";
            var payload = new
            {
                DeviceCode = deviceCode,
                Timestamp = DateTime.UtcNow,
                DataPoints = dataPoints
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
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _mqttClient?.Dispose();
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
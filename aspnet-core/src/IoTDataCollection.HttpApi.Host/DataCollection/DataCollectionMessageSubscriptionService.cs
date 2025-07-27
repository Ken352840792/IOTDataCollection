using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using IoTDataCollection.DataCollection;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// 数据采集消息订阅服务配置
/// </summary>
public class DataCollectionMessageSubscriptionOptions
{
    /// <summary>
    /// MQTT服务器地址
    /// </summary>
    public string MqttServer { get; set; } = "localhost";

    /// <summary>
    /// MQTT服务器端口
    /// </summary>
    public int MqttPort { get; set; } = 1883;

    /// <summary>
    /// MQTT客户端ID
    /// </summary>
    public string ClientId { get; set; } = "IoTDataCollection_Center";

    /// <summary>
    /// 设备数据主题
    /// </summary>
    public string DeviceDataTopic { get; set; } = "iot/device/data";

    /// <summary>
    /// 采集端状态主题
    /// </summary>
    public string CollectorStatusTopic { get; set; } = "iot/collector/status";

    /// <summary>
    /// 连接超时时间（秒）
    /// </summary>
    public int ConnectionTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// 重连间隔（秒）
    /// </summary>
    public int ReconnectIntervalSeconds { get; set; } = 10;
}

/// <summary>
/// 数据采集消息订阅服务
/// </summary>
public class DataCollectionMessageSubscriptionService : BackgroundService
{
    private readonly ILogger<DataCollectionMessageSubscriptionService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly DataCollectionMessageSubscriptionOptions _options;
    private IMqttClient? _mqttClient;
    private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

    public DataCollectionMessageSubscriptionService(
        ILogger<DataCollectionMessageSubscriptionService> logger,
        IServiceProvider serviceProvider,
        IOptions<DataCollectionMessageSubscriptionOptions> options)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("启动数据采集消息订阅服务...");

        try
        {
            await InitializeMqttClientAsync(stoppingToken);
            await SubscribeToTopicsAsync(stoppingToken);

            // 保持服务运行
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                
                // 检查连接状态
                if (_mqttClient?.IsConnected == false)
                {
                    _logger.LogWarning("MQTT连接断开，尝试重连...");
                    await ReconnectAsync(stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据采集消息订阅服务执行异常");
        }
        finally
        {
            await CleanupAsync();
        }
    }

    private async Task InitializeMqttClientAsync(CancellationToken cancellationToken)
    {
        var factory = new MqttFactory();
        _mqttClient = factory.CreateMqttClient();

        // 配置MQTT客户端选项
        var options = new MqttClientOptionsBuilder()
            .WithTcpServer(_options.MqttServer, _options.MqttPort)
            .WithClientId(_options.ClientId)
            .WithCleanSession()
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .Build();

        // 设置连接事件处理
        _mqttClient.ConnectedAsync += OnMqttConnectedAsync;
        _mqttClient.DisconnectedAsync += OnMqttDisconnectedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += OnMqttMessageReceivedAsync;

        // 连接到MQTT服务器
        await _mqttClient.ConnectAsync(options, cancellationToken);
    }

    private async Task SubscribeToTopicsAsync(CancellationToken cancellationToken)
    {
        if (_mqttClient?.IsConnected != true)
        {
            _logger.LogWarning("MQTT客户端未连接，无法订阅主题");
            return;
        }

        // 订阅设备数据主题
        var deviceDataSubscribeOptions = new MqttTopicFilterBuilder()
            .WithTopic(_options.DeviceDataTopic)
            .Build();

        await _mqttClient.SubscribeAsync(deviceDataSubscribeOptions, cancellationToken);
        _logger.LogInformation("已订阅设备数据主题: {Topic}", _options.DeviceDataTopic);

        // 订阅采集端状态主题
        var collectorStatusSubscribeOptions = new MqttTopicFilterBuilder()
            .WithTopic(_options.CollectorStatusTopic)
            .Build();

        await _mqttClient.SubscribeAsync(collectorStatusSubscribeOptions, cancellationToken);
        _logger.LogInformation("已订阅采集端状态主题: {Topic}", _options.CollectorStatusTopic);
    }

    private async Task OnMqttConnectedAsync(MqttClientConnectedEventArgs eventArgs)
    {
        _logger.LogInformation("MQTT连接成功");
        await Task.CompletedTask;
    }

    private async Task OnMqttDisconnectedAsync(MqttClientDisconnectedEventArgs eventArgs)
    {
        _logger.LogWarning("MQTT连接断开: {Reason}", eventArgs.Reason);
        await Task.CompletedTask;
    }

    private async Task OnMqttMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs eventArgs)
    {
        try
        {
            var topic = eventArgs.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(eventArgs.ApplicationMessage.PayloadSegment);

            _logger.LogDebug("收到MQTT消息: {Topic}, 长度: {Length}", topic, payload.Length);

            using var scope = _serviceProvider.CreateScope();
            var messageHandler = scope.ServiceProvider.GetRequiredService<IDataCollectionMessageHandler>();

            if (topic == _options.DeviceDataTopic)
            {
                await ProcessDeviceDataMessageAsync(payload, messageHandler);
            }
            else if (topic == _options.CollectorStatusTopic)
            {
                await ProcessCollectorStatusMessageAsync(payload, messageHandler);
            }
            else
            {
                _logger.LogWarning("收到未知主题的消息: {Topic}", topic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理MQTT消息失败");
        }

        await Task.CompletedTask;
    }

    private async Task ProcessDeviceDataMessageAsync(string payload, IDataCollectionMessageHandler messageHandler)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<DeviceDataMessage>(payload);
            if (message != null)
            {
                var result = await messageHandler.ProcessDeviceDataMessageAsync(message);
                if (!result.Success)
                {
                    _logger.LogError("处理设备数据消息失败: {Error}", result.ErrorMessage);
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "解析设备数据消息JSON失败");
        }
    }

    private async Task ProcessCollectorStatusMessageAsync(string payload, IDataCollectionMessageHandler messageHandler)
    {
        try
        {
            var message = JsonConvert.DeserializeObject<CollectorStatusMessage>(payload);
            if (message != null)
            {
                var result = await messageHandler.ProcessCollectorStatusMessageAsync(message);
                if (!result.Success)
                {
                    _logger.LogError("处理采集端状态消息失败: {Error}", result.ErrorMessage);
                }
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "解析采集端状态消息JSON失败");
        }
    }

    private async Task ReconnectAsync(CancellationToken cancellationToken)
    {
        await _connectionSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (_mqttClient?.IsConnected == false)
            {
                var options = new MqttClientOptionsBuilder()
                    .WithTcpServer(_options.MqttServer, _options.MqttPort)
                    .WithClientId(_options.ClientId)
                    .WithCleanSession()
                    .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
                    .Build();

                await _mqttClient.ConnectAsync(options, cancellationToken);
                await SubscribeToTopicsAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "MQTT重连失败");
        }
        finally
        {
            _connectionSemaphore.Release();
        }
    }

    private async Task CleanupAsync()
    {
        if (_mqttClient != null)
        {
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.DisconnectAsync();
            }
            _mqttClient.Dispose();
        }
        _connectionSemaphore.Dispose();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("停止数据采集消息订阅服务...");
        await CleanupAsync();
        await base.StopAsync(cancellationToken);
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Core.Services;
using IoTDataCollection.Collector.Protocols;

namespace IoTDataCollection.Collector.Console.Services;

/// <summary>
/// 采集服务
/// </summary>
public class CollectorService : ICollectorService
{
    private readonly ILogger<CollectorService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IStorageService _storageService;
    private readonly ICommunicationService _communicationService;
    private readonly IRuleEngine _ruleEngine;
    private readonly IProtocolFactory _protocolFactory;
    private readonly CollectorConfiguration _config;
    private readonly Dictionary<string, IDeviceProtocol> _deviceProtocols;

    public CollectorService(
        ILogger<CollectorService> logger,
        IConfigurationService configurationService,
        IStorageService storageService,
        ICommunicationService communicationService,
        IRuleEngine ruleEngine,
        IProtocolFactory protocolFactory,
        IOptions<CollectorConfiguration> config)
    {
        _logger = logger;
        _configurationService = configurationService;
        _storageService = storageService;
        _communicationService = communicationService;
        _ruleEngine = ruleEngine;
        _protocolFactory = protocolFactory;
        _config = config.Value;
        _deviceProtocols = new Dictionary<string, IDeviceProtocol>();
    }

    /// <summary>
    /// 初始化采集服务
    /// </summary>
    public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("初始化采集服务");

            // 初始化配置服务
            if (!await _configurationService.InitializeAsync(cancellationToken))
            {
                _logger.LogError("配置服务初始化失败");
                return false;
            }

            // 初始化存储服务
            if (!await _storageService.InitializeAsync(cancellationToken))
            {
                _logger.LogError("存储服务初始化失败");
                return false;
            }

            // 初始化通信服务
            if (_config.EnableMqttCommunication)
            {
                if (!await _communicationService.ConnectAsync(cancellationToken))
                {
                    _logger.LogError("通信服务初始化失败");
                    return false;
                }
            }

            // 初始化设备连接
            await InitializeDeviceConnectionsAsync(cancellationToken);

            _logger.LogInformation("采集服务初始化完成");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集服务初始化失败");
            return false;
        }
    }

    /// <summary>
    /// 采集设备数据
    /// </summary>
    public async Task<bool> CollectDeviceDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();

            _logger.LogDebug("开始采集设备数据，设备数量: {Count}", enabledDevices.Count);

            foreach (var deviceConfig in enabledDevices)
            {
                try
                {
                    await CollectSingleDeviceDataAsync(deviceConfig, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "采集设备数据失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量采集设备数据失败");
            return false;
        }
    }

    /// <summary>
    /// 发送未发送的数据
    /// </summary>
    public async Task<bool> SendUnsentDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_config.EnableMqttCommunication)
            {
                return true;
            }

            var unsentData = await _storageService.GetUnsentDataPointsAsync(1000, cancellationToken);
            if (unsentData.Count == 0)
            {
                return true;
            }

            _logger.LogDebug("开始发送未发送数据，数据点数量: {Count}", unsentData.Count);

            // 按设备分组发送
            var deviceGroups = unsentData.GroupBy(d => d.DeviceCode);
            var sentIds = new List<string>();

            foreach (var deviceGroup in deviceGroups)
            {
                try
                {
                    var deviceData = deviceGroup.ToList();
                    var success = await _communicationService.SendDeviceDataAsync(deviceGroup.Key, deviceData, cancellationToken);

                    if (success)
                    {
                        sentIds.AddRange(deviceData.Select(d => d.Id));
                        _logger.LogDebug("设备数据发送成功: {DeviceCode}, 数据点数量: {Count}", 
                            deviceGroup.Key, deviceData.Count);
                    }
                    else
                    {
                        _logger.LogWarning("设备数据发送失败: {DeviceCode}", deviceGroup.Key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "发送设备数据异常: {DeviceCode}", deviceGroup.Key);
                }
            }

            // 标记已发送的数据
            if (sentIds.Count > 0)
            {
                await _storageService.MarkDataPointsAsSentAsync(sentIds, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送未发送数据失败");
            return false;
        }
    }

    /// <summary>
    /// 发送心跳消息
    /// </summary>
    public async Task<bool> SendHeartbeatAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_config.EnableMqttCommunication)
            {
                return true;
            }

            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();

            foreach (var deviceConfig in enabledDevices)
            {
                try
                {
                    var status = _deviceProtocols.TryGetValue(deviceConfig.DeviceCode, out var protocol) && protocol.IsConnected
                        ? DeviceStatus.Online
                        : DeviceStatus.Offline;

                    await _communicationService.SendHeartbeatAsync(deviceConfig.DeviceCode, status, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "发送心跳消息失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送心跳消息失败");
            return false;
        }
    }

    /// <summary>
    /// 清理过期数据
    /// </summary>
    public async Task<bool> CleanupExpiredDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("开始清理过期数据");

            var result = await _storageService.DeleteExpiredDataAsync(30, cancellationToken);

            if (result)
            {
                _logger.LogInformation("过期数据清理完成");
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理过期数据失败");
            return false;
        }
    }

    /// <summary>
    /// 获取采集统计信息
    /// </summary>
    public async Task<CollectorStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var storageStats = await _storageService.GetStatisticsAsync(cancellationToken);
            var configStats = await _configurationService.GetStatisticsAsync(cancellationToken);

            var statistics = new CollectorStatistics
            {
                TotalDataPoints = storageStats.TotalDataPoints,
                UnsentDataPoints = storageStats.UnsentDataPoints,
                SentDataPoints = storageStats.SentDataPoints,
                DeviceConfigCount = configStats.DeviceConfigCount,
                EnabledDeviceCount = configStats.EnabledDeviceCount,
                ConnectedDeviceCount = _deviceProtocols.Count(p => p.Value.IsConnected),
                RuleCount = configStats.RuleCount,
                EnabledRuleCount = configStats.EnabledRuleCount,
                DatabaseSizeBytes = storageStats.DatabaseSizeBytes,
                IsMqttConnected = _communicationService.IsConnected,
                Timestamp = DateTime.UtcNow
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取采集统计信息失败");
            return new CollectorStatistics();
        }
    }

    /// <summary>
    /// 初始化设备连接
    /// </summary>
    private async Task InitializeDeviceConnectionsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();

            _logger.LogInformation("初始化设备连接，设备数量: {Count}", enabledDevices.Count);

            foreach (var deviceConfig in enabledDevices)
            {
                try
                {
                    await ConnectDeviceAsync(deviceConfig, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "初始化设备连接失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "初始化设备连接失败");
        }
    }

    /// <summary>
    /// 连接设备
    /// </summary>
    private async Task ConnectDeviceAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken)
    {
        try
        {
            // 创建协议适配器
            var protocol = _protocolFactory.CreateProtocol(deviceConfig);
            _deviceProtocols[deviceConfig.DeviceCode] = protocol;

            // 连接设备
            var connected = await protocol.ConnectAsync(deviceConfig, cancellationToken);
            if (connected)
            {
                _logger.LogInformation("设备连接成功: {DeviceCode}", deviceConfig.DeviceCode);
            }
            else
            {
                _logger.LogWarning("设备连接失败: {DeviceCode}", deviceConfig.DeviceCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "连接设备异常: {DeviceCode}", deviceConfig.DeviceCode);
        }
    }

    /// <summary>
    /// 采集单个设备数据
    /// </summary>
    private async Task CollectSingleDeviceDataAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken)
    {
        try
        {
            if (!_deviceProtocols.TryGetValue(deviceConfig.DeviceCode, out var protocol))
            {
                _logger.LogWarning("设备协议未找到: {DeviceCode}", deviceConfig.DeviceCode);
                return;
            }

            if (!protocol.IsConnected)
            {
                _logger.LogWarning("设备未连接: {DeviceCode}", deviceConfig.DeviceCode);
                return;
            }

            // 获取数据点地址
            var addresses = deviceConfig.DataPoints
                .Where(dp => dp.IsEnabled)
                .Select(dp => dp.Address)
                .ToArray();

            if (addresses.Length == 0)
            {
                return;
            }

            // 读取数据
            var dataPoints = await protocol.ReadDataAsync(addresses, cancellationToken);
            if (dataPoints.Count == 0)
            {
                return;
            }

            // 应用规则处理
            if (_config.EnableRuleEngine)
            {
                dataPoints = await ApplyRulesAsync(dataPoints, cancellationToken);
            }

            // 保存到本地存储
            if (_config.EnableLocalStorage)
            {
                await _storageService.SaveDataPointsAsync(dataPoints, cancellationToken);
            }

            _logger.LogDebug("设备数据采集完成: {DeviceCode}, 数据点数量: {Count}", 
                deviceConfig.DeviceCode, dataPoints.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集单个设备数据失败: {DeviceCode}", deviceConfig.DeviceCode);
        }
    }

    /// <summary>
    /// 应用规则处理
    /// </summary>
    private async Task<List<DataPoint>> ApplyRulesAsync(List<DataPoint> dataPoints, CancellationToken cancellationToken)
    {
        try
        {
            var rules = await _configurationService.GetAllRulesAsync(cancellationToken);
            var enabledRules = rules.Where(r => r.IsEnabled).ToList();

            if (enabledRules.Count == 0)
            {
                return dataPoints;
            }

            var processedDataPoints = new List<DataPoint>(dataPoints);

            foreach (var rule in enabledRules)
            {
                try
                {
                    var result = await _ruleEngine.ExecuteRuleAsync(rule.Code, processedDataPoints, cancellationToken);
                    if (result.Success && result.OutputData.Count > 0)
                    {
                        processedDataPoints.AddRange(result.OutputData);
                        _logger.LogDebug("规则执行成功: {RuleId}, 输出数据点数量: {Count}", 
                            rule.Id, result.OutputData.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "规则执行失败: {RuleId}", rule.Id);
                }
            }

            return processedDataPoints;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "应用规则处理失败");
            return dataPoints;
        }
    }
}

/// <summary>
/// 采集服务接口
/// </summary>
public interface ICollectorService
{
    /// <summary>
    /// 初始化采集服务
    /// </summary>
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 采集设备数据
    /// </summary>
    Task<bool> CollectDeviceDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送未发送的数据
    /// </summary>
    Task<bool> SendUnsentDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送心跳消息
    /// </summary>
    Task<bool> SendHeartbeatAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 清理过期数据
    /// </summary>
    Task<bool> CleanupExpiredDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取采集统计信息
    /// </summary>
    Task<CollectorStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 采集统计信息
/// </summary>
public class CollectorStatistics
{
    /// <summary>
    /// 总数据点数
    /// </summary>
    public long TotalDataPoints { get; set; }

    /// <summary>
    /// 未发送数据点数
    /// </summary>
    public long UnsentDataPoints { get; set; }

    /// <summary>
    /// 已发送数据点数
    /// </summary>
    public long SentDataPoints { get; set; }

    /// <summary>
    /// 设备配置数量
    /// </summary>
    public int DeviceConfigCount { get; set; }

    /// <summary>
    /// 启用的设备数量
    /// </summary>
    public int EnabledDeviceCount { get; set; }

    /// <summary>
    /// 已连接的设备数量
    /// </summary>
    public int ConnectedDeviceCount { get; set; }

    /// <summary>
    /// 规则数量
    /// </summary>
    public int RuleCount { get; set; }

    /// <summary>
    /// 启用的规则数量
    /// </summary>
    public int EnabledRuleCount { get; set; }

    /// <summary>
    /// 数据库大小（字节）
    /// </summary>
    public long DatabaseSizeBytes { get; set; }

    /// <summary>
    /// MQTT连接状态
    /// </summary>
    public bool IsMqttConnected { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 
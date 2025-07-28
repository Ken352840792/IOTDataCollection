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
public class CollectorService : IoTDataCollection.Collector.Core.Interfaces.ICollectorService
{
    private readonly ILogger<CollectorService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IStorageService _storageService;
    private readonly ICommunicationService _communicationService;
    private readonly IRuleEngine _ruleEngine;
    private readonly IProtocolFactory _protocolFactory;
    private readonly ISystemMonitoringService _systemMonitoringService;
    private readonly CollectorConfiguration _config;
    private readonly Dictionary<string, IDeviceProtocol> _deviceProtocols;

    public CollectorService(
        ILogger<CollectorService> logger,
        IConfigurationService configurationService,
        IStorageService storageService,
        ICommunicationService communicationService,
        IRuleEngine ruleEngine,
        IProtocolFactory protocolFactory,
        ISystemMonitoringService systemMonitoringService,
        IOptions<CollectorConfiguration> config)
    {
        _logger = logger;
        _configurationService = configurationService;
        _storageService = storageService;
        _communicationService = communicationService;
        _ruleEngine = ruleEngine;
        _protocolFactory = protocolFactory;
        _systemMonitoringService = systemMonitoringService;
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

            // 检查MQTT连接状态
            if (!_communicationService.IsConnected)
            {
                _logger.LogDebug("MQTT未连接，跳过数据发送，数据将保留在本地缓存中");
                return false;
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
            var failedDevices = new List<string>();

            foreach (var deviceGroup in deviceGroups)
            {
                try
                {
                    var deviceData = deviceGroup.ToList();
                    
                    // 检查网络连接状态
                    if (!_communicationService.IsConnected)
                    {
                        _logger.LogWarning("发送过程中MQTT连接断开，停止发送剩余数据");
                        break;
                    }

                    var success = await _communicationService.SendDeviceDataAsync(deviceGroup.Key, deviceData, cancellationToken);

                    if (success)
                    {
                        sentIds.AddRange(deviceData.Select(d => d.Id));
                        _logger.LogDebug("设备数据发送成功: {DeviceCode}, 数据点数量: {Count}", 
                            deviceGroup.Key, deviceData.Count);
                    }
                    else
                    {
                        failedDevices.Add(deviceGroup.Key);
                        _logger.LogWarning("设备数据发送失败: {DeviceCode}", deviceGroup.Key);
                    }
                }
                catch (Exception ex)
                {
                    failedDevices.Add(deviceGroup.Key);
                    _logger.LogError(ex, "发送设备数据异常: {DeviceCode}", deviceGroup.Key);
                }
            }

            // 标记已发送的数据
            if (sentIds.Count > 0)
            {
                await _storageService.MarkDataPointsAsSentAsync(sentIds, cancellationToken);
                _logger.LogInformation("成功发送 {SentCount} 个数据点，失败设备: {FailedDevices}", 
                    sentIds.Count, string.Join(", ", failedDevices));
            }

            return sentIds.Count > 0;
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

            // 检查MQTT连接状态
            if (!_communicationService.IsConnected)
            {
                _logger.LogDebug("MQTT未连接，跳过心跳发送");
                return false;
            }

            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();

            var successCount = 0;
            var totalCount = enabledDevices.Count;

            foreach (var deviceConfig in enabledDevices)
            {
                try
                {
                    // 检查网络连接状态
                    if (!_communicationService.IsConnected)
                    {
                        _logger.LogWarning("心跳发送过程中MQTT连接断开，停止发送剩余心跳");
                        break;
                    }

                    var status = _deviceProtocols.TryGetValue(deviceConfig.DeviceCode, out var protocol) && protocol.IsConnected
                        ? DeviceStatus.Online
                        : DeviceStatus.Offline;

                    var success = await _communicationService.SendHeartbeatAsync(deviceConfig.DeviceCode, status, cancellationToken);
                    if (success)
                    {
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "发送心跳消息失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }

            _logger.LogDebug("心跳发送完成: {SuccessCount}/{TotalCount}", successCount, totalCount);
            return successCount > 0;
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
    /// 报告状态
    /// </summary>
    public async Task<bool> ReportStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("开始报告采集端状态");

            if (_communicationService == null || !_communicationService.IsConnected)
            {
                _logger.LogWarning("MQTT未连接，跳过状态报告");
                return false;
            }

            // 获取系统资源信息
            var systemResources = _systemMonitoringService.GetCurrentSystemResources();

            // 构建设备连接状态
            var deviceConnections = new Dictionary<string, bool>();
            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            foreach (var deviceConfig in deviceConfigs)
            {
                if (_deviceProtocols.TryGetValue(deviceConfig.DeviceCode, out var protocol))
                {
                    deviceConnections[deviceConfig.DeviceCode] = protocol.IsConnected;
                }
                else
                {
                    deviceConnections[deviceConfig.DeviceCode] = false;
                }
            }

            // 构建状态消息
            var statusMessage = new CollectorStatusMessage
            {
                CollectorNode = _config.CollectorNodeCode,
                Timestamp = DateTime.UtcNow,
                StatusType = "Online",
                StatusDescription = "采集端正常运行",
                DeviceConnections = deviceConnections,
                SystemResources = systemResources
            };

            // 发送状态消息
            var topic = $"iot/collector/{_config.CollectorNodeCode}/status";
            var success = await _communicationService.PublishAsync(topic, statusMessage, 1, cancellationToken);
            
            if (success)
            {
                _logger.LogDebug("状态报告发送成功: {CollectorNode}", _config.CollectorNodeCode);
            }
            else
            {
                _logger.LogWarning("状态报告发送失败: {CollectorNode}", _config.CollectorNodeCode);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "报告状态失败");
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
            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();
            var connectedDevices = enabledDevices.Count(d => 
                _deviceProtocols.TryGetValue(d.DeviceCode, out var protocol) && protocol.IsConnected);

            var stats = await _storageService.GetStatisticsAsync(cancellationToken);

            return new CollectorStatistics
            {
                TotalDataPoints = stats.TotalDataPoints,
                UnsentDataPoints = stats.UnsentDataPoints,
                SentDataPoints = stats.SentDataPoints,
                DeviceConfigCount = deviceConfigs.Count,
                EnabledDeviceCount = enabledDevices.Count,
                ConnectedDeviceCount = connectedDevices,
                RuleCount = stats.RuleCount,
                EnabledRuleCount = 0, // 暂时设为0，后续可以从规则引擎获取
                DatabaseSizeBytes = stats.DatabaseSizeBytes,
                IsMqttConnected = _communicationService.IsConnected,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取统计信息失败");
            return new CollectorStatistics
            {
                Timestamp = DateTime.UtcNow,
                IsMqttConnected = _communicationService.IsConnected
            };
        }
    }

    /// <summary>
    /// 采集数据
    /// </summary>
    public async Task<bool> CollectDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("开始采集设备数据");

            var deviceConfigs = await _configurationService.GetAllDeviceConfigsAsync(cancellationToken);
            var enabledDevices = deviceConfigs.Where(d => d.IsEnabled).ToList();

            var successCount = 0;
            var totalCount = enabledDevices.Count;

            foreach (var deviceConfig in enabledDevices)
            {
                try
                {
                    await CollectSingleDeviceDataAsync(deviceConfig, cancellationToken);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "采集设备数据失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }

            _logger.LogDebug("数据采集完成: {SuccessCount}/{TotalCount}", successCount, totalCount);
            return successCount > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集数据失败");
            return false;
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public async Task<bool> StopAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("停止采集服务");

            // 断开所有设备连接
            foreach (var protocol in _deviceProtocols.Values)
            {
                try
                {
                    await protocol.DisconnectAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "断开设备连接失败");
                }
            }

            _deviceProtocols.Clear();
            _logger.LogInformation("采集服务已停止");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止采集服务失败");
            return false;
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

            // 检查设备连接状态，如果未连接则尝试重连
            if (!protocol.IsConnected)
            {
                _logger.LogWarning("设备未连接，尝试重连: {DeviceCode}", deviceConfig.DeviceCode);
                var reconnected = await protocol.ConnectAsync(deviceConfig, cancellationToken);
                if (!reconnected)
                {
                    _logger.LogError("设备重连失败: {DeviceCode}", deviceConfig.DeviceCode);
                    return;
                }
                _logger.LogInformation("设备重连成功: {DeviceCode}", deviceConfig.DeviceCode);
            }

            // 获取数据点地址
            var addresses = deviceConfig.DataPoints
                .Where(dp => dp.IsEnabled)
                .Select(dp => dp.Address)
                .ToArray();

            if (addresses.Length == 0)
            {
                _logger.LogDebug("设备没有启用的数据点: {DeviceCode}", deviceConfig.DeviceCode);
                return;
            }

            // 读取数据
            var dataPoints = await protocol.ReadDataAsync(addresses, cancellationToken);
            if (dataPoints.Count == 0)
            {
                _logger.LogDebug("设备数据读取为空: {DeviceCode}", deviceConfig.DeviceCode);
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
                var saveSuccess = await _storageService.SaveDataPointsAsync(dataPoints, cancellationToken);
                if (!saveSuccess)
                {
                    _logger.LogWarning("设备数据保存到本地存储失败: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }

            _logger.LogDebug("设备数据采集完成: {DeviceCode}, 数据点数量: {Count}", 
                deviceConfig.DeviceCode, dataPoints.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集单个设备数据失败: {DeviceCode}", deviceConfig.DeviceCode);
            
            // 如果是连接相关错误，标记设备为断开状态
            if (ex.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
                ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                if (_deviceProtocols.TryGetValue(deviceConfig.DeviceCode, out var protocol))
                {
                    // 这里可以添加设备断开状态的处理逻辑
                    _logger.LogWarning("设备连接异常，将在下次采集时尝试重连: {DeviceCode}", deviceConfig.DeviceCode);
                }
            }
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
    /// 停止服务
    /// </summary>
    Task<bool> StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 采集数据
    /// </summary>
    Task<bool> CollectDataAsync(CancellationToken cancellationToken = default);

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
    /// 报告状态
    /// </summary>
    Task<bool> ReportStatusAsync(CancellationToken cancellationToken = default);

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
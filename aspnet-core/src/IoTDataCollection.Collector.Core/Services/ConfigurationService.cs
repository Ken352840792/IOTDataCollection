using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Services;

/// <summary>
/// 配置管理服务
/// </summary>
public class ConfigurationService : IConfigurationService
{
    private readonly ILogger<ConfigurationService> _logger;
    private readonly IStorageService _storageService;
    private readonly IConfigurationSyncService? _configurationSyncService;
    private readonly Dictionary<string, DeviceConfig> _deviceConfigCache;
    private readonly Dictionary<string, RuleInfo> _ruleCache;

    public ConfigurationService(
        ILogger<ConfigurationService> logger,
        IStorageService storageService,
        IConfigurationSyncService? configurationSyncService = null)
    {
        _logger = logger;
        _storageService = storageService;
        _configurationSyncService = configurationSyncService;
        _deviceConfigCache = new Dictionary<string, DeviceConfig>();
        _ruleCache = new Dictionary<string, RuleInfo>();
    }

    /// <summary>
    /// 初始化配置
    /// </summary>
    public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("初始化配置管理服务");

            // 尝试从中心系统同步配置
            if (_configurationSyncService != null)
            {
                try
                {
                    var syncedConfigs = await _configurationSyncService.SyncConfigurationsFromCenterAsync(cancellationToken);
                    _logger.LogInformation("从中心系统同步配置完成，设备数量: {Count}", syncedConfigs.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "从中心系统同步配置失败，将使用本地配置");
                }
            }

            // 加载设备配置到缓存
            var deviceConfigs = await _storageService.GetAllDeviceConfigsAsync(cancellationToken);
            foreach (var config in deviceConfigs)
            {
                _deviceConfigCache[config.DeviceCode] = config;
            }

            // 加载规则到缓存
            var rules = await _storageService.GetAllRulesAsync(cancellationToken);
            foreach (var rule in rules)
            {
                _ruleCache[rule.Id] = rule;
            }

            _logger.LogInformation("配置管理服务初始化完成，设备配置: {DeviceCount}, 规则: {RuleCount}", 
                _deviceConfigCache.Count, _ruleCache.Count);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "配置管理服务初始化失败");
            return false;
        }
    }

    /// <summary>
    /// 获取设备配置
    /// </summary>
    public async Task<DeviceConfig?> GetDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        try
        {
            // 先从缓存获取
            if (_deviceConfigCache.TryGetValue(deviceCode, out var cachedConfig))
            {
                return cachedConfig;
            }

            // 从存储获取
            var config = await _storageService.GetDeviceConfigAsync(deviceCode, cancellationToken);
            if (config != null)
            {
                _deviceConfigCache[deviceCode] = config;
            }

            return config;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备配置失败: {DeviceCode}", deviceCode);
            return null;
        }
    }

    /// <summary>
    /// 获取所有设备配置
    /// </summary>
    public async Task<List<DeviceConfig>> GetAllDeviceConfigsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return _deviceConfigCache.Values.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有设备配置失败");
            return new List<DeviceConfig>();
        }
    }

    /// <summary>
    /// 保存设备配置
    /// </summary>
    public async Task<bool> SaveDeviceConfigAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken = default)
    {
        try
        {
            // 更新配置
            deviceConfig.UpdatedAt = DateTime.UtcNow;

            // 保存到存储
            var result = await _storageService.SaveDeviceConfigAsync(deviceConfig, cancellationToken);
            if (result)
            {
                // 更新缓存
                _deviceConfigCache[deviceConfig.DeviceCode] = deviceConfig;
                _logger.LogInformation("设备配置保存成功: {DeviceCode}", deviceConfig.DeviceCode);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存设备配置失败: {DeviceCode}", deviceConfig.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 删除设备配置
    /// </summary>
    public async Task<bool> DeleteDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        try
        {
            // 从缓存移除
            _deviceConfigCache.Remove(deviceCode);

            // TODO: 实现从存储删除的逻辑
            _logger.LogInformation("设备配置删除成功: {DeviceCode}", deviceCode);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除设备配置失败: {DeviceCode}", deviceCode);
            return false;
        }
    }

    /// <summary>
    /// 获取规则
    /// </summary>
    public async Task<RuleInfo?> GetRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // 先从缓存获取
            if (_ruleCache.TryGetValue(ruleId, out var cachedRule))
            {
                return cachedRule;
            }

            // 从存储获取
            var rule = await _storageService.GetRuleAsync(ruleId, cancellationToken);
            if (rule != null)
            {
                _ruleCache[ruleId] = rule;
            }

            return rule;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取规则失败: {RuleId}", ruleId);
            return null;
        }
    }

    /// <summary>
    /// 获取所有规则
    /// </summary>
    public async Task<List<RuleInfo>> GetAllRulesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return _ruleCache.Values.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有规则失败");
            return new List<RuleInfo>();
        }
    }

    /// <summary>
    /// 保存规则
    /// </summary>
    public async Task<bool> SaveRuleAsync(RuleInfo ruleInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // 更新规则
            ruleInfo.UpdatedAt = DateTime.UtcNow;
            if (string.IsNullOrEmpty(ruleInfo.Id))
            {
                ruleInfo.Id = Guid.NewGuid().ToString();
                ruleInfo.CreatedAt = DateTime.UtcNow;
                ruleInfo.Version = 1;
            }
            else
            {
                ruleInfo.Version++;
            }

            // 保存到存储
            var result = await _storageService.SaveRuleAsync(ruleInfo, cancellationToken);
            if (result)
            {
                // 更新缓存
                _ruleCache[ruleInfo.Id] = ruleInfo;
                _logger.LogInformation("规则保存成功: {RuleId}", ruleInfo.Id);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存规则失败: {RuleId}", ruleInfo.Id);
            return false;
        }
    }

    /// <summary>
    /// 删除规则
    /// </summary>
    public async Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // 从存储删除
            var result = await _storageService.DeleteRuleAsync(ruleId, cancellationToken);
            if (result)
            {
                // 从缓存移除
                _ruleCache.Remove(ruleId);
                _logger.LogInformation("规则删除成功: {RuleId}", ruleId);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除规则失败: {RuleId}", ruleId);
            return false;
        }
    }

    /// <summary>
    /// 刷新配置缓存
    /// </summary>
    public async Task<bool> RefreshCacheAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("刷新配置缓存");

            // 尝试从中心系统同步最新配置
            if (_configurationSyncService != null)
            {
                try
                {
                    var syncedConfigs = await _configurationSyncService.SyncConfigurationsFromCenterAsync(cancellationToken);
                    _logger.LogInformation("从中心系统同步最新配置完成，设备数量: {Count}", syncedConfigs.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "从中心系统同步最新配置失败");
                }
            }

            // 清空缓存
            _deviceConfigCache.Clear();
            _ruleCache.Clear();

            // 重新加载
            return await InitializeAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "刷新配置缓存失败");
            return false;
        }
    }

    /// <summary>
    /// 获取配置统计信息
    /// </summary>
    public async Task<ConfigurationStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var statistics = new ConfigurationStatistics
            {
                DeviceConfigCount = _deviceConfigCache.Count,
                RuleCount = _ruleCache.Count,
                EnabledDeviceCount = _deviceConfigCache.Values.Count(c => c.IsEnabled),
                EnabledRuleCount = _ruleCache.Values.Count(r => r.IsEnabled),
                Timestamp = DateTime.UtcNow
            };

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取配置统计信息失败");
            return new ConfigurationStatistics();
        }
    }
}

/// <summary>
/// 配置管理服务接口
/// </summary>
public interface IConfigurationService
{
    /// <summary>
    /// 初始化配置
    /// </summary>
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备配置
    /// </summary>
    Task<DeviceConfig?> GetDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有设备配置
    /// </summary>
    Task<List<DeviceConfig>> GetAllDeviceConfigsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存设备配置
    /// </summary>
    Task<bool> SaveDeviceConfigAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除设备配置
    /// </summary>
    Task<bool> DeleteDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则
    /// </summary>
    Task<RuleInfo?> GetRuleAsync(string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有规则
    /// </summary>
    Task<List<RuleInfo>> GetAllRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存规则
    /// </summary>
    Task<bool> SaveRuleAsync(RuleInfo ruleInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除规则
    /// </summary>
    Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 刷新配置缓存
    /// </summary>
    Task<bool> RefreshCacheAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取配置统计信息
    /// </summary>
    Task<ConfigurationStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 配置统计信息
/// </summary>
public class ConfigurationStatistics
{
    /// <summary>
    /// 设备配置数量
    /// </summary>
    public int DeviceConfigCount { get; set; }

    /// <summary>
    /// 规则数量
    /// </summary>
    public int RuleCount { get; set; }

    /// <summary>
    /// 启用的设备数量
    /// </summary>
    public int EnabledDeviceCount { get; set; }

    /// <summary>
    /// 启用的规则数量
    /// </summary>
    public int EnabledRuleCount { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 
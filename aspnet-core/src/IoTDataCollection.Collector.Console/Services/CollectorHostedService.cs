using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Core.Services;
using IoTDataCollection.Collector.Console.Services;

namespace IoTDataCollection.Collector.Console.Services;

/// <summary>
/// 采集器托管服务
/// </summary>
public class CollectorHostedService : BackgroundService
{
    private readonly ILogger<CollectorHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CollectorConfiguration _config;
    private readonly Timer _dataCollectionTimer;
    private readonly Timer _heartbeatTimer;
    private readonly Timer _statusReportTimer;
    private readonly Timer _configSyncTimer;
    private IoTDataCollection.Collector.Core.Interfaces.ICollectorService? _collectorService;
    private ICommunicationService? _communicationService;
    private ISystemMonitoringService? _systemMonitoringService;
    private IConfigurationSyncService? _configSyncService;

    public CollectorHostedService(
        ILogger<CollectorHostedService> logger,
        IServiceProvider serviceProvider,
        IOptions<CollectorConfiguration> config)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config.Value;
        _dataCollectionTimer = new Timer(CollectData, null, Timeout.Infinite, Timeout.Infinite);
        _heartbeatTimer = new Timer(SendHeartbeat, null, Timeout.Infinite, Timeout.Infinite);
        _statusReportTimer = new Timer(ReportStatus, null, Timeout.Infinite, Timeout.Infinite);
        _configSyncTimer = new Timer(SyncConfiguration, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// 执行后台服务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("启动采集器托管服务");

            // 获取服务实例
            _collectorService = _serviceProvider.GetRequiredService<IoTDataCollection.Collector.Core.Interfaces.ICollectorService>();
            _communicationService = _serviceProvider.GetRequiredService<ICommunicationService>();
            _systemMonitoringService = _serviceProvider.GetRequiredService<ISystemMonitoringService>();
            _configSyncService = _serviceProvider.GetRequiredService<IConfigurationSyncService>();

            // 启动系统监控
            _systemMonitoringService.StartMonitoring();
            _logger.LogInformation("系统监控服务已启动");

            // 订阅系统资源采集事件
            _systemMonitoringService.OnSystemResourcesCollected += OnSystemResourcesCollected;

            // 初始化采集服务
            await _collectorService.InitializeAsync(stoppingToken);

            // 启动MQTT通信
            if (_config.EnableMqttCommunication)
            {
                await _communicationService.ConnectAsync(stoppingToken);
            }

            // 启动定时器
            StartTimers();

            _logger.LogInformation("采集器托管服务启动完成");

            // 等待停止信号
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("采集器托管服务收到停止信号");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集器托管服务执行异常");
        }
        finally
        {
            await StopAsync(stoppingToken);
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("正在停止采集器托管服务");

            // 停止定时器
            StopTimers();

            // 停止系统监控
            if (_systemMonitoringService != null)
            {
                _systemMonitoringService.OnSystemResourcesCollected -= OnSystemResourcesCollected;
                _systemMonitoringService.StopMonitoring();
                _systemMonitoringService.Dispose();
            }

            // 断开MQTT连接
            if (_communicationService != null && _config.EnableMqttCommunication)
            {
                await _communicationService.DisconnectAsync(cancellationToken);
            }

            // 停止采集服务
            if (_collectorService != null)
            {
                await _collectorService.StopAsync(cancellationToken);
            }

            _logger.LogInformation("采集器托管服务已停止");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止采集器托管服务时发生异常");
        }
        finally
        {
            await base.StopAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 启动定时器
    /// </summary>
    private void StartTimers()
    {
        // 数据采集定时器
        _dataCollectionTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_config.SamplingIntervalSeconds));

        // 心跳定时器
        _heartbeatTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_config.HeartbeatIntervalSeconds));

        // 状态报告定时器（每30秒报告一次）
        _statusReportTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));

        // 配置同步定时器
        if (_config.EnableConfigSync)
        {
            _configSyncTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_config.ConfigSyncIntervalSeconds));
        }

        _logger.LogInformation("所有定时器已启动");
    }

    /// <summary>
    /// 停止定时器
    /// </summary>
    private void StopTimers()
    {
        _dataCollectionTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _heartbeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _statusReportTimer.Change(Timeout.Infinite, Timeout.Infinite);
        _configSyncTimer.Change(Timeout.Infinite, Timeout.Infinite);

        _logger.LogInformation("所有定时器已停止");
    }

    /// <summary>
    /// 数据采集回调
    /// </summary>
    private async void CollectData(object? state)
    {
        try
        {
            if (_collectorService != null)
            {
                await _collectorService.CollectDataAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据采集失败");
        }
    }

    /// <summary>
    /// 发送心跳回调
    /// </summary>
    private async void SendHeartbeat(object? state)
    {
        try
        {
            if (_collectorService != null && _communicationService != null)
            {
                await _collectorService.SendHeartbeatAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送心跳失败");
        }
    }

    /// <summary>
    /// 报告状态回调
    /// </summary>
    private async void ReportStatus(object? state)
    {
        try
        {
            if (_collectorService != null && _communicationService != null && _systemMonitoringService != null)
            {
                await _collectorService.ReportStatusAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "报告状态失败");
        }
    }

    /// <summary>
    /// 同步配置回调
    /// </summary>
    private async void SyncConfiguration(object? state)
    {
        try
        {
            if (_configSyncService != null)
            {
                await _configSyncService.SyncConfigurationsFromCenterAsync(CancellationToken.None);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "同步配置失败");
        }
    }

    /// <summary>
    /// 系统资源采集事件处理
    /// </summary>
    private void OnSystemResourcesCollected(object? sender, SystemResourcesCollectedEventArgs e)
    {
        try
        {
            _logger.LogDebug("系统资源采集事件 - CPU: {CpuUsage}%, 内存: {MemoryUsage}%, 磁盘: {DiskUsage}%", 
                e.ResourceInfo.CpuUsage, e.ResourceInfo.MemoryUsage, e.ResourceInfo.DiskUsage);

            // 这里可以添加告警检查逻辑
            CheckSystemAlerts(e.ResourceInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理系统资源采集事件失败");
        }
    }

    /// <summary>
    /// 检查系统告警
    /// </summary>
    private void CheckSystemAlerts(SystemResourceInfo resourceInfo)
    {
        try
        {
            // CPU告警检查（阈值80%）
            if (resourceInfo.CpuUsage > 80)
            {
                _logger.LogWarning("CPU使用率告警: {CpuUsage}%", resourceInfo.CpuUsage);
            }

            // 内存告警检查（阈值85%）
            if (resourceInfo.MemoryUsage > 85)
            {
                _logger.LogWarning("内存使用率告警: {MemoryUsage}%", resourceInfo.MemoryUsage);
            }

            // 磁盘告警检查（阈值90%）
            if (resourceInfo.DiskUsage > 90)
            {
                _logger.LogWarning("磁盘使用率告警: {DiskUsage}%", resourceInfo.DiskUsage);
            }

            // 网络连接检查
            if (!resourceInfo.NetworkInfo.IsConnected)
            {
                _logger.LogWarning("网络连接异常");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "检查系统告警失败");
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void Dispose()
    {
        _dataCollectionTimer?.Dispose();
        _heartbeatTimer?.Dispose();
        _statusReportTimer?.Dispose();
        _configSyncTimer?.Dispose();
        base.Dispose();
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IoTDataCollection.Collector.Console.Services;

namespace IoTDataCollection.Collector.Console.Services;

/// <summary>
/// 采集器后台服务
/// </summary>
public class CollectorHostedService : BackgroundService
{
    private readonly ILogger<CollectorHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly CollectorConfiguration _config;
    private readonly Timer _heartbeatTimer;
    private readonly Timer _cleanupTimer;

    public CollectorHostedService(
        ILogger<CollectorHostedService> logger,
        IServiceProvider serviceProvider,
        IOptions<CollectorConfiguration> config)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _config = config.Value;
        _heartbeatTimer = new Timer(SendHeartbeat, null, Timeout.Infinite, Timeout.Infinite);
        _cleanupTimer = new Timer(CleanupExpiredData, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    /// 执行后台服务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("启动采集器后台服务");

            // 初始化采集服务
            using var scope = _serviceProvider.CreateScope();
            var collectorService = scope.ServiceProvider.GetRequiredService<ICollectorService>();

            if (!await collectorService.InitializeAsync(stoppingToken))
            {
                _logger.LogError("采集服务初始化失败");
                return;
            }

            // 启动心跳定时器
            _heartbeatTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(_config.HeartbeatIntervalSeconds));

            // 启动清理定时器（每天执行一次）
            _cleanupTimer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromHours(24));

            _logger.LogInformation("采集器后台服务启动完成");

            // 主循环
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 采集设备数据
                    await collectorService.CollectDeviceDataAsync(stoppingToken);

                    // 发送未发送的数据
                    await collectorService.SendUnsentDataAsync(stoppingToken);

                    // 等待下次采集
                    await Task.Delay(TimeSpan.FromSeconds(_config.SamplingIntervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "采集循环执行异常");
                    await Task.Delay(TimeSpan.FromSeconds(_config.RetryIntervalSeconds), stoppingToken);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "采集器后台服务执行失败");
        }
        finally
        {
            _logger.LogInformation("采集器后台服务停止");
        }
    }

    /// <summary>
    /// 停止后台服务
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("正在停止采集器后台服务");

        // 停止定时器
        _heartbeatTimer?.Change(Timeout.Infinite, Timeout.Infinite);
        _cleanupTimer?.Change(Timeout.Infinite, Timeout.Infinite);

        await base.StopAsync(cancellationToken);
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void Dispose()
    {
        _heartbeatTimer?.Dispose();
        _cleanupTimer?.Dispose();
        base.Dispose();
    }

    /// <summary>
    /// 发送心跳
    /// </summary>
    private async void SendHeartbeat(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var collectorService = scope.ServiceProvider.GetRequiredService<ICollectorService>();

            await collectorService.SendHeartbeatAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送心跳失败");
        }
    }

    /// <summary>
    /// 清理过期数据
    /// </summary>
    private async void CleanupExpiredData(object? state)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var collectorService = scope.ServiceProvider.GetRequiredService<ICollectorService>();

            await collectorService.CleanupExpiredDataAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "清理过期数据失败");
        }
    }
} 
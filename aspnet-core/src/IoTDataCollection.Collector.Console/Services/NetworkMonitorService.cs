using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;

namespace IoTDataCollection.Collector.Console.Services;

/// <summary>
/// 网络状态监控服务
/// </summary>
public class NetworkMonitorService : BackgroundService
{
    private readonly ILogger<NetworkMonitorService> _logger;
    private readonly ICommunicationService _communicationService;
    private readonly Timer _networkCheckTimer;
    private readonly Timer _statusReportTimer;
    private bool _lastConnectionState = false;
    private DateTime _lastConnectionChange = DateTime.UtcNow;
    private int _connectionLossCount = 0;
    private int _reconnectionCount = 0;

    public NetworkMonitorService(
        ILogger<NetworkMonitorService> logger,
        ICommunicationService communicationService)
    {
        _logger = logger;
        _communicationService = communicationService;
        _networkCheckTimer = new Timer(CheckNetworkStatus, null, Timeout.Infinite, Timeout.Infinite);
        _statusReportTimer = new Timer(ReportNetworkStatus, null, Timeout.Infinite, Timeout.Infinite);

        // 订阅连接状态变化事件
        _communicationService.ConnectionStatusChanged += OnConnectionStatusChanged;
    }

    /// <summary>
    /// 执行后台服务
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("启动网络状态监控服务");

            // 启动网络检查定时器（每30秒检查一次）
            _networkCheckTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30));

            // 启动状态报告定时器（每5分钟报告一次）
            _statusReportTimer.Change(TimeSpan.Zero, TimeSpan.FromMinutes(5));

            _logger.LogInformation("网络状态监控服务启动完成");

            // 等待取消信号
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            // 正常取消
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网络状态监控服务执行失败");
        }
    }

    /// <summary>
    /// 停止服务
    /// </summary>
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("停止网络状态监控服务");

            _networkCheckTimer?.Change(Timeout.Infinite, Timeout.Infinite);
            _statusReportTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            await base.StopAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止网络状态监控服务失败");
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public override void Dispose()
    {
        try
        {
            _communicationService.ConnectionStatusChanged -= OnConnectionStatusChanged;
            _networkCheckTimer?.Dispose();
            _statusReportTimer?.Dispose();
            base.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放网络状态监控服务资源失败");
        }
    }

    /// <summary>
    /// 连接状态变化事件处理
    /// </summary>
    private void OnConnectionStatusChanged(object? sender, ConnectionStatusChangedEventArgs e)
    {
        var currentState = e.IsConnected;
        var now = DateTime.UtcNow;

        if (currentState != _lastConnectionState)
        {
            if (currentState)
            {
                // 连接恢复
                _reconnectionCount++;
                var downtime = now - _lastConnectionChange;
                _logger.LogInformation("网络连接已恢复，断开持续时间: {Duration}, 重连次数: {ReconnectCount}", 
                    downtime.ToString(@"hh\:mm\:ss"), _reconnectionCount);
            }
            else
            {
                // 连接断开
                _connectionLossCount++;
                _logger.LogWarning("网络连接已断开: {Reason}, 断开次数: {LossCount}", 
                    e.ErrorMessage ?? "未知原因", _connectionLossCount);
            }

            _lastConnectionState = currentState;
            _lastConnectionChange = now;
        }
    }

    /// <summary>
    /// 检查网络状态
    /// </summary>
    private async void CheckNetworkStatus(object? state)
    {
        try
        {
            var isConnected = _communicationService.IsConnected;
            
            // 如果连接状态异常，记录详细信息
            if (!isConnected && _lastConnectionState)
            {
                _logger.LogWarning("网络状态检查发现连接异常，当前状态: {IsConnected}", isConnected);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网络状态检查失败");
        }
    }

    /// <summary>
    /// 报告网络状态
    /// </summary>
    private async void ReportNetworkStatus(object? state)
    {
        try
        {
            var uptime = DateTime.UtcNow - _lastConnectionChange;
            var isConnected = _communicationService.IsConnected;

            _logger.LogInformation("网络状态报告 - 连接状态: {IsConnected}, 当前状态持续时间: {Duration}, " +
                "总断开次数: {LossCount}, 总重连次数: {ReconnectCount}", 
                isConnected, uptime.ToString(@"hh\:mm\:ss"), _connectionLossCount, _reconnectionCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "网络状态报告失败");
        }
    }

    /// <summary>
    /// 获取网络统计信息
    /// </summary>
    public NetworkStatistics GetNetworkStatistics()
    {
        return new NetworkStatistics
        {
            IsConnected = _communicationService.IsConnected,
            LastConnectionChange = _lastConnectionChange,
            CurrentStateDuration = DateTime.UtcNow - _lastConnectionChange,
            ConnectionLossCount = _connectionLossCount,
            ReconnectionCount = _reconnectionCount,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// 网络统计信息
/// </summary>
public class NetworkStatistics
{
    /// <summary>
    /// 是否连接
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// 最后连接状态变化时间
    /// </summary>
    public DateTime LastConnectionChange { get; set; }

    /// <summary>
    /// 当前状态持续时间
    /// </summary>
    public TimeSpan CurrentStateDuration { get; set; }

    /// <summary>
    /// 连接断开次数
    /// </summary>
    public int ConnectionLossCount { get; set; }

    /// <summary>
    /// 重连次数
    /// </summary>
    public int ReconnectionCount { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }
} 
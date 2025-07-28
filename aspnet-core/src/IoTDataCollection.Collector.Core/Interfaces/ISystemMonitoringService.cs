using System;
using IoTDataCollection.Collector.Core.Services;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 系统监控服务接口
/// </summary>
public interface ISystemMonitoringService : IDisposable
{
    /// <summary>
    /// 开始监控
    /// </summary>
    void StartMonitoring();

    /// <summary>
    /// 停止监控
    /// </summary>
    void StopMonitoring();

    /// <summary>
    /// 获取当前系统资源信息
    /// </summary>
    /// <returns>系统资源信息</returns>
    SystemResourceInfo GetCurrentSystemResources();

    /// <summary>
    /// 系统资源采集事件
    /// </summary>
    event EventHandler<SystemResourcesCollectedEventArgs>? OnSystemResourcesCollected;
} 
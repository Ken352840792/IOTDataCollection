using System;
using System.Diagnostics;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using System.Linq;
using System.Runtime.InteropServices;

namespace IoTDataCollection.Collector.Core.Services;

/// <summary>
/// 系统监控服务
/// </summary>
public class SystemMonitoringService : ISystemMonitoringService
{
    private readonly ILogger<SystemMonitoringService> _logger;
    private readonly Timer _monitoringTimer;
    private readonly NetworkInterface[] _networkInterfaces;
    private SystemResourceInfo _lastResourceInfo;
    private DateTime _lastCollectionTime;
    private bool _isMonitoring = false;
    
    // CPU 监控相关字段
    private DateTime _lastCpuCheck = DateTime.UtcNow;
    private TimeSpan _lastCpuTime = TimeSpan.Zero;

    public SystemMonitoringService(ILogger<SystemMonitoringService> logger)
    {
        _logger = logger;
        
        // 获取网络接口
        _networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        
        // 创建监控定时器
        _monitoringTimer = new Timer(CollectSystemResources, null, Timeout.Infinite, Timeout.Infinite);
        
        // 初始化系统资源信息
        _lastResourceInfo = new SystemResourceInfo
        {
            Timestamp = DateTime.UtcNow,
            CpuUsage = 0,
            MemoryUsage = 0,
            DiskUsage = 0,
            NetworkInfo = new NetworkInfo(),
            ProcessCount = 0,
            SystemUptime = 0,
            Hostname = Environment.MachineName,
            OperatingSystem = Environment.OSVersion.ToString()
        };
        _lastCollectionTime = DateTime.UtcNow;
        
        _logger.LogInformation("系统监控服务初始化完成 - 跨平台兼容模式");
    }

    /// <summary>
    /// 开始监控
    /// </summary>
    public void StartMonitoring()
    {
        if (_isMonitoring)
        {
            _logger.LogWarning("系统监控已在运行中");
            return;
        }

        try
        {
            _isMonitoring = true;
            _monitoringTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(30)); // 30秒采集间隔
            _logger.LogInformation("系统监控已启动，采集间隔：30秒");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "启动系统监控失败");
            _isMonitoring = false;
        }
    }

    /// <summary>
    /// 停止监控
    /// </summary>
    public void StopMonitoring()
    {
        if (!_isMonitoring)
        {
            return;
        }

        try
        {
            _isMonitoring = false;
            _monitoringTimer.Change(Timeout.Infinite, Timeout.Infinite);
            _logger.LogInformation("系统监控已停止");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "停止系统监控失败");
        }
    }

    /// <summary>
    /// 获取当前系统资源信息
    /// </summary>
    public SystemResourceInfo GetCurrentSystemResources()
    {
        try
        {
            var resourceInfo = new SystemResourceInfo
            {
                Timestamp = DateTime.UtcNow,
                CpuUsage = GetCpuUsage(),
                MemoryUsage = GetMemoryUsage(),
                DiskUsage = GetDiskUsage(),
                NetworkInfo = GetNetworkInfo(),
                ProcessCount = Process.GetProcesses().Length,
                SystemUptime = GetSystemUptime(),
                Hostname = Environment.MachineName,
                OperatingSystem = Environment.OSVersion.ToString()
            };

            _lastResourceInfo = resourceInfo;
            _lastCollectionTime = DateTime.UtcNow;

            return resourceInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统资源信息失败");
            return _lastResourceInfo ?? new SystemResourceInfo
            {
                Timestamp = DateTime.UtcNow,
                CpuUsage = 0,
                MemoryUsage = 0,
                DiskUsage = 0,
                NetworkInfo = new NetworkInfo(),
                ProcessCount = 0,
                SystemUptime = 0,
                Hostname = Environment.MachineName,
                OperatingSystem = Environment.OSVersion.ToString()
            };
        }
    }

    /// <summary>
    /// 系统资源采集回调
    /// </summary>
    private void CollectSystemResources(object? state)
    {
        if (!_isMonitoring)
        {
            return;
        }

        try
        {
            var resourceInfo = GetCurrentSystemResources();
            
            // 触发资源采集事件
            OnSystemResourcesCollected?.Invoke(this, new SystemResourcesCollectedEventArgs
            {
                ResourceInfo = resourceInfo,
                CollectionTime = DateTime.UtcNow
            });

            _logger.LogDebug("系统资源采集完成 - CPU: {CpuUsage}%, 内存: {MemoryUsage}%, 磁盘: {DiskUsage}%", 
                resourceInfo.CpuUsage, resourceInfo.MemoryUsage, resourceInfo.DiskUsage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "系统资源采集失败");
        }
    }

    /// <summary>
    /// 获取CPU使用率 - 跨平台实现
    /// </summary>
    private double GetCpuUsage()
    {
        try
        {
            var currentTime = DateTime.UtcNow;
            var process = Process.GetCurrentProcess();
            var currentCpuTime = process.TotalProcessorTime;
            
            var timeDiff = (currentTime - _lastCpuCheck).TotalSeconds;
            var cpuTimeDiff = (currentCpuTime - _lastCpuTime).TotalSeconds;
            
            if (timeDiff > 0)
            {
                var cpuUsage = (cpuTimeDiff / timeDiff) * 100.0 / Environment.ProcessorCount;
                
                // 更新上次检查时间
                _lastCpuCheck = currentTime;
                _lastCpuTime = currentCpuTime;
                
                return Math.Round(Math.Min(cpuUsage, 100.0), 2);
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取CPU使用率失败");
            return 0;
        }
    }

    /// <summary>
    /// 获取内存使用率 - 跨平台实现
    /// </summary>
    private double GetMemoryUsage()
    {
        try
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // Windows 平台：使用 GC 信息
                var totalMemory = GC.GetTotalMemory(false);
                var gcMemoryInfo = GC.GetGCMemoryInfo();
                var maxMemory = gcMemoryInfo.TotalAvailableMemoryBytes;
                
                if (maxMemory > 0)
                {
                    return Math.Round((double)totalMemory / maxMemory * 100, 2);
                }
            }
            else
            {
                // Linux/Unix 平台：尝试读取 /proc/meminfo
                try
                {
                    if (File.Exists("/proc/meminfo"))
                    {
                        var memInfo = File.ReadAllText("/proc/meminfo");
                        var lines = memInfo.Split('\n');
                        
                        long totalMem = 0, freeMem = 0, availableMem = 0;
                        
                        foreach (var line in lines)
                        {
                            if (line.StartsWith("MemTotal:"))
                            {
                                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && long.TryParse(parts[1], out var total))
                                    totalMem = total * 1024; // 转换为字节
                            }
                            else if (line.StartsWith("MemAvailable:"))
                            {
                                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && long.TryParse(parts[1], out var available))
                                    availableMem = available * 1024; // 转换为字节
                            }
                            else if (line.StartsWith("MemFree:"))
                            {
                                var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length >= 2 && long.TryParse(parts[1], out var free))
                                    freeMem = free * 1024; // 转换为字节
                            }
                        }
                        
                        if (totalMem > 0)
                        {
                            var usedMem = totalMem - (availableMem > 0 ? availableMem : freeMem);
                            return Math.Round((double)usedMem / totalMem * 100, 2);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "读取 /proc/meminfo 失败，使用备用方法");
                }
            }
            
            // 备用方法：使用 GC 信息
            var processMemory = GC.GetTotalMemory(false);
            var gcInfo = GC.GetGCMemoryInfo();
            var availableMemory = gcInfo.TotalAvailableMemoryBytes;
            
            if (availableMemory > 0)
            {
                return Math.Round((double)processMemory / availableMemory * 100, 2);
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取内存使用率失败");
            return 0;
        }
    }

    /// <summary>
    /// 获取磁盘使用率 - 跨平台实现
    /// </summary>
    private double GetDiskUsage()
    {
        try
        {
            // 跨平台方法：使用 DriveInfo
            var rootPath = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                ? Path.GetPathRoot(Environment.SystemDirectory) ?? "C:"
                : "/";
                
            var driveInfo = new DriveInfo(rootPath);
            var totalSize = driveInfo.TotalSize;
            var availableSize = driveInfo.AvailableFreeSpace;
            var usedSize = totalSize - availableSize;
            
            return Math.Round((double)usedSize / totalSize * 100, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取磁盘使用率失败");
            return 0;
        }
    }

    /// <summary>
    /// 获取网络信息
    /// </summary>
    private NetworkInfo GetNetworkInfo()
    {
        try
        {
            var networkInfo = new NetworkInfo();
            var totalBytesReceived = 0L;
            var totalBytesSent = 0L;

            foreach (var networkInterface in _networkInterfaces)
            {
                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    var stats = networkInterface.GetIPv4Statistics();
                    totalBytesReceived += stats.BytesReceived;
                    totalBytesSent += stats.BytesSent;
                }
            }

            // 计算网络速度（KB/s）
            if (_lastResourceInfo?.NetworkInfo != null)
            {
                var timeDiff = (DateTime.UtcNow - _lastResourceInfo.Timestamp).TotalSeconds;
                if (timeDiff > 0)
                {
                    networkInfo.ReceiveSpeed = Math.Round((totalBytesReceived - _lastResourceInfo.NetworkInfo.TotalBytesReceived) / 1024.0 / timeDiff, 2);
                    networkInfo.SendSpeed = Math.Round((totalBytesSent - _lastResourceInfo.NetworkInfo.TotalBytesSent) / 1024.0 / timeDiff, 2);
                }
            }

            networkInfo.TotalBytesReceived = totalBytesReceived;
            networkInfo.TotalBytesSent = totalBytesSent;
            networkInfo.IsConnected = _networkInterfaces.Any(ni => ni.OperationalStatus == OperationalStatus.Up);

            return networkInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取网络信息失败");
            return new NetworkInfo();
        }
    }

    /// <summary>
    /// 获取系统运行时间（小时）
    /// </summary>
    private double GetSystemUptime()
    {
        try
        {
            var uptime = Environment.TickCount64 / 1000.0 / 3600.0; // 转换为小时
            return Math.Round(uptime, 2);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取系统运行时间失败");
            return 0;
        }
    }

    /// <summary>
    /// 系统资源采集事件
    /// </summary>
    public event EventHandler<SystemResourcesCollectedEventArgs>? OnSystemResourcesCollected;

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        StopMonitoring();
        _monitoringTimer?.Dispose();
    }
}

/// <summary>
/// 系统资源信息
/// </summary>
public class SystemResourceInfo
{
    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// CPU使用率（百分比）
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率（百分比）
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 磁盘使用率（百分比）
    /// </summary>
    public double DiskUsage { get; set; }

    /// <summary>
    /// 网络信息
    /// </summary>
    public NetworkInfo NetworkInfo { get; set; } = new();

    /// <summary>
    /// 进程数量
    /// </summary>
    public int ProcessCount { get; set; }

    /// <summary>
    /// 系统运行时间（小时）
    /// </summary>
    public double SystemUptime { get; set; }

    /// <summary>
    /// 主机名
    /// </summary>
    public string Hostname { get; set; } = string.Empty;

    /// <summary>
    /// 操作系统信息
    /// </summary>
    public string OperatingSystem { get; set; } = string.Empty;
}

/// <summary>
/// 网络信息
/// </summary>
public class NetworkInfo
{
    /// <summary>
    /// 网络连接状态
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// 接收速度（KB/s）
    /// </summary>
    public double ReceiveSpeed { get; set; }

    /// <summary>
    /// 发送速度（KB/s）
    /// </summary>
    public double SendSpeed { get; set; }

    /// <summary>
    /// 总接收字节数
    /// </summary>
    public long TotalBytesReceived { get; set; }

    /// <summary>
    /// 总发送字节数
    /// </summary>
    public long TotalBytesSent { get; set; }
}

/// <summary>
/// 系统资源采集事件参数
/// </summary>
public class SystemResourcesCollectedEventArgs : EventArgs
{
    /// <summary>
    /// 资源信息
    /// </summary>
    public SystemResourceInfo ResourceInfo { get; set; } = new();

    /// <summary>
    /// 采集时间
    /// </summary>
    public DateTime CollectionTime { get; set; }
} 
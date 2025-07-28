using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 配置同步服务接口
/// </summary>
public interface IConfigurationSyncService
{
    /// <summary>
    /// 从中心系统同步配置
    /// </summary>
    Task<List<DeviceConfig>> SyncConfigurationsFromCenterAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 测试配置同步连接
    /// </summary>
    Task<bool> TestSyncConnectionAsync(CancellationToken cancellationToken = default);
} 
using System;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 采集服务接口
/// </summary>
public interface ICollectorService
{
    /// <summary>
    /// 初始化服务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>初始化结果</returns>
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 停止服务
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>停止结果</returns>
    Task<bool> StopAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 采集数据
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>采集结果</returns>
    Task<bool> CollectDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送心跳
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>发送结果</returns>
    Task<bool> SendHeartbeatAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 报告状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>报告结果</returns>
    Task<bool> ReportStatusAsync(CancellationToken cancellationToken = default);
} 
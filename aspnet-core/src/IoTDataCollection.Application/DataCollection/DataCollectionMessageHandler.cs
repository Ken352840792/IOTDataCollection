using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using IoTDataCollection.TimeSeriesData;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// 数据采集消息处理服务实现
/// </summary>
public class DataCollectionMessageHandler : IDataCollectionMessageHandler, ITransientDependency
{
    private readonly ILogger<DataCollectionMessageHandler> _logger;
    private readonly ITimeSeriesRepository _timeSeriesRepository;

    public DataCollectionMessageHandler(
        ILogger<DataCollectionMessageHandler> logger,
        ITimeSeriesRepository timeSeriesRepository)
    {
        _logger = logger;
        _timeSeriesRepository = timeSeriesRepository;
    }

    public async Task<MessageProcessingResult> ProcessDeviceDataMessageAsync(DeviceDataMessage message, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new MessageProcessingResult();

        try
        {
            _logger.LogDebug("开始处理设备数据消息: {MessageId}, 设备: {DeviceCode}, 数据点数量: {DataPointCount}", 
                message.MessageId, message.DeviceCode, message.DataPoints.Count);

            // 验证消息
            if (string.IsNullOrEmpty(message.DeviceCode) || message.DataPoints.Count == 0)
            {
                result.Success = false;
                result.ErrorMessage = "消息验证失败：设备编码为空或数据点为空";
                return result;
            }

            // 转换数据点
            var deviceDataPoints = message.DataPoints.Select(dp => new DeviceDataPoint
            {
                DeviceCode = message.DeviceCode,
                PointCode = dp.PointCode,
                NumericValue = dp.NumericValue,
                StringValue = dp.StringValue,
                BooleanValue = dp.BooleanValue,
                Quality = dp.Quality,
                Timestamp = message.Timestamp,
                CollectorNode = message.CollectorNode
            }).ToList();

            // 批量写入InfluxDB
            await _timeSeriesRepository.WriteDeviceDataPointsAsync(deviceDataPoints, cancellationToken);

            result.Success = true;
            result.ProcessedDataPoints = deviceDataPoints.Count;

            _logger.LogInformation("设备数据消息处理成功: {MessageId}, 设备: {DeviceCode}, 处理数据点: {Count}", 
                message.MessageId, message.DeviceCode, result.ProcessedDataPoints);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "处理设备数据消息失败: {MessageId}, 设备: {DeviceCode}", 
                message.MessageId, message.DeviceCode);
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return result;
    }

    public async Task<MessageProcessingResult> ProcessCollectorStatusMessageAsync(CollectorStatusMessage message, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new MessageProcessingResult();

        try
        {
            _logger.LogDebug("开始处理采集端状态消息: {MessageId}, 节点: {CollectorNode}, 状态: {StatusType}", 
                message.MessageId, message.CollectorNode, message.StatusType);

            // 验证消息
            if (string.IsNullOrEmpty(message.CollectorNode))
            {
                result.Success = false;
                result.ErrorMessage = "消息验证失败：采集端节点标识为空";
                return result;
            }

            // 创建系统监控数据
            var monitoringData = new SystemMonitoringData
            {
                NodeCode = message.CollectorNode,
                Timestamp = message.Timestamp,
                NodeName = message.CollectorNode, // 使用节点标识作为节点名称
                NodeType = "collector", // 标记为采集端节点
                CpuUsage = message.SystemResources?.CpuUsage ?? 0,
                MemoryUsage = message.SystemResources?.MemoryUsage ?? 0,
                DiskUsage = message.SystemResources?.DiskUsage ?? 0,
                // 根据设备连接状态计算在线设备数量
                OnlineDevices = message.DeviceConnections.Count(d => d.Value),
                TotalDevices = message.DeviceConnections.Count,
                // 网络状态通过NetworkRx/NetworkTx来表示
                NetworkRx = message.SystemResources?.NetworkConnected == true ? 1.0 : 0.0,
                NetworkTx = message.SystemResources?.NetworkConnected == true ? 1.0 : 0.0
            };

            // 写入InfluxDB
            await _timeSeriesRepository.WriteSystemMonitoringDataAsync(monitoringData, cancellationToken);

            result.Success = true;
            result.ProcessedDataPoints = 1; // 状态消息算作1个数据点

            _logger.LogInformation("采集端状态消息处理成功: {MessageId}, 节点: {CollectorNode}, 状态: {StatusType}", 
                message.MessageId, message.CollectorNode, message.StatusType);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "处理采集端状态消息失败: {MessageId}, 节点: {CollectorNode}", 
                message.MessageId, message.CollectorNode);
        }
        finally
        {
            stopwatch.Stop();
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return result;
    }
} 
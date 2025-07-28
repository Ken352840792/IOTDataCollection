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
                // 使用统一的Value字段
                RawValue = GetRawValue(dp),
                CalculatedValue = GetCalculatedValue(dp),
                DataType = GetDataType(dp),
                Quality = dp.Quality,
                Timestamp = message.Timestamp,
                ProcessedTime = DateTime.UtcNow,
                CollectorNode = message.CollectorNode,
                Unit = dp.Unit, 
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

    /// <summary>
    /// 处理设备心跳消息
    /// </summary>
    public async Task<MessageProcessingResult> ProcessDeviceHeartbeatMessageAsync(DeviceHeartbeatMessage message, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new MessageProcessingResult();

        try
        {
            _logger.LogDebug("处理设备心跳消息: {DeviceCode}, 状态: {Status}", message.DeviceCode, message.Status);

            // 这里可以添加心跳消息的处理逻辑
            // 例如：更新设备状态、记录心跳时间、检查设备健康状态等

            result.Success = true;
            result.ProcessedDataPoints = 1; // 心跳消息算作一个处理项
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogDebug("设备心跳消息处理完成: {DeviceCode}, 耗时: {TimeMs}ms", 
                message.DeviceCode, result.ProcessingTimeMs);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            result.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            _logger.LogError(ex, "处理设备心跳消息失败: {DeviceCode}", message.DeviceCode);
        }

        return result;
    }

    #region 数据值转换辅助方法

    /// <summary>
    /// 获取原始值 - 从设备直接采集的原始值
    /// </summary>
    private string GetRawValue(DataPointMessage dp)
    {
        // 优先使用Value字段
        if (dp.Value != null)
        {
            return dp.Value.ToString();
        }
        
        // 回退到类型化字段
        if (dp.NumericValue.HasValue) return dp.NumericValue.Value.ToString();
        if (!string.IsNullOrEmpty(dp.StringValue)) return dp.StringValue;
        if (dp.BooleanValue.HasValue) return dp.BooleanValue.Value.ToString();
        
        return string.Empty;
    }

    /// <summary>
    /// 获取计算后的值 - 经过缩放和偏移计算的值
    /// </summary>
    private string GetCalculatedValue(DataPointMessage dp)
    {
        var rawValue = GetRawValue(dp);
        
        // 如果是数值类型，应用缩放和偏移
        if (double.TryParse(rawValue, out var numericValue))
        {
            // 这里可以从设备配置中获取实际的缩放因子和偏移量
            var scaleFactor = 1.0; // 默认缩放因子
            var offset = 0.0;      // 默认偏移量
            
            var calculatedValue = numericValue * scaleFactor + offset;
            return calculatedValue.ToString();
        }
        
        // 非数值类型直接返回原始值
        return rawValue;
    }

    /// <summary>
    /// 获取显示值 - 用于显示的值
    /// </summary>
    private string GetDisplayValue(DataPointMessage dp)
    {
        // 显示值通常与计算值相同，但可以添加单位等格式化
        var calculatedValue = GetCalculatedValue(dp);
        
        // 如果有单位，可以添加到显示值中
        if (!string.IsNullOrEmpty(dp.Unit))
        {
            return $"{calculatedValue} {dp.Unit}";
        }
        
        return calculatedValue;
    }

    /// <summary>
    /// 获取数据类型 - 记录原始数据类型
    /// </summary>
    private string GetDataType(DataPointMessage dp)
    {
        // 优先使用Value字段的类型
        if (dp.Value != null)
        {
            var type = dp.Value.GetType();
            if (type == typeof(int) || type == typeof(long) || type == typeof(short))
                return "INTEGER";
            else if (type == typeof(double) || type == typeof(float) || type == typeof(decimal))
                return "FLOAT";
            else if (type == typeof(bool))
                return "BOOLEAN";
            else if (type == typeof(string))
                return "STRING";
            else
                return "STRING"; // 默认类型
        }
        
        // 回退到类型化字段判断
        if (dp.NumericValue.HasValue) return "FLOAT";
        if (!string.IsNullOrEmpty(dp.StringValue)) return "STRING";
        if (dp.BooleanValue.HasValue) return "BOOLEAN";
        
        return "STRING"; // 默认类型
    }

    /// <summary>
    /// 获取采集对象 - 标识采集的物理对象
    /// </summary>
    private string GetCollectionObject(string deviceCode, string pointCode)
    {
        // 这里可以根据设备编码和数据点编码推断采集对象
        // 例如：PLC_001.TEMP_001 -> "温度传感器"
        // 暂时使用简单的映射逻辑
        
        if (pointCode.Contains("TEMP", StringComparison.OrdinalIgnoreCase))
        {
            return "温度传感器";
        }
        else if (pointCode.Contains("PRESSURE", StringComparison.OrdinalIgnoreCase))
        {
            return "压力传感器";
        }
        else if (pointCode.Contains("FLOW", StringComparison.OrdinalIgnoreCase))
        {
            return "流量计";
        }
        else if (pointCode.Contains("STATUS", StringComparison.OrdinalIgnoreCase))
        {
            return "状态信号";
        }
        else if (pointCode.Contains("ALARM", StringComparison.OrdinalIgnoreCase))
        {
            return "报警信号";
        }
        else
        {
            return "未知对象";
        }
    }

    #endregion
} 
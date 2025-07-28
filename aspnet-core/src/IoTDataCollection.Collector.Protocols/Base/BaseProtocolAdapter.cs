using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Protocols.Base;

/// <summary>
/// 协议适配器基类
/// </summary>
public abstract class BaseProtocolAdapter : IDeviceProtocol
{
    protected readonly ILogger<BaseProtocolAdapter> _logger;
    protected DeviceConfig? _currentConfig;
    protected bool _isConnected;

    protected BaseProtocolAdapter(ILogger<BaseProtocolAdapter> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// 协议名称
    /// </summary>
    public abstract string ProtocolName { get; }

    /// <summary>
    /// 连接状态
    /// </summary>
    public bool IsConnected => _isConnected;

    /// <summary>
    /// 设备状态变化事件
    /// </summary>
    public event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;

    /// <summary>
    /// 连接设备
    /// </summary>
    public virtual async Task<bool> ConnectAsync(DeviceConfig config, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("正在连接设备: {DeviceCode} ({ProtocolName})", config.DeviceCode, ProtocolName);
            
            _currentConfig = config;
            var result = await ConnectInternalAsync(config, cancellationToken);
            
            if (result)
            {
                _isConnected = true;
                OnStatusChanged(config.DeviceCode, DeviceStatus.Online);
                _logger.LogInformation("设备连接成功: {DeviceCode}", config.DeviceCode);
            }
            else
            {
                OnStatusChanged(config.DeviceCode, DeviceStatus.Error, "连接失败");
                _logger.LogError("设备连接失败: {DeviceCode}", config.DeviceCode);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设备连接异常: {DeviceCode}", config.DeviceCode);
            OnStatusChanged(config.DeviceCode, DeviceStatus.Error, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 断开连接
    /// </summary>
    public virtual async Task<bool> DisconnectAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_isConnected || _currentConfig == null)
            {
                return true;
            }

            _logger.LogInformation("正在断开设备连接: {DeviceCode}", _currentConfig.DeviceCode);
            
            var result = await DisconnectInternalAsync(cancellationToken);
            
            if (result)
            {
                _isConnected = false;
                OnStatusChanged(_currentConfig.DeviceCode, DeviceStatus.Offline);
                _logger.LogInformation("设备断开连接成功: {DeviceCode}", _currentConfig.DeviceCode);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设备断开连接异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 读取数据
    /// </summary>
    public virtual async Task<List<DataPoint>> ReadDataAsync(string[] addresses, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_isConnected || _currentConfig == null)
            {
                _logger.LogWarning("设备未连接，无法读取数据: {DeviceCode}", _currentConfig?.DeviceCode);
                return new List<DataPoint>();
            }

            _logger.LogDebug("正在读取数据: {DeviceCode}, 地址数量: {Count}", _currentConfig.DeviceCode, addresses.Length);
            
            var result = await ReadDataInternalAsync(addresses, cancellationToken);
            
            _logger.LogDebug("数据读取完成: {DeviceCode}, 数据点数量: {Count}", _currentConfig.DeviceCode, result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据读取异常: {DeviceCode}", _currentConfig?.DeviceCode);
            OnStatusChanged(_currentConfig?.DeviceCode ?? string.Empty, DeviceStatus.Error, ex.Message);
            return new List<DataPoint>();
        }
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    public virtual async Task<bool> WriteDataAsync(string address, object value, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_isConnected || _currentConfig == null)
            {
                _logger.LogWarning("设备未连接，无法写入数据: {DeviceCode}", _currentConfig?.DeviceCode);
                return false;
            }

            _logger.LogDebug("正在写入数据: {DeviceCode}, 地址: {Address}, 值: {Value}", 
                _currentConfig.DeviceCode, address, value);
            
            var result = await WriteDataInternalAsync(address, value, cancellationToken);
            
            if (result)
            {
                _logger.LogDebug("数据写入成功: {DeviceCode}, 地址: {Address}", _currentConfig.DeviceCode, address);
            }
            else
            {
                _logger.LogError("数据写入失败: {DeviceCode}, 地址: {Address}", _currentConfig.DeviceCode, address);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据写入异常: {DeviceCode}, 地址: {Address}", _currentConfig?.DeviceCode, address);
            OnStatusChanged(_currentConfig?.DeviceCode ?? string.Empty, DeviceStatus.Error, ex.Message);
            return false;
        }
    }

    /// <summary>
    /// 测试连接
    /// </summary>
    public virtual async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentConfig == null)
            {
                return false;
            }

            _logger.LogDebug("正在测试设备连接: {DeviceCode}", _currentConfig.DeviceCode);
            
            var result = await TestConnectionInternalAsync(cancellationToken);
            
            _logger.LogDebug("设备连接测试完成: {DeviceCode}, 结果: {Result}", _currentConfig.DeviceCode, result);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "设备连接测试异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 内部连接实现
    /// </summary>
    protected abstract Task<bool> ConnectInternalAsync(DeviceConfig config, CancellationToken cancellationToken);

    /// <summary>
    /// 内部断开连接实现
    /// </summary>
    protected abstract Task<bool> DisconnectInternalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 内部读取数据实现
    /// </summary>
    protected abstract Task<List<DataPoint>> ReadDataInternalAsync(string[] addresses, CancellationToken cancellationToken);

    /// <summary>
    /// 内部写入数据实现
    /// </summary>
    protected abstract Task<bool> WriteDataInternalAsync(string address, object value, CancellationToken cancellationToken);

    /// <summary>
    /// 内部测试连接实现
    /// </summary>
    protected abstract Task<bool> TestConnectionInternalAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 触发状态变化事件
    /// </summary>
    protected virtual void OnStatusChanged(string deviceCode, DeviceStatus status, string? errorMessage = null)
    {
        StatusChanged?.Invoke(this, new DeviceStatusChangedEventArgs
        {
            DeviceCode = deviceCode,
            Status = status,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// 创建数据点
    /// </summary>
    protected virtual DataPoint CreateDataPoint(string deviceCode, string pointCode, string pointName, 
        string address, DataType dataType, object value, string? unit = null)
    {
        var dataPoint = new DataPoint
        {
            Id = Guid.NewGuid().ToString(),
            DeviceCode = deviceCode,
            PointCode = pointCode,
            PointName = pointName,
            Address = address,
            DataType = dataType,
            Unit = unit,
            Timestamp = DateTime.UtcNow,
            Quality = 192
        };

        // 根据数据类型设置对应的值
        switch (dataType)
        {
            case DataType.Bool:
                dataPoint.BooleanValue = Convert.ToBoolean(value);
                dataPoint.RawValue = value.ToString();
                break;
            case DataType.Int16:
            case DataType.Int32:
            case DataType.Float:
                dataPoint.NumericValue = Convert.ToDouble(value);
                dataPoint.RawValue = value.ToString();
                break;
            case DataType.String:
                dataPoint.StringValue = value.ToString();
                dataPoint.RawValue = value.ToString();
                break;
        }

        return dataPoint;
    }
} 
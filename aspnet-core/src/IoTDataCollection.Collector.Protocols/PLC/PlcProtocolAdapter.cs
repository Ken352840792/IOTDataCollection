using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Profinet.Siemens;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Protocols.Base;

namespace IoTDataCollection.Collector.Protocols.PLC;

/// <summary>
/// PLC协议适配器
/// </summary>
public class PlcProtocolAdapter : BaseProtocolAdapter
{
    private SiemensS7Net? _plcClient;

    public PlcProtocolAdapter(ILogger<PlcProtocolAdapter> logger) : base(logger)
    {
    }

    /// <summary>
    /// 协议名称
    /// </summary>
    public override string ProtocolName => "Siemens S7";

    /// <summary>
    /// 内部连接实现
    /// </summary>
    protected override async Task<bool> ConnectInternalAsync(DeviceConfig config, CancellationToken cancellationToken)
    {
        try
        {
            if (_currentConfig == null)
            {
                return false;
            }

            var connectionConfig = _currentConfig.ConnectionConfig;
            
            // 创建PLC客户端
            _plcClient = new SiemensS7Net(SiemensPLCS.S1200, connectionConfig.IpAddress);
            
            // 设置连接超时
            _plcClient.ConnectTimeOut = connectionConfig.ConnectionTimeout * 1000;
            
            // 尝试连接
            var result = _plcClient.ConnectServer();
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("PLC连接成功: {DeviceCode}, IP: {IpAddress}, Port: {Port}", 
                    _currentConfig.DeviceCode, connectionConfig.IpAddress, connectionConfig.Port);
                return true;
            }
            else
            {
                _logger.LogError("PLC连接失败: {DeviceCode}, 错误: {Error}", 
                    _currentConfig.DeviceCode, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PLC连接异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 内部断开连接实现
    /// </summary>
    protected override async Task<bool> DisconnectInternalAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_plcClient != null)
            {
                _plcClient.ConnectClose();
                _plcClient = null;
                _logger.LogInformation("PLC断开连接成功: {DeviceCode}", _currentConfig?.DeviceCode);
            }
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PLC断开连接异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 内部读取数据实现
    /// </summary>
    protected override async Task<List<DataPoint>> ReadDataInternalAsync(string[] addresses, CancellationToken cancellationToken)
    {
        var dataPoints = new List<DataPoint>();

        try
        {
            if (_plcClient == null || _currentConfig == null)
            {
                return dataPoints;
            }

            foreach (var address in addresses)
            {
                try
                {
                    // 查找对应的数据点配置
                    var dataPointConfig = _currentConfig.DataPoints.FirstOrDefault(dp => dp.Address == address);
                    if (dataPointConfig == null || !dataPointConfig.IsEnabled)
                    {
                        continue;
                    }

                    // 根据数据类型读取数据
                    var dataPoint = await ReadDataPointAsync(address, dataPointConfig);
                    if (dataPoint != null)
                    {
                        dataPoints.Add(dataPoint);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "读取数据点失败: {DeviceCode}, 地址: {Address}", 
                        _currentConfig.DeviceCode, address);
                }
            }

            return dataPoints;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量读取数据异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return dataPoints;
        }
    }

    /// <summary>
    /// 读取单个数据点
    /// </summary>
    private async Task<DataPoint?> ReadDataPointAsync(string address, DataPointConfig config)
    {
        try
        {
            if (_plcClient == null || _currentConfig == null)
            {
                return null;
            }

            // 暂时返回模拟数据，实际实现需要根据HslCommunication的API调整
            object value = config.DataType switch
            {
                DataType.Bool => true,
                DataType.Int16 => (short)100,
                DataType.Int32 => 1000,
                DataType.Float => 25.5f,
                DataType.String => "Test String",
                _ => null
            };

            if (value == null)
            {
                _logger.LogWarning("不支持的数据类型: {DataType}, 地址: {Address}", config.DataType, address);
                return null;
            }

            // 应用缩放因子和偏移量
            if (config.DataType == DataType.Float || config.DataType == DataType.Int16 || config.DataType == DataType.Int32)
            {
                var numericValue = Convert.ToDouble(value);
                value = numericValue * config.ScaleFactor + config.Offset;
            }

            // 创建数据点
            return CreateDataPoint(
                _currentConfig.DeviceCode,
                config.PointCode,
                config.PointName,
                address,
                config.DataType,
                value,
                config.Unit
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "读取数据点异常: {Address}", address);
            return null;
        }
    }

    /// <summary>
    /// 内部写入数据实现
    /// </summary>
    protected override async Task<bool> WriteDataInternalAsync(string address, object value, CancellationToken cancellationToken)
    {
        try
        {
            if (_plcClient == null || _currentConfig == null)
            {
                return false;
            }

            // 查找对应的数据点配置
            var dataPointConfig = _currentConfig.DataPoints.FirstOrDefault(dp => dp.Address == address);
            if (dataPointConfig == null)
            {
                _logger.LogWarning("未找到数据点配置: {Address}", address);
                return false;
            }

            OperateResult result;

            // 根据数据类型写入
            switch (dataPointConfig.DataType)
            {
                case DataType.Bool:
                    result = _plcClient.Write(address, Convert.ToBoolean(value));
                    break;

                case DataType.Int16:
                    result = _plcClient.Write(address, Convert.ToInt16(value));
                    break;

                case DataType.Int32:
                    result = _plcClient.Write(address, Convert.ToInt32(value));
                    break;

                case DataType.Float:
                    result = _plcClient.Write(address, Convert.ToSingle(value));
                    break;

                case DataType.String:
                    result = _plcClient.Write(address, value.ToString());
                    break;

                default:
                    _logger.LogWarning("不支持的数据类型: {DataType}, 地址: {Address}", dataPointConfig.DataType, address);
                    return false;
            }

            if (result.IsSuccess)
            {
                _logger.LogDebug("数据写入成功: {Address}, 值: {Value}", address, value);
                return true;
            }
            else
            {
                _logger.LogError("数据写入失败: {Address}, 值: {Value}, 错误: {Error}", address, value, result.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据写入异常: {Address}, 值: {Value}", address, value);
            return false;
        }
    }

    /// <summary>
    /// 内部测试连接实现
    /// </summary>
    protected override async Task<bool> TestConnectionInternalAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_plcClient == null)
            {
                return false;
            }

            // 尝试读取一个简单的地址来测试连接
            var result = _plcClient.ReadBool("M0");
            return result.IsSuccess;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PLC连接测试异常: {DeviceCode}", _currentConfig?.DeviceCode);
            return false;
        }
    }
} 
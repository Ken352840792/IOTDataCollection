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
/// PLC协议适配器基类
/// </summary>
public abstract class PlcProtocolAdapter : BaseProtocolAdapter
{
    protected SiemensS7Net? _plcClient;

    protected PlcProtocolAdapter(ILogger<PlcProtocolAdapter> logger) : base(logger)
    {
    }

    /// <summary>
    /// 协议名称
    /// </summary>
    public override string ProtocolName => GetProtocolName();

    /// <summary>
    /// 获取协议名称
    /// </summary>
    protected abstract string GetProtocolName();

    /// <summary>
    /// 获取PLC类型
    /// </summary>
    protected abstract SiemensPLCS GetPlcType();

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
            _plcClient = new SiemensS7Net(GetPlcType(), connectionConfig.IpAddress);
            
            // 设置连接超时
            _plcClient.ConnectTimeOut = connectionConfig.ConnectionTimeout * 1000;
            
            // 尝试连接
            var result = _plcClient.ConnectServer();
            
            if (result.IsSuccess)
            {
                _logger.LogInformation("PLC连接成功: {DeviceCode}, IP: {IpAddress}, Port: {Port}, 型号: {PlcType}", 
                    _currentConfig.DeviceCode, connectionConfig.IpAddress, connectionConfig.Port, GetPlcType());
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

            object value;

            // 根据数据类型使用HslCommunication读取数据
            switch (config.DataType)
            {
                case DataType.Bool:
                    var boolResult = _plcClient.ReadBool(address);
                    if (boolResult.IsSuccess)
                    {
                        value = boolResult.Content;
                    }
                    else
                    {
                        _logger.LogWarning("读取布尔值失败: {Address}, 错误: {Error}", address, boolResult.Message);
                        return null;
                    }
                    break;

                case DataType.Int16:
                    var int16Result = _plcClient.ReadInt16(address);
                    if (int16Result.IsSuccess)
                    {
                        value = int16Result.Content;
                    }
                    else
                    {
                        _logger.LogWarning("读取Int16失败: {Address}, 错误: {Error}", address, int16Result.Message);
                        return null;
                    }
                    break;

                case DataType.Int32:
                    var int32Result = _plcClient.ReadInt32(address);
                    if (int32Result.IsSuccess)
                    {
                        value = int32Result.Content;
                    }
                    else
                    {
                        _logger.LogWarning("读取Int32失败: {Address}, 错误: {Error}", address, int32Result.Message);
                        return null;
                    }
                    break;

                case DataType.Float:
                    var floatResult = _plcClient.ReadFloat(address);
                    if (floatResult.IsSuccess)
                    {
                        value = floatResult.Content;
                    }
                    else
                    {
                        _logger.LogWarning("读取Float失败: {Address}, 错误: {Error}", address, floatResult.Message);
                        return null;
                    }
                    break;

                case DataType.String:
                    var stringResult = _plcClient.ReadString(address, 50); // 读取50个字符
                    if (stringResult.IsSuccess)
                    {
                        value = stringResult.Content;
                    }
                    else
                    {
                        _logger.LogWarning("读取String失败: {Address}, 错误: {Error}", address, stringResult.Message);
                        return null;
                    }
                    break;

                default:
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

/// <summary>
/// Siemens S7-1200 PLC适配器
/// </summary>
public class SiemensS7_1200Adapter : PlcProtocolAdapter
{
    public SiemensS7_1200Adapter(ILogger<SiemensS7_1200Adapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-1200";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S1200;
}

/// <summary>
/// Siemens S7-1500 PLC适配器
/// </summary>
public class SiemensS7_1500Adapter : PlcProtocolAdapter
{
    public SiemensS7_1500Adapter(ILogger<SiemensS7_1500Adapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-1500";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S1500;
}

/// <summary>
/// Siemens S7-200 PLC适配器
/// </summary>
public class SiemensS7_200Adapter : PlcProtocolAdapter
{
    public SiemensS7_200Adapter(ILogger<SiemensS7_200Adapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-200";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S200;
}

/// <summary>
/// Siemens S7-200Smart PLC适配器
/// </summary>
public class SiemensS7_200SmartAdapter : PlcProtocolAdapter
{
    public SiemensS7_200SmartAdapter(ILogger<SiemensS7_200SmartAdapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-200Smart";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S200Smart;
}

/// <summary>
/// Siemens S7-300 PLC适配器
/// </summary>
public class SiemensS7_300Adapter : PlcProtocolAdapter
{
    public SiemensS7_300Adapter(ILogger<SiemensS7_300Adapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-300";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S300;
}

/// <summary>
/// Siemens S7-400 PLC适配器
/// </summary>
public class SiemensS7_400Adapter : PlcProtocolAdapter
{
    public SiemensS7_400Adapter(ILogger<SiemensS7_400Adapter> logger) : base(logger)
    {
    }

    protected override string GetProtocolName() => "Siemens S7-400";

    protected override SiemensPLCS GetPlcType() => SiemensPLCS.S400;
} 
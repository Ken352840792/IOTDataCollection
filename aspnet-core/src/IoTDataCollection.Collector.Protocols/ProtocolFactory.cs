using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Protocols.PLC;

namespace IoTDataCollection.Collector.Protocols;

/// <summary>
/// 协议工厂
/// </summary>
public class ProtocolFactory : IProtocolFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ProtocolFactory> _logger;

    public ProtocolFactory(IServiceProvider serviceProvider, ILogger<ProtocolFactory> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// 创建协议适配器
    /// </summary>
    /// <param name="deviceType">设备类型</param>
    /// <returns>协议适配器</returns>
    public IDeviceProtocol CreateProtocol(DeviceType deviceType)
    {
        try
        {
            _logger.LogDebug("创建协议适配器: {DeviceType}", deviceType);

            return deviceType switch
            {
                DeviceType.PLC => _serviceProvider.GetRequiredService<SiemensS7_1200Adapter>(), // 默认使用S7-1200
                DeviceType.Modbus => throw new NotSupportedException($"设备类型 {deviceType} 暂不支持"),
                DeviceType.OPCUA => throw new NotSupportedException($"设备类型 {deviceType} 暂不支持"),
                DeviceType.Socket => throw new NotSupportedException($"设备类型 {deviceType} 暂不支持"),
                _ => throw new ArgumentException($"未知的设备类型: {deviceType}")
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "创建协议适配器失败: {DeviceType}", deviceType);
            throw;
        }
    }

    /// <summary>
    /// 创建协议适配器（根据设备配置）
    /// </summary>
    /// <param name="deviceConfig">设备配置</param>
    /// <returns>协议适配器</returns>
    public IDeviceProtocol CreateProtocol(DeviceConfig deviceConfig)
    {
        try
        {
            _logger.LogDebug("根据设备配置创建协议适配器: {DeviceCode}, 设备类型: {DeviceType}", 
                deviceConfig.DeviceCode, deviceConfig.DeviceType);

            // 如果是PLC设备，根据设备名称选择具体的适配器
            if (deviceConfig.DeviceType == DeviceType.PLC)
            {
                return CreatePlcAdapter(deviceConfig);
            }

            // 其他设备类型使用通用方法
            return CreateProtocol(deviceConfig.DeviceType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "根据设备配置创建协议适配器失败: {DeviceCode}", deviceConfig.DeviceCode);
            throw;
        }
    }

    /// <summary>
    /// 创建PLC适配器
    /// </summary>
    /// <param name="deviceConfig">设备配置</param>
    /// <returns>PLC协议适配器</returns>
    private IDeviceProtocol CreatePlcAdapter(DeviceConfig deviceConfig)
    {
        // 根据设备名称判断PLC型号
        var deviceName = deviceConfig.DeviceName?.ToUpper() ?? "";

        // 根据设备名称判断
        if (deviceName.Contains("S7-1200") || deviceName.Contains("1200"))
        {
            _logger.LogDebug("创建S7-1200适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_1200Adapter>();
        }
        else if (deviceName.Contains("S7-1500") || deviceName.Contains("1500"))
        {
            _logger.LogDebug("创建S7-1500适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_1500Adapter>();
        }
        else if (deviceName.Contains("S7-200") && deviceName.Contains("SMART"))
        {
            _logger.LogDebug("创建S7-200Smart适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_200SmartAdapter>();
        }
        else if (deviceName.Contains("S7-200") || deviceName.Contains("200"))
        {
            _logger.LogDebug("创建S7-200适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_200Adapter>();
        }
        else if (deviceName.Contains("S7-300") || deviceName.Contains("300"))
        {
            _logger.LogDebug("创建S7-300适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_300Adapter>();
        }
        else if (deviceName.Contains("S7-400") || deviceName.Contains("400"))
        {
            _logger.LogDebug("创建S7-400适配器: {DeviceCode}", deviceConfig.DeviceCode);
            return _serviceProvider.GetRequiredService<SiemensS7_400Adapter>();
        }

        // 默认使用S7-1200适配器
        _logger.LogWarning("无法确定PLC型号，使用默认S7-1200适配器: {DeviceCode}, 设备名称: {DeviceName}", 
            deviceConfig.DeviceCode, deviceConfig.DeviceName);
        return _serviceProvider.GetRequiredService<SiemensS7_1200Adapter>();
    }
}

/// <summary>
/// 协议工厂接口
/// </summary>
public interface IProtocolFactory
{
    /// <summary>
    /// 创建协议适配器
    /// </summary>
    /// <param name="deviceType">设备类型</param>
    /// <returns>协议适配器</returns>
    IDeviceProtocol CreateProtocol(DeviceType deviceType);

    /// <summary>
    /// 创建协议适配器（根据设备配置）
    /// </summary>
    /// <param name="deviceConfig">设备配置</param>
    /// <returns>协议适配器</returns>
    IDeviceProtocol CreateProtocol(DeviceConfig deviceConfig);
} 
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
                DeviceType.PLC => _serviceProvider.GetRequiredService<PlcProtocolAdapter>(),
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
        return CreateProtocol(deviceConfig.DeviceType);
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IoTDataCollection.Devices;
using IoTDataCollection.Monitoring;
using IoTDataCollection.DataPoints;

namespace IoTDataCollection.Application.Devices;

/// <summary>
/// 设备配置应用服务
/// </summary>
public class DeviceConfigurationAppService : ApplicationService, IDeviceConfigurationAppService
{
    private readonly IRepository<Device, Guid> _deviceRepository;
    private readonly IRepository<CollectorNode, Guid> _collectorNodeRepository;
    private readonly IDataPointRepository _dataPointRepository;
    private readonly ILogger<DeviceConfigurationAppService> _logger;

    public DeviceConfigurationAppService(
        IRepository<Device, Guid> deviceRepository,
        IRepository<CollectorNode, Guid> collectorNodeRepository,
        IDataPointRepository dataPointRepository,
        ILogger<DeviceConfigurationAppService> logger)
    {
        _deviceRepository = deviceRepository;
        _collectorNodeRepository = collectorNodeRepository;
        _dataPointRepository = dataPointRepository;
        _logger = logger;
    }

    /// <summary>
    /// 获取设备配置同步数据
    /// </summary>
    public async Task<DeviceConfigurationSyncResponse> GetDeviceConfigurationsForSyncAsync(
        DeviceConfigurationSyncRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("开始获取设备配置同步数据，采集端节点: {CollectorNodeCode}", request.CollectorNodeCode);

            // 验证采集端节点是否存在
            var collectorNode = await _collectorNodeRepository.FirstOrDefaultAsync(
                x => x.F_NodeCode == request.CollectorNodeCode, cancellationToken);

            if (collectorNode == null)
            {
                return new DeviceConfigurationSyncResponse
                {
                    Success = false,
                    ErrorMessage = $"采集端节点不存在: {request.CollectorNodeCode}"
                };
            }

            // 获取该采集端节点下的所有设备
            var devices = await _deviceRepository.GetListAsync(
                x => x.F_IsCollectionEnabled && x.F_CollectorNodeCode == request.CollectorNodeCode, 
                cancellationToken: cancellationToken);

            var deviceConfigurations = new List<DeviceConfigurationDto>();

            foreach (var device in devices)
            {
                var deviceConfig = new DeviceConfigurationDto
                {
                    Id = device.Id,
                    DeviceCode = device.F_DeviceCode,
                    DeviceName = device.F_DeviceName,
                    DeviceType = device.F_DeviceType ?? string.Empty,
                    CommunicationProtocol = device.F_CommunicationProtocol ?? string.Empty,
                    IpAddress = device.F_IpAddress ?? string.Empty,
                    Port = device.F_Port ?? 0,
                    Username = null, // Device实体中没有这些属性，暂时设为null
                    Password = null,
                    ConnectionTimeout = device.F_Timeout / 1000, // 转换为秒
                    ReadTimeout = 10, // 默认值
                    WriteTimeout = 10, // 默认值
                    SamplingInterval = device.F_CollectionInterval / 1000, // 转换为秒
                    IsEnabled = device.F_IsCollectionEnabled,
                    CreationTime = device.CreationTime,
                    LastModificationTime = device.LastModificationTime,
                    DataPoints = new List<DataPointConfigurationDto>()
                };

                // 获取设备的数据点配置
                var dataPoints = await _dataPointRepository.GetListByDeviceAsync(
                    device.Id,
                    includeDetails: true,
                    cancellationToken: cancellationToken);

                // 只获取启用采集的数据点
                var enabledDataPoints = dataPoints.Where(dp => dp.F_IsCollectionEnabled).ToList();

                foreach (var dataPoint in enabledDataPoints)
                {
                    deviceConfig.DataPoints.Add(new DataPointConfigurationDto
                    {
                        PointCode = dataPoint.F_PointCode,
                        PointName = dataPoint.F_PointName,
                        Address = dataPoint.F_RegisterAddress ?? string.Empty,
                        DataType = dataPoint.F_DataType,
                        Unit = dataPoint.F_Unit ?? string.Empty,
                        ScaleFactor = dataPoint.F_ScaleFactor,
                        Offset = dataPoint.F_Offset,
                        IsEnabled = dataPoint.F_IsCollectionEnabled,
                        Description = dataPoint.F_Description ?? string.Empty
                    });
                }

                deviceConfigurations.Add(deviceConfig);
            }

            var response = new DeviceConfigurationSyncResponse
            {
                Success = true,
                DeviceConfigurations = deviceConfigurations,
                ConfigVersion = DateTime.UtcNow.ToString("yyyyMMddHHmmss"),
                SyncTime = DateTime.UtcNow,
                HasChanges = deviceConfigurations.Count > 0
            };

            _logger.LogInformation("设备配置同步数据获取成功，设备数量: {DeviceCount}", deviceConfigurations.Count);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备配置同步数据失败，采集端节点: {CollectorNodeCode}", request.CollectorNodeCode);

            return new DeviceConfigurationSyncResponse
            {
                Success = false,
                ErrorMessage = $"获取设备配置同步数据失败: {ex.Message}"
            };
        }
    }

    /// <summary>
    /// 更新设备配置
    /// </summary>
    public async Task<bool> UpdateDeviceConfigurationAsync(
        DeviceConfigurationUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("开始更新设备配置，设备编码: {DeviceCode}", request.Configuration.DeviceCode);

            // 查找现有设备
            var existingDevice = await _deviceRepository.FirstOrDefaultAsync(
                x => x.F_DeviceCode == request.Configuration.DeviceCode, cancellationToken);

            if (existingDevice == null)
            {
                _logger.LogWarning("设备不存在，设备编码: {DeviceCode}", request.Configuration.DeviceCode);
                return false;
            }

            // 更新设备信息
            existingDevice.F_DeviceName = request.Configuration.DeviceName;
            existingDevice.F_DeviceType = request.Configuration.DeviceType;
            existingDevice.F_CommunicationProtocol = request.Configuration.CommunicationProtocol;
            existingDevice.F_IpAddress = request.Configuration.IpAddress;
            existingDevice.F_Port = request.Configuration.Port;
            // 注意：Device实体中没有这些属性，暂时注释掉
            // existingDevice.F_Username = request.Configuration.Username;
            // existingDevice.F_Password = request.Configuration.Password;
            // existingDevice.F_ConnectionTimeout = request.Configuration.ConnectionTimeout;
            // existingDevice.F_ReadTimeout = request.Configuration.ReadTimeout;
            // existingDevice.F_WriteTimeout = request.Configuration.WriteTimeout;
            // existingDevice.F_SamplingInterval = request.Configuration.SamplingInterval;
            existingDevice.F_Timeout = request.Configuration.ConnectionTimeout * 1000; // 转换为毫秒
            existingDevice.F_CollectionInterval = request.Configuration.SamplingInterval * 1000; // 转换为毫秒
            existingDevice.F_IsCollectionEnabled = request.Configuration.IsEnabled;
            // existingDevice.F_CollectorNodeCode = request.CollectorNodeCode;

            await _deviceRepository.UpdateAsync(existingDevice, cancellationToken: cancellationToken);

            _logger.LogInformation("设备配置更新成功，设备编码: {DeviceCode}", request.Configuration.DeviceCode);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "更新设备配置失败，设备编码: {DeviceCode}", request.Configuration.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 获取设备配置列表
    /// </summary>
    public async Task<List<DeviceConfigurationDto>> GetDeviceConfigurationsAsync(
        string? collectorNodeCode = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 注意：Device实体目前没有F_CollectorNodeCode属性，暂时获取所有启用的设备
            var devices = await _deviceRepository.GetListAsync(
                x => x.F_IsCollectionEnabled, 
                cancellationToken: cancellationToken);

            var deviceConfigurations = new List<DeviceConfigurationDto>();

            foreach (var device in devices)
            {
                var deviceConfig = new DeviceConfigurationDto
                {
                    Id = device.Id,
                    DeviceCode = device.F_DeviceCode,
                    DeviceName = device.F_DeviceName,
                    DeviceType = device.F_DeviceType ?? string.Empty,
                    CommunicationProtocol = device.F_CommunicationProtocol ?? string.Empty,
                    IpAddress = device.F_IpAddress ?? string.Empty,
                    Port = device.F_Port ?? 0,
                    Username = null, // Device实体中没有这些属性，暂时设为null
                    Password = null,
                    ConnectionTimeout = device.F_Timeout / 1000, // 转换为秒
                    ReadTimeout = 10, // 默认值
                    WriteTimeout = 10, // 默认值
                    SamplingInterval = device.F_CollectionInterval / 1000, // 转换为秒
                    IsEnabled = device.F_IsCollectionEnabled,
                    CreationTime = device.CreationTime,
                    LastModificationTime = device.LastModificationTime,
                    DataPoints = new List<DataPointConfigurationDto>()
                };

                // 获取设备的数据点配置
                var dataPoints = await _dataPointRepository.GetListByDeviceAsync(
                    device.Id,
                    includeDetails: true,
                    cancellationToken: cancellationToken);

                // 只获取启用采集的数据点
                var enabledDataPoints = dataPoints.Where(dp => dp.F_IsCollectionEnabled).ToList();

                foreach (var dataPoint in enabledDataPoints)
                {
                    // 构建寄存器地址字符串
                    var address = dataPoint.F_RegisterAddress?.ToString() ?? string.Empty;

                    deviceConfig.DataPoints.Add(new DataPointConfigurationDto
                    {
                        PointCode = dataPoint.F_PointCode,
                        PointName = dataPoint.F_PointName,
                        Address = address,
                        DataType = dataPoint.F_DataType,
                        Unit = dataPoint.F_Unit ?? string.Empty,
                        ScaleFactor = dataPoint.F_ScaleFactor,
                        Offset = dataPoint.F_Offset,
                        IsEnabled = dataPoint.F_IsCollectionEnabled,
                        Description = dataPoint.F_Description ?? string.Empty
                    });
                }

                deviceConfigurations.Add(deviceConfig);
            }

            return deviceConfigurations;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备配置列表失败");
            return new List<DeviceConfigurationDto>();
        }
    }
}

/// <summary>
/// 设备配置应用服务接口
/// </summary>
public interface IDeviceConfigurationAppService
{
    /// <summary>
    /// 获取设备配置同步数据
    /// </summary>
    Task<DeviceConfigurationSyncResponse> GetDeviceConfigurationsForSyncAsync(
        DeviceConfigurationSyncRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新设备配置
    /// </summary>
    Task<bool> UpdateDeviceConfigurationAsync(
        DeviceConfigurationUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备配置列表
    /// </summary>
    Task<List<DeviceConfigurationDto>> GetDeviceConfigurationsAsync(
        string? collectorNodeCode = null,
        CancellationToken cancellationToken = default);
} 
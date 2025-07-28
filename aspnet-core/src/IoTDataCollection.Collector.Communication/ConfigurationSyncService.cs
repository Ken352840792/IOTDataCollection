using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Communication;

/// <summary>
/// 配置同步服务
/// </summary>
public class ConfigurationSyncService : IConfigurationSyncService
{
    private readonly ILogger<ConfigurationSyncService> _logger;
    private readonly HttpClientService _httpClientService;
    private readonly IStorageService _storageService;
    private readonly string _collectorNodeCode;

    public ConfigurationSyncService(
        ILogger<ConfigurationSyncService> logger,
        HttpClientService httpClientService,
        IStorageService storageService,
        string collectorNodeCode)
    {
        _logger = logger;
        _httpClientService = httpClientService;
        _storageService = storageService;
        _collectorNodeCode = collectorNodeCode;
    }

    /// <summary>
    /// 从中心系统同步配置
    /// </summary>
    public async Task<List<DeviceConfig>> SyncConfigurationsFromCenterAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("开始从中心系统同步配置，采集端节点: {CollectorNodeCode}", _collectorNodeCode);

            // 测试HTTP连接
            if (!await _httpClientService.TestConnectionAsync(cancellationToken))
            {
                _logger.LogWarning("无法连接到中心系统，跳过配置同步");
                return new List<DeviceConfig>();
            }

            // 构建同步请求
            var syncRequest = new
            {
                CollectorNodeCode = _collectorNodeCode,
                LastSyncTime = DateTime.UtcNow.AddHours(-1), // 获取最近1小时的配置变更
                ConfigVersion = string.Empty
            };

            // 调用中心系统API
            var response = await _httpClientService.PostAsync<dynamic>(
                "api/device-configurations/sync", 
                syncRequest, 
                cancellationToken);

            if (response == null)
            {
                _logger.LogError("从中心系统获取配置失败");
                return new List<DeviceConfig>();
            }

            // 解析响应数据
            var deviceConfigurations = ParseSyncResponse(response);
            if (deviceConfigurations.Count == 0)
            {
                _logger.LogInformation("没有新的配置需要同步");
                return new List<DeviceConfig>();
            }

            // 保存配置到本地存储
            var savedConfigs = new List<DeviceConfig>();
            foreach (var config in deviceConfigurations)
            {
                try
                {
                    var success = await _storageService.SaveDeviceConfigAsync(config, cancellationToken);
                    if (success)
                    {
                        savedConfigs.Add(config);
                        // _logger.LogDebug("设备配置保存成功: {DeviceCode}", config.DeviceCode);
                    }
                    else
                    {
                        // _logger.LogError("设备配置保存失败: {DeviceCode}", config.DeviceCode);
                    }
                }
                catch (Exception ex)
                {
                    // _logger.LogError(ex, "保存设备配置异常: {DeviceCode}", config.DeviceCode);
                }
            }

            _logger.LogInformation("配置同步完成，成功同步 {Count} 个设备配置", savedConfigs.Count);
            return savedConfigs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "配置同步异常");
            return new List<DeviceConfig>();
        }
    }

    /// <summary>
    /// 解析同步响应数据
    /// </summary>
    private List<DeviceConfig> ParseSyncResponse(dynamic response)
    {
        var deviceConfigurations = new List<DeviceConfig>();

        try
        {
            // 检查外层响应是否成功
            if (response?.success == true && response?.data != null)
            {
                var data = response.data;
                
                // 检查内层数据是否成功
                if (data?.success == true && data?.deviceConfigurations != null)
                {
                    foreach (var deviceConfig in data.deviceConfigurations)
                    {
                        try
                        {
                            var config = new DeviceConfig
                            {
                                DeviceCode = deviceConfig.deviceCode?.ToString() ?? string.Empty,
                                DeviceName = deviceConfig.deviceName?.ToString() ?? string.Empty,
                                DeviceType = ParseDeviceType(deviceConfig.deviceType?.ToString()),
                                ConnectionConfig = new ConnectionConfig
                                {
                                    IpAddress = deviceConfig.ipAddress?.ToString() ?? string.Empty,
                                    Port = deviceConfig.port ?? 0,
                                    Username = deviceConfig.username?.ToString(),
                                    Password = deviceConfig.password?.ToString(),
                                    ConnectionTimeout = deviceConfig.connectionTimeout ?? 30,
                                    ReadTimeout = deviceConfig.readTimeout ?? 10,
                                    WriteTimeout = deviceConfig.writeTimeout ?? 10
                                },
                                DataPoints = new List<DataPointConfig>(),
                                SamplingInterval = deviceConfig.samplingInterval ?? 1,
                                IsEnabled = deviceConfig.isEnabled ?? true
                            };

                            // 解析数据点配置
                            if (deviceConfig.dataPoints != null)
                            {
                                foreach (var dataPoint in deviceConfig.dataPoints)
                                {
                                    config.DataPoints.Add(new DataPointConfig
                                    {
                                        PointCode = dataPoint.pointCode?.ToString() ?? string.Empty,
                                        PointName = dataPoint.pointName?.ToString() ?? string.Empty,
                                        Address = dataPoint.address?.ToString() ?? string.Empty,
                                        DataType = ParseDataType(dataPoint.dataType?.ToString()),
                                        Unit = dataPoint.unit?.ToString() ?? string.Empty,
                                        ScaleFactor = dataPoint.scaleFactor ?? 1.0,
                                        Offset = dataPoint.offset ?? 0.0,
                                        IsEnabled = dataPoint.isEnabled ?? true,
                                        Description = dataPoint.description?.ToString() ?? string.Empty
                                    });
                                }
                            }

                            deviceConfigurations.Add(config);
                        }
                        catch (Exception ex)
                        {
                            // _logger.LogError(ex, "解析设备配置异常: {DeviceCode}", deviceConfig?.deviceCode);
                        }
                    }
                }
                else
                {
                    // _logger.LogWarning("内层响应数据不成功或为空: success={Success}, deviceConfigurations={HasConfigs}", 
                    //     data?.success, data?.deviceConfigurations != null);
                }
            }
            else
            {
                // _logger.LogWarning("外层响应不成功或为空: success={Success}, data={HasData}", 
                //     response?.success, response?.data != null);
            }
        }
        catch (Exception ex)
        {
            // _logger.LogError(ex, "解析同步响应数据异常");
        }

        return deviceConfigurations;
    }

    /// <summary>
    /// 解析设备类型
    /// </summary>
    private DeviceType ParseDeviceType(string? deviceType)
    {
        return deviceType?.ToUpperInvariant() switch
        {
            "PLC" => DeviceType.PLC,
            "OPC" => DeviceType.OPCUA,
            "MODBUS" => DeviceType.Modbus,
            "SOCKET" => DeviceType.Socket,
            _ => DeviceType.PLC
        };
    }

    /// <summary>
    /// 解析数据类型
    /// </summary>
    private DataType ParseDataType(string? dataType)
    {
        return dataType?.ToUpperInvariant() switch
        {
            "BOOL" => DataType.Bool,
            "BYTE" => DataType.Int16,
            "SHORT" => DataType.Int16,
            "INT" => DataType.Int32,
            "LONG" => DataType.Int32,
            "FLOAT" => DataType.Float,
            "DOUBLE" => DataType.Float,
            "STRING" => DataType.String,
            _ => DataType.Float
        };
    }

    /// <summary>
    /// 测试配置同步连接
    /// </summary>
    public async Task<bool> TestSyncConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClientService.TestConnectionAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "测试配置同步连接异常");
            return false;
        }
    }
} 
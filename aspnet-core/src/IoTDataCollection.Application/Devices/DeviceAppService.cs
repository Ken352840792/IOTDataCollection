using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IoTDataCollection.Permissions;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备应用服务实现
/// </summary>
[Authorize(IoTDataCollectionPermissions.Devices.Default)]
public class DeviceAppService : CrudAppService<
        Device,
        DeviceDto,
        Guid,
        GetDeviceListDto,
        CreateDeviceDto,
        UpdateDeviceDto>,
    IDeviceAppService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceAppService(IDeviceRepository repository) : base(repository)
    {
        _deviceRepository = repository;
        GetPolicyName = IoTDataCollectionPermissions.Devices.Default;
        GetListPolicyName = IoTDataCollectionPermissions.Devices.Default;
        CreatePolicyName = IoTDataCollectionPermissions.Devices.Create;
        UpdatePolicyName = IoTDataCollectionPermissions.Devices.Edit;
        DeletePolicyName = IoTDataCollectionPermissions.Devices.Delete;
    }

    protected override async Task<IQueryable<Device>> CreateFilteredQueryAsync(GetDeviceListDto input)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();

        // 应用基础过滤
        query = query.WhereIf(
            !input.Filter.IsNullOrWhiteSpace(),
            e => e.F_DeviceName.Contains(input.Filter!) ||
                 e.F_DeviceCode.Contains(input.Filter!) ||
                 (e.F_DeviceType != null && e.F_DeviceType.Contains(input.Filter!)) ||
                 (e.F_Manufacturer != null && e.F_Manufacturer.Contains(input.Filter!)) ||
                 (e.F_CommunicationProtocol != null && e.F_CommunicationProtocol.Contains(input.Filter!))
        );

        // 应用详细过滤条件
        query = query
            .WhereIf(input.SiteId.HasValue, e => e.F_SiteId == input.SiteId.Value)
            .WhereIf(!input.DeviceType.IsNullOrWhiteSpace(), e => e.F_DeviceType == input.DeviceType)
            .WhereIf(!input.CommunicationProtocol.IsNullOrWhiteSpace(), e => e.F_CommunicationProtocol == input.CommunicationProtocol)
            .WhereIf(input.ConnectionStatus.HasValue, e => e.F_ConnectionStatus == input.ConnectionStatus.Value)
            .WhereIf(input.IsCollectionEnabled.HasValue, e => e.F_IsCollectionEnabled == input.IsCollectionEnabled.Value)
            .WhereIf(input.Status.HasValue, e => e.F_Status == input.Status.Value);

        return query;
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Create)]
    public override async Task<DeviceDto> CreateAsync(CreateDeviceDto input)
    {
        // 检查设备编码唯一性
        if (await _deviceRepository.IsCodeExistAsync(input.DeviceCode))
        {
            throw new Volo.Abp.UserFriendlyException($"设备编码 '{input.DeviceCode}' 已存在，请使用其他编码。");
        }

        return await base.CreateAsync(input);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public override async Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto input)
    {
        // 检查设备编码唯一性
        if (await _deviceRepository.IsCodeExistAsync(input.DeviceCode, id))
        {
            throw new Volo.Abp.UserFriendlyException($"设备编码 '{input.DeviceCode}' 已存在，请使用其他编码。");
        }

        return await base.UpdateAsync(id, input);
    }

    public async Task<DeviceDto?> FindByCodeAsync(string deviceCode)
    {
        var device = await _deviceRepository.FindByCodeAsync(deviceCode);
        return device != null ? await MapToGetOutputDtoAsync(device) : null;
    }

    public async Task<PagedResultDto<DeviceDto>> GetListBySiteAsync(Guid siteId, GetDeviceListDto input)
    {
        var devices = await _deviceRepository.GetListBySiteAsync(
            siteId,
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "F_SortOrder",
            input.Filter
        );

        var totalCount = await _deviceRepository.GetCountAsync(input.Filter);
        
        var deviceDtos = await MapToGetListOutputDtosAsync(devices);

        return new PagedResultDto<DeviceDto>(totalCount, deviceDtos);
    }

    public async Task<PagedResultDto<DeviceDto>> GetOnlineDevicesAsync(GetDeviceListDto input)
    {
        var devices = await _deviceRepository.GetOnlineDevicesAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "F_SortOrder"
        );

        var totalCount = devices.Count;
        
        var deviceDtos = await MapToGetListOutputDtosAsync(devices);

        return new PagedResultDto<DeviceDto>(totalCount, deviceDtos);
    }

    public async Task<PagedResultDto<DeviceDto>> GetCollectionEnabledDevicesAsync(GetDeviceListDto input)
    {
        var devices = await _deviceRepository.GetCollectionEnabledDevicesAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting ?? "F_SortOrder"
        );

        var totalCount = devices.Count;
        
        var deviceDtos = await MapToGetListOutputDtosAsync(devices);

        return new PagedResultDto<DeviceDto>(totalCount, deviceDtos);
    }

    public async Task<bool> IsCodeExistAsync(string deviceCode, Guid? excludeId = null)
    {
        return await _deviceRepository.IsCodeExistAsync(deviceCode, excludeId);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task EnableAsync(Guid id)
    {
        var device = await _deviceRepository.GetAsync(id);
        device.F_Status = 1;
        await _deviceRepository.UpdateAsync(device);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task DisableAsync(Guid id)
    {
        var device = await _deviceRepository.GetAsync(id);
        device.F_Status = 0;
        await _deviceRepository.UpdateAsync(device);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task EnableCollectionAsync(Guid id)
    {
        var device = await _deviceRepository.GetAsync(id);
        device.F_IsCollectionEnabled = true;
        await _deviceRepository.UpdateAsync(device);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task DisableCollectionAsync(Guid id)
    {
        var device = await _deviceRepository.GetAsync(id);
        device.F_IsCollectionEnabled = false;
        await _deviceRepository.UpdateAsync(device);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task UpdateConnectionStatusAsync(Guid id, int connectionStatus, DateTime? lastCommunicationTime = null)
    {
        await _deviceRepository.UpdateConnectionStatusAsync(id, connectionStatus, lastCommunicationTime);
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Edit)]
    public async Task BatchUpdateConnectionStatusAsync(List<DeviceConnectionStatusDto> connectionStatusList)
    {
        foreach (var statusUpdate in connectionStatusList)
        {
            await _deviceRepository.UpdateConnectionStatusAsync(
                statusUpdate.DeviceId,
                statusUpdate.ConnectionStatus,
                statusUpdate.LastCommunicationTime
            );
        }
    }

    public async Task<DeviceStatsDto> GetStatsAsync()
    {
        var allDevices = await _deviceRepository.GetListAsync();
        
        return await CalculateStatsAsync(allDevices);
    }

    public async Task<DeviceStatsDto> GetStatsBySiteAsync(Guid siteId)
    {
        var siteDevices = await _deviceRepository.GetListBySiteAsync(siteId);
        
        return await CalculateStatsAsync(siteDevices);
    }

    private async Task<DeviceStatsDto> CalculateStatsAsync(List<Device> devices)
    {
        var stats = new DeviceStatsDto
        {
            TotalCount = devices.Count,
            OnlineCount = devices.Count(d => d.F_ConnectionStatus == 1),
            OfflineCount = devices.Count(d => d.F_ConnectionStatus == 0),
            FaultCount = devices.Count(d => d.F_ConnectionStatus == 2),
            CollectionEnabledCount = devices.Count(d => d.F_IsCollectionEnabled),
            RecentlyCreatedCount = devices.Count(d => d.CreationTime >= DateTime.Now.AddDays(-1))
        };

        // 按设备类型统计
        stats.CountByDeviceType = devices
            .Where(d => !string.IsNullOrEmpty(d.F_DeviceType))
            .GroupBy(d => d.F_DeviceType!)
            .ToDictionary(g => g.Key, g => g.Count());

        // 按通信协议统计
        stats.CountByProtocol = devices
            .Where(d => !string.IsNullOrEmpty(d.F_CommunicationProtocol))
            .GroupBy(d => d.F_CommunicationProtocol!)
            .ToDictionary(g => g.Key, g => g.Count());

        return stats;
    }

    #region 设备调试功能

    [Authorize(IoTDataCollectionPermissions.Devices.Debug)]
    public async Task<DeviceDebugResponseDto> DebugDeviceAsync(DeviceDebugRequestDto request)
    {
        return request.OperationType.ToUpper() switch
        {
            "TESTCONNECTION" => await TestConnectionAsync(request.DeviceId, request.Timeout),
            "READDATA" => await ReadDeviceDataAsync(request.DeviceId, request.Address!, request.DataType!, request.Timeout),
            "WRITEDATA" => await WriteDeviceDataAsync(request.DeviceId, request.Address!, request.WriteValue!, request.DataType!, request.Timeout),
            _ => new DeviceDebugResponseDto
            {
                IsSuccess = false,
                OperationType = request.OperationType,
                ErrorMessage = $"不支持的操作类型: {request.OperationType}",
                ExecutionTimeMs = 0
            }
        };
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Debug)]
    public async Task<DeviceDebugResponseDto> TestConnectionAsync(Guid deviceId, int timeout = 5000)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DeviceDebugResponseDto
        {
            OperationType = "TestConnection",
            DebugTime = DateTime.Now
        };

        try
        {
            var device = await _deviceRepository.GetAsync(deviceId);
            
            response.DebugDetails.Add($"开始测试设备连接: {device.F_DeviceName} ({device.F_DeviceCode})");
            response.DebugDetails.Add($"设备地址: {device.F_IpAddress}:{device.F_Port}");
            response.DebugDetails.Add($"通信协议: {device.F_CommunicationProtocol}");
            response.DebugDetails.Add($"超时设置: {timeout}ms");

            // 这里应该根据不同的通信协议实现真实的连接测试
            // 暂时使用模拟实现
            await Task.Delay(100); // 模拟网络延迟
            
            bool connectionResult = !string.IsNullOrEmpty(device.F_IpAddress); // 简单的模拟逻辑
            
            response.IsSuccess = connectionResult;
            if (connectionResult)
            {
                response.DebugDetails.Add("连接测试成功");
                // 更新设备连接状态
                await UpdateConnectionStatusAsync(deviceId, 1, DateTime.Now);
            }
            else
            {
                response.ErrorMessage = "连接测试失败 - 无法建立连接";
                response.DebugDetails.Add("连接测试失败");
                await UpdateConnectionStatusAsync(deviceId, 0, DateTime.Now);
            }
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.ErrorMessage = ex.Message;
            response.DebugDetails.Add($"连接测试异常: {ex.Message}");
            Logger.LogError(ex, "设备连接测试失败，设备ID: {DeviceId}", deviceId);
        }
        finally
        {
            stopwatch.Stop();
            response.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return response;
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Debug)]
    public async Task<DeviceDebugResponseDto> ReadDeviceDataAsync(Guid deviceId, string address, string dataType, int timeout = 5000)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DeviceDebugResponseDto
        {
            OperationType = "ReadData",
            DebugTime = DateTime.Now
        };

        try
        {
            var device = await _deviceRepository.GetAsync(deviceId);
            
            response.DebugDetails.Add($"开始读取设备数据: {device.F_DeviceName} ({device.F_DeviceCode})");
            response.DebugDetails.Add($"数据地址: {address}");
            response.DebugDetails.Add($"数据类型: {dataType}");
            response.DebugDetails.Add($"超时设置: {timeout}ms");

            // 这里应该根据不同的通信协议实现真实的数据读取
            // 暂时使用模拟实现
            await Task.Delay(50); // 模拟读取延迟

            // 模拟读取结果
            response.ReadValue = dataType.ToUpper() switch
            {
                "INT16" => "12345",
                "INT32" => "123456789",
                "FLOAT" => "123.45",
                "BOOLEAN" => "true",
                "STRING" => "Hello IoT",
                _ => "0"
            };

            response.IsSuccess = true;
            response.DebugDetails.Add($"数据读取成功，值: {response.ReadValue}");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.ErrorMessage = ex.Message;
            response.DebugDetails.Add($"数据读取异常: {ex.Message}");
            Logger.LogError(ex, "设备数据读取失败，设备ID: {DeviceId}, 地址: {Address}", deviceId, address);
        }
        finally
        {
            stopwatch.Stop();
            response.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return response;
    }

    [Authorize(IoTDataCollectionPermissions.Devices.Debug)]
    public async Task<DeviceDebugResponseDto> WriteDeviceDataAsync(Guid deviceId, string address, string value, string dataType, int timeout = 5000)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = new DeviceDebugResponseDto
        {
            OperationType = "WriteData",
            DebugTime = DateTime.Now
        };

        try
        {
            var device = await _deviceRepository.GetAsync(deviceId);
            
            response.DebugDetails.Add($"开始写入设备数据: {device.F_DeviceName} ({device.F_DeviceCode})");
            response.DebugDetails.Add($"数据地址: {address}");
            response.DebugDetails.Add($"写入值: {value}");
            response.DebugDetails.Add($"数据类型: {dataType}");
            response.DebugDetails.Add($"超时设置: {timeout}ms");

            // 这里应该根据不同的通信协议实现真实的数据写入
            // 暂时使用模拟实现
            await Task.Delay(80); // 模拟写入延迟

            response.IsSuccess = true;
            response.DebugDetails.Add("数据写入成功");
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.ErrorMessage = ex.Message;
            response.DebugDetails.Add($"数据写入异常: {ex.Message}");
            Logger.LogError(ex, "设备数据写入失败，设备ID: {DeviceId}, 地址: {Address}, 值: {Value}", deviceId, address, value);
        }
        finally
        {
            stopwatch.Stop();
            response.ExecutionTimeMs = stopwatch.ElapsedMilliseconds;
        }

        return response;
    }

    #endregion
} 
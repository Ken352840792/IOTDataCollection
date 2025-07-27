using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IoTDataCollection.Devices;

namespace IoTDataCollection.Controllers;

/// <summary>
/// 设备管理API控制器
/// </summary>
[Area(IoTDataCollectionRemoteServiceConsts.ModuleName)]
[RemoteService(Name = IoTDataCollectionRemoteServiceConsts.RemoteServiceName)]
[Route("api/devices")]
public class DevicesController : AbpControllerBase, IDeviceAppService
{
    private readonly IDeviceAppService _deviceAppService;

    public DevicesController(IDeviceAppService deviceAppService)
    {
        _deviceAppService = deviceAppService;
    }

    /// <summary>
    /// 获取设备列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页设备列表</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<DeviceDto>> GetListAsync(GetDeviceListDto input)
    {
        return _deviceAppService.GetListAsync(input);
    }

    /// <summary>
    /// 根据ID获取设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns>设备信息</returns>
    [HttpGet("{id}")]
    public virtual Task<DeviceDto> GetAsync(Guid id)
    {
        return _deviceAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建设备
    /// </summary>
    /// <param name="input">创建参数</param>
    /// <returns>创建的设备信息</returns>
    [HttpPost]
    public virtual Task<DeviceDto> CreateAsync(CreateDeviceDto input)
    {
        return _deviceAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <param name="input">更新参数</param>
    /// <returns>更新后的设备信息</returns>
    [HttpPut("{id}")]
    public virtual Task<DeviceDto> UpdateAsync(Guid id, UpdateDeviceDto input)
    {
        return _deviceAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _deviceAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 根据设备编码获取设备
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <returns>设备信息</returns>
    [HttpGet("by-code/{siteId}/{deviceCode}")]
    public virtual Task<DeviceDto?> FindByCodeAsync(Guid siteId, string deviceCode)
    {
        return _deviceAppService.FindByCodeAsync(siteId, deviceCode);
    }

    /// <summary>
    /// 获取指定站点下的设备列表
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="input">查询参数</param>
    /// <returns>设备列表</returns>
    [HttpGet("by-site/{siteId}")]
    public virtual Task<PagedResultDto<DeviceDto>> GetListBySiteAsync(Guid siteId, [FromQuery] GetDeviceListDto input)
    {
        return _deviceAppService.GetListBySiteAsync(siteId, input);
    }

    /// <summary>
    /// 获取在线设备列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>在线设备列表</returns>
    [HttpGet("online")]
    public virtual Task<PagedResultDto<DeviceDto>> GetOnlineDevicesAsync([FromQuery] GetDeviceListDto input)
    {
        return _deviceAppService.GetOnlineDevicesAsync(input);
    }

    /// <summary>
    /// 获取启用采集的设备列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>启用采集的设备列表</returns>
    [HttpGet("collection-enabled")]
    public virtual Task<PagedResultDto<DeviceDto>> GetCollectionEnabledDevicesAsync([FromQuery] GetDeviceListDto input)
    {
        return _deviceAppService.GetCollectionEnabledDevicesAsync(input);
    }

    /// <summary>
    /// 检查设备编码是否已存在
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="excludeId">排除的设备ID</param>
    /// <returns>是否存在</returns>
    [HttpGet("check-code")]
    public virtual Task<bool> IsCodeExistAsync([FromQuery] Guid siteId, [FromQuery] string deviceCode, [FromQuery] Guid? excludeId = null)
    {
        return _deviceAppService.IsCodeExistAsync(siteId, deviceCode, excludeId);
    }

    /// <summary>
    /// 启用设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    [HttpPost("{id}/enable")]
    public virtual Task EnableAsync(Guid id)
    {
        return _deviceAppService.EnableAsync(id);
    }

    /// <summary>
    /// 禁用设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    [HttpPost("{id}/disable")]
    public virtual Task DisableAsync(Guid id)
    {
        return _deviceAppService.DisableAsync(id);
    }

    /// <summary>
    /// 启用设备采集
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    [HttpPost("{id}/enable-collection")]
    public virtual Task EnableCollectionAsync(Guid id)
    {
        return _deviceAppService.EnableCollectionAsync(id);
    }

    /// <summary>
    /// 禁用设备采集
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    [HttpPost("{id}/disable-collection")]
    public virtual Task DisableCollectionAsync(Guid id)
    {
        return _deviceAppService.DisableCollectionAsync(id);
    }

    /// <summary>
    /// 更新设备连接状态
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <param name="connectionStatus">连接状态</param>
    /// <param name="lastCommunicationTime">最后通信时间</param>
    /// <returns></returns>
    [HttpPost("{id}/connection-status")]
    public virtual Task UpdateConnectionStatusAsync(Guid id, int connectionStatus, DateTime? lastCommunicationTime = null)
    {
        return _deviceAppService.UpdateConnectionStatusAsync(id, connectionStatus, lastCommunicationTime);
    }

    /// <summary>
    /// 批量更新设备连接状态
    /// </summary>
    /// <param name="connectionStatusList">连接状态列表</param>
    /// <returns></returns>
    [HttpPost("batch-connection-status")]
    public virtual Task BatchUpdateConnectionStatusAsync(List<DeviceConnectionStatusDto> connectionStatusList)
    {
        return _deviceAppService.BatchUpdateConnectionStatusAsync(connectionStatusList);
    }

    /// <summary>
    /// 获取设备统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    [HttpGet("stats")]
    public virtual Task<DeviceStatsDto> GetStatsAsync()
    {
        return _deviceAppService.GetStatsAsync();
    }

    /// <summary>
    /// 获取指定站点的设备统计信息
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <returns>统计信息</returns>
    [HttpGet("stats/by-site/{siteId}")]
    public virtual Task<DeviceStatsDto> GetStatsBySiteAsync(Guid siteId)
    {
        return _deviceAppService.GetStatsBySiteAsync(siteId);
    }

    #region 设备调试功能

    /// <summary>
    /// 调试设备 - 测试连接、读取数据、写入数据
    /// </summary>
    /// <param name="request">调试请求</param>
    /// <returns>调试结果</returns>
    [HttpPost("debug")]
    public virtual Task<DeviceDebugResponseDto> DebugDeviceAsync(DeviceDebugRequestDto request)
    {
        return _deviceAppService.DebugDeviceAsync(request);
    }

    /// <summary>
    /// 测试设备连接
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>测试结果</returns>
    [HttpPost("{deviceId}/test-connection")]
    public virtual Task<DeviceDebugResponseDto> TestConnectionAsync(Guid deviceId, int timeout = 5000)
    {
        return _deviceAppService.TestConnectionAsync(deviceId, timeout);
    }

    /// <summary>
    /// 读取设备数据
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="address">数据地址</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>读取结果</returns>
    [HttpPost("{deviceId}/read-data")]
    public virtual Task<DeviceDebugResponseDto> ReadDeviceDataAsync(Guid deviceId, string address, string dataType, int timeout = 5000)
    {
        return _deviceAppService.ReadDeviceDataAsync(deviceId, address, dataType, timeout);
    }

    /// <summary>
    /// 写入设备数据
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="address">数据地址</param>
    /// <param name="value">写入值</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>写入结果</returns>
    [HttpPost("{deviceId}/write-data")]
    public virtual Task<DeviceDebugResponseDto> WriteDeviceDataAsync(Guid deviceId, string address, string value, string dataType, int timeout = 5000)
    {
        return _deviceAppService.WriteDeviceDataAsync(deviceId, address, value, dataType, timeout);
    }

    #endregion
} 
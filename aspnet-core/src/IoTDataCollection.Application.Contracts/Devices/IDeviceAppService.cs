using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备应用服务接口
/// </summary>
public interface IDeviceAppService : ICrudAppService<
    DeviceDto,
    Guid,
    GetDeviceListDto,
    CreateDeviceDto,
    UpdateDeviceDto>
{
    /// <summary>
    /// 根据设备编码查找设备
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <returns>设备信息</returns>
    Task<DeviceDto?> FindByCodeAsync(Guid siteId, string deviceCode);

    /// <summary>
    /// 获取指定站点下的设备列表
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="input">查询参数</param>
    /// <returns>设备列表</returns>
    Task<PagedResultDto<DeviceDto>> GetListBySiteAsync(Guid siteId, GetDeviceListDto input);

    /// <summary>
    /// 获取在线设备列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>在线设备列表</returns>
    Task<PagedResultDto<DeviceDto>> GetOnlineDevicesAsync(GetDeviceListDto input);

    /// <summary>
    /// 获取启用采集的设备列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>启用采集的设备列表</returns>
    Task<PagedResultDto<DeviceDto>> GetCollectionEnabledDevicesAsync(GetDeviceListDto input);

    /// <summary>
    /// 检查设备编码是否已存在
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="excludeId">排除的设备ID</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(Guid siteId, string deviceCode, Guid? excludeId = null);

    /// <summary>
    /// 启用设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    Task EnableAsync(Guid id);

    /// <summary>
    /// 禁用设备
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    Task DisableAsync(Guid id);

    /// <summary>
    /// 启用设备采集
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    Task EnableCollectionAsync(Guid id);

    /// <summary>
    /// 禁用设备采集
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <returns></returns>
    Task DisableCollectionAsync(Guid id);

    /// <summary>
    /// 更新设备连接状态
    /// </summary>
    /// <param name="id">设备ID</param>
    /// <param name="connectionStatus">连接状态</param>
    /// <param name="lastCommunicationTime">最后通信时间</param>
    /// <returns></returns>
    Task UpdateConnectionStatusAsync(Guid id, int connectionStatus, DateTime? lastCommunicationTime = null);

    /// <summary>
    /// 批量更新设备连接状态
    /// </summary>
    /// <param name="connectionStatusList">连接状态列表</param>
    /// <returns></returns>
    Task BatchUpdateConnectionStatusAsync(List<DeviceConnectionStatusDto> connectionStatusList);

    /// <summary>
    /// 获取设备统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    Task<DeviceStatsDto> GetStatsAsync();

    /// <summary>
    /// 获取指定站点的设备统计信息
    /// </summary>
    /// <param name="siteId">站点ID</param>
    /// <returns>统计信息</returns>
    Task<DeviceStatsDto> GetStatsBySiteAsync(Guid siteId);

    #region 设备调试功能

    /// <summary>
    /// 调试设备 - 测试连接、读取数据、写入数据
    /// </summary>
    /// <param name="request">调试请求</param>
    /// <returns>调试结果</returns>
    Task<DeviceDebugResponseDto> DebugDeviceAsync(DeviceDebugRequestDto request);

    /// <summary>
    /// 测试设备连接
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>测试结果</returns>
    Task<DeviceDebugResponseDto> TestConnectionAsync(Guid deviceId, int timeout = 5000);

    /// <summary>
    /// 读取设备数据
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="address">数据地址</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>读取结果</returns>
    Task<DeviceDebugResponseDto> ReadDeviceDataAsync(Guid deviceId, string address, string dataType, int timeout = 5000);

    /// <summary>
    /// 写入设备数据
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="address">数据地址</param>
    /// <param name="value">写入值</param>
    /// <param name="dataType">数据类型</param>
    /// <param name="timeout">超时时间（毫秒）</param>
    /// <returns>写入结果</returns>
    Task<DeviceDebugResponseDto> WriteDeviceDataAsync(Guid deviceId, string address, string value, string dataType, int timeout = 5000);

    #endregion
} 
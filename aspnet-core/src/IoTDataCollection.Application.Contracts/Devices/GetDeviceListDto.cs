using System;
using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Devices;

/// <summary>
/// 获取设备列表请求DTO
/// </summary>
public class GetDeviceListDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤关键词 - 可对设备编码、设备名称、设备类型、制造商进行模糊搜索
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 站点ID过滤
    /// </summary>
    public Guid? SiteId { get; set; }

    /// <summary>
    /// 设备类型过滤
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// 通信协议过滤
    /// </summary>
    public string? CommunicationProtocol { get; set; }

    /// <summary>
    /// 连接状态过滤 - 0:离线 1:在线 2:故障
    /// </summary>
    public int? ConnectionStatus { get; set; }

    /// <summary>
    /// 是否启用采集过滤
    /// </summary>
    public bool? IsCollectionEnabled { get; set; }

    /// <summary>
    /// 状态过滤 - 0:禁用 1:启用
    /// </summary>
    public int? Status { get; set; }

    public GetDeviceListDto()
    {
        Sorting = "F_SortOrder,F_DeviceCode";
    }
} 
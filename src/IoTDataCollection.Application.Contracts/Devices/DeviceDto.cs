using System;
using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备数据传输对象
/// </summary>
public class DeviceDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 站点ID
    /// </summary>
    public Guid SiteId { get; set; }

    /// <summary>
    /// 设备编码
    /// </summary>
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    public string? DeviceType { get; set; }

    /// <summary>
    /// 设备型号
    /// </summary>
    public string? DeviceModel { get; set; }

    /// <summary>
    /// 设备制造商
    /// </summary>
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 通信协议
    /// </summary>
    public string? CommunicationProtocol { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// 从站地址
    /// </summary>
    public int? SlaveAddress { get; set; }

    /// <summary>
    /// 采集间隔（毫秒）
    /// </summary>
    public int CollectionInterval { get; set; }

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    public int Timeout { get; set; }

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// 设备位置描述
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// 设备描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 连接状态 - 0:离线 1:在线 2:故障
    /// </summary>
    public int ConnectionStatus { get; set; }

    /// <summary>
    /// 最后通信时间
    /// </summary>
    public DateTime? LastCommunicationTime { get; set; }

    /// <summary>
    /// 是否启用采集
    /// </summary>
    public bool IsCollectionEnabled { get; set; }

    /// <summary>
    /// 状态 - 0:禁用 1:启用
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 连接状态文本
    /// </summary>
    public string ConnectionStatusText => ConnectionStatus switch
    {
        0 => "离线",
        1 => "在线",
        2 => "故障",
        _ => "未知"
    };

    /// <summary>
    /// 状态文本
    /// </summary>
    public string StatusText => Status == 1 ? "启用" : "禁用";
} 
using System;
using System.ComponentModel.DataAnnotations;

namespace IoTDataCollection.Devices;

/// <summary>
/// 更新设备数据传输对象
/// </summary>
public class UpdateDeviceDto
{
    /// <summary>
    /// 站点ID
    /// </summary>
    [Required(ErrorMessage = "站点ID不能为空")]
    public Guid SiteId { get; set; }

    /// <summary>
    /// 设备编码
    /// </summary>
    [Required(ErrorMessage = "设备编码不能为空")]
    [StringLength(64, ErrorMessage = "设备编码长度不能超过64个字符")]
    public string DeviceCode { get; set; } = string.Empty;

    /// <summary>
    /// 设备名称
    /// </summary>
    [Required(ErrorMessage = "设备名称不能为空")]
    [StringLength(256, ErrorMessage = "设备名称长度不能超过256个字符")]
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>
    /// 设备类型
    /// </summary>
    [StringLength(64, ErrorMessage = "设备类型长度不能超过64个字符")]
    public string? DeviceType { get; set; }

    /// <summary>
    /// 设备型号
    /// </summary>
    [StringLength(128, ErrorMessage = "设备型号长度不能超过128个字符")]
    public string? DeviceModel { get; set; }

    /// <summary>
    /// 设备制造商
    /// </summary>
    [StringLength(128, ErrorMessage = "设备制造商长度不能超过128个字符")]
    public string? Manufacturer { get; set; }

    /// <summary>
    /// 通信协议
    /// </summary>
    [StringLength(64, ErrorMessage = "通信协议长度不能超过64个字符")]
    public string? CommunicationProtocol { get; set; }

    /// <summary>
    /// IP地址
    /// </summary>
    [StringLength(45, ErrorMessage = "IP地址长度不能超过45个字符")]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 端口号
    /// </summary>
    [Range(1, 65535, ErrorMessage = "端口号必须在1-65535之间")]
    public int? Port { get; set; }

    /// <summary>
    /// 从站地址
    /// </summary>
    [Range(0, 255, ErrorMessage = "从站地址必须在0-255之间")]
    public int? SlaveAddress { get; set; }

    /// <summary>
    /// 采集间隔（毫秒）
    /// </summary>
    [Range(100, 3600000, ErrorMessage = "采集间隔必须在100毫秒到1小时之间")]
    public int CollectionInterval { get; set; } = 1000;

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    [Range(1000, 60000, ErrorMessage = "超时时间必须在1秒到1分钟之间")]
    public int Timeout { get; set; } = 5000;

    /// <summary>
    /// 重试次数
    /// </summary>
    [Range(0, 10, ErrorMessage = "重试次数必须在0-10之间")]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// 设备位置描述
    /// </summary>
    [StringLength(512, ErrorMessage = "设备位置描述长度不能超过512个字符")]
    public string? Location { get; set; }

    /// <summary>
    /// 设备描述
    /// </summary>
    [StringLength(1000, ErrorMessage = "设备描述长度不能超过1000个字符")]
    public string? Description { get; set; }

    /// <summary>
    /// 是否启用采集
    /// </summary>
    public bool IsCollectionEnabled { get; set; } = true;

    /// <summary>
    /// 状态 - 0:禁用 1:启用
    /// </summary>
    public int Status { get; set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;
} 
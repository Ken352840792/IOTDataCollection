using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备配置数据传输对象
/// </summary>
public class DeviceConfigurationDto : EntityDto<Guid>
{
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
    public string DeviceType { get; set; } = string.Empty;

    /// <summary>
    /// 通信协议
    /// </summary>
    public string CommunicationProtocol { get; set; } = string.Empty;

    /// <summary>
    /// IP地址
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// 端口
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// 连接超时时间（秒）
    /// </summary>
    public int ConnectionTimeout { get; set; } = 30;

    /// <summary>
    /// 读取超时时间（秒）
    /// </summary>
    public int ReadTimeout { get; set; } = 10;

    /// <summary>
    /// 写入超时时间（秒）
    /// </summary>
    public int WriteTimeout { get; set; } = 10;

    /// <summary>
    /// 采集间隔（秒）
    /// </summary>
    public int SamplingInterval { get; set; } = 1;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 数据点配置列表
    /// </summary>
    public List<DataPointConfigurationDto> DataPoints { get; set; } = new();

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    /// <summary>
    /// 最后修改时间
    /// </summary>
    public DateTime? LastModificationTime { get; set; }
}

/// <summary>
/// 数据点配置数据传输对象
/// </summary>
public class DataPointConfigurationDto
{
    /// <summary>
    /// 数据点编码
    /// </summary>
    public string PointCode { get; set; } = string.Empty;

    /// <summary>
    /// 数据点名称
    /// </summary>
    public string PointName { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 数据类型
    /// </summary>
    public string DataType { get; set; } = string.Empty;

    /// <summary>
    /// 单位
    /// </summary>
    public string? Unit { get; set; }

    /// <summary>
    /// 缩放因子
    /// </summary>
    public double ScaleFactor { get; set; } = 1.0;

    /// <summary>
    /// 偏移量
    /// </summary>
    public double Offset { get; set; } = 0.0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 描述
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// 设备配置同步请求
/// </summary>
public class DeviceConfigurationSyncRequest
{
    /// <summary>
    /// 采集端节点编码
    /// </summary>
    public string CollectorNodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 最后同步时间
    /// </summary>
    public DateTime? LastSyncTime { get; set; }

    /// <summary>
    /// 配置版本
    /// </summary>
    public string? ConfigVersion { get; set; }
}

/// <summary>
/// 设备配置同步响应
/// </summary>
public class DeviceConfigurationSyncResponse
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 设备配置列表
    /// </summary>
    public List<DeviceConfigurationDto> DeviceConfigurations { get; set; } = new();

    /// <summary>
    /// 配置版本
    /// </summary>
    public string ConfigVersion { get; set; } = string.Empty;

    /// <summary>
    /// 同步时间
    /// </summary>
    public DateTime SyncTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 是否有配置变更
    /// </summary>
    public bool HasChanges { get; set; } = false;
}

/// <summary>
/// 设备配置更新请求
/// </summary>
public class DeviceConfigurationUpdateRequest
{
    /// <summary>
    /// 设备配置
    /// </summary>
    public DeviceConfigurationDto Configuration { get; set; } = new();

    /// <summary>
    /// 采集端节点编码
    /// </summary>
    public string CollectorNodeCode { get; set; } = string.Empty;

    /// <summary>
    /// 是否立即同步
    /// </summary>
    public bool SyncImmediately { get; set; } = false;
} 
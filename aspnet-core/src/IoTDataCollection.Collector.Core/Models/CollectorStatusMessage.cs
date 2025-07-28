using System;
using System.Collections.Generic;
using IoTDataCollection.Collector.Core.Services;

namespace IoTDataCollection.Collector.Core.Models;

/// <summary>
/// 采集端状态消息
/// </summary>
public class CollectorStatusMessage
{
    /// <summary>
    /// 采集端节点标识
    /// </summary>
    public string CollectorNode { get; set; } = string.Empty;

    /// <summary>
    /// 时间戳
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 状态类型
    /// </summary>
    public string StatusType { get; set; } = string.Empty; // Online, Offline, Error, Warning

    /// <summary>
    /// 状态描述
    /// </summary>
    public string StatusDescription { get; set; } = string.Empty;

    /// <summary>
    /// 设备连接状态
    /// </summary>
    public Dictionary<string, bool> DeviceConnections { get; set; } = new();

    /// <summary>
    /// 系统资源信息
    /// </summary>
    public SystemResourceInfo SystemResources { get; set; } = new();
} 
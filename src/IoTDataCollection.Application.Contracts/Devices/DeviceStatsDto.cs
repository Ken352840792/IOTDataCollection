using System.Collections.Generic;

namespace IoTDataCollection.Devices;

/// <summary>
/// 设备统计信息DTO
/// </summary>
public class DeviceStatsDto
{
    /// <summary>
    /// 设备总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 在线设备数
    /// </summary>
    public int OnlineCount { get; set; }

    /// <summary>
    /// 离线设备数
    /// </summary>
    public int OfflineCount { get; set; }

    /// <summary>
    /// 故障设备数
    /// </summary>
    public int FaultCount { get; set; }

    /// <summary>
    /// 启用采集的设备数
    /// </summary>
    public int CollectionEnabledCount { get; set; }

    /// <summary>
    /// 按设备类型统计
    /// </summary>
    public Dictionary<string, int> CountByDeviceType { get; set; } = new();

    /// <summary>
    /// 按通信协议统计
    /// </summary>
    public Dictionary<string, int> CountByProtocol { get; set; } = new();

    /// <summary>
    /// 按站点统计
    /// </summary>
    public Dictionary<string, int> CountBySite { get; set; } = new();

    /// <summary>
    /// 最近24小时新增设备数
    /// </summary>
    public int RecentlyCreatedCount { get; set; }

    /// <summary>
    /// 在线率（百分比）
    /// </summary>
    public decimal OnlineRate => TotalCount == 0 ? 0 : (decimal)OnlineCount / TotalCount * 100;

    /// <summary>
    /// 采集启用率（百分比）
    /// </summary>
    public decimal CollectionEnabledRate => TotalCount == 0 ? 0 : (decimal)CollectionEnabledCount / TotalCount * 100;
} 
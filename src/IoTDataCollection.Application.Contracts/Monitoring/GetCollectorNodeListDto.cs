using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Monitoring;

/// <summary>
/// 获取采集节点列表请求DTO
/// </summary>
public class GetCollectorNodeListDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤关键词 - 可对节点编码、节点名称、IP地址进行模糊搜索
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 节点类型过滤 - Edge、Gateway、Central
    /// </summary>
    public string? NodeType { get; set; }

    /// <summary>
    /// 节点状态过滤 - 0:离线 1:在线 2:故障 3:维护中
    /// </summary>
    public int? NodeStatus { get; set; }

    /// <summary>
    /// 连接状态过滤 - 0:断开 1:已连接 2:连接中
    /// </summary>
    public int? ConnectionStatus { get; set; }

    /// <summary>
    /// 是否启用监控过滤
    /// </summary>
    public bool? IsMonitoringEnabled { get; set; }

    /// <summary>
    /// 是否启用告警过滤
    /// </summary>
    public bool? IsAlertEnabled { get; set; }

    /// <summary>
    /// 是否只显示超时节点
    /// </summary>
    public bool? OnlyTimeoutNodes { get; set; }

    /// <summary>
    /// 是否只显示故障节点
    /// </summary>
    public bool? OnlyFaultNodes { get; set; }

    public GetCollectorNodeListDto()
    {
        Sorting = "F_SortOrder,F_NodeCode";
    }
} 
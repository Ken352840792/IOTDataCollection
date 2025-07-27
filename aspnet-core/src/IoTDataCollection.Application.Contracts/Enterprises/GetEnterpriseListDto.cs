using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 获取企业列表DTO
/// </summary>
public class GetEnterpriseListDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 过滤条件（搜索企业编码、名称、简称）
    /// </summary>
    public string? Filter { get; set; }

    /// <summary>
    /// 企业类型过滤
    /// </summary>
    public string? EnterpriseType { get; set; }

    /// <summary>
    /// 是否启用过滤
    /// </summary>
    public bool? IsEnabled { get; set; }
} 
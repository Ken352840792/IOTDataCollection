using System;
using Volo.Abp.Application.Dtos;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业数据传输对象
/// </summary>
public class EnterpriseDto : FullAuditedEntityDto<Guid>
{
    /// <summary>
    /// 企业编码
    /// </summary>
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// 企业名称
    /// </summary>
    public string EnterpriseName { get; set; } = string.Empty;

    /// <summary>
    /// 企业简称
    /// </summary>
    public string? ShortName { get; set; }

    /// <summary>
    /// 企业类型
    /// </summary>
    public string EnterpriseType { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 企业地址
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// 企业描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; }
} 
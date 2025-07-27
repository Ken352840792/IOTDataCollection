using System.ComponentModel.DataAnnotations;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 创建企业DTO
/// </summary>
public class CreateEnterpriseDto
{
    /// <summary>
    /// 企业编码
    /// </summary>
    [Required]
    [StringLength(50)]
    public string EnterpriseCode { get; set; } = string.Empty;

    /// <summary>
    /// 企业名称
    /// </summary>
    [Required]
    [StringLength(200)]
    public string EnterpriseName { get; set; } = string.Empty;

    /// <summary>
    /// 企业简称
    /// </summary>
    [StringLength(100)]
    public string? ShortName { get; set; }

    /// <summary>
    /// 企业类型
    /// </summary>
    [Required]
    [StringLength(50)]
    public string EnterpriseType { get; set; } = string.Empty;

    /// <summary>
    /// 联系人
    /// </summary>
    [StringLength(100)]
    public string? ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(50)]
    public string? ContactPhone { get; set; }

    /// <summary>
    /// 企业地址
    /// </summary>
    [StringLength(500)]
    public string? Address { get; set; }

    /// <summary>
    /// 企业描述
    /// </summary>
    [StringLength(1000)]
    public string? Description { get; set; }

    /// <summary>
    /// 排序号
    /// </summary>
    public int SortOrder { get; set; } = 0;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;
} 
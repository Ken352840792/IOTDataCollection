using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Sites;

/// <summary>
/// 站点实体 - 表名：T_SITES
/// </summary>
[Table("T_SITES")]
public class Site : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 企业ID - 外键关联到T_ENTERPRISES
    /// </summary>
    [Required]
    [Column("F_ENTERPRISE_ID")]
    public Guid F_EnterpriseId { get; set; }

    /// <summary>
    /// 站点编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_SITE_CODE")]
    public string F_SiteCode { get; set; }

    /// <summary>
    /// 站点名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_SITE_NAME")]
    public string F_SiteName { get; set; }

    /// <summary>
    /// 站点简称
    /// </summary>
    [StringLength(128)]
    [Column("F_SHORT_NAME")]
    public string? F_ShortName { get; set; }

    /// <summary>
    /// 站点类型 - 工厂、车间、仓库等
    /// </summary>
    [StringLength(64)]
    [Column("F_SITE_TYPE")]
    public string? F_SiteType { get; set; }

    /// <summary>
    /// 站点地址
    /// </summary>
    [StringLength(512)]
    [Column("F_ADDRESS")]
    public string? F_Address { get; set; }

    /// <summary>
    /// 负责人
    /// </summary>
    [StringLength(128)]
    [Column("F_MANAGER")]
    public string? F_Manager { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(32)]
    [Column("F_CONTACT_PHONE")]
    public string? F_ContactPhone { get; set; }

    /// <summary>
    /// 站点描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 状态 - 0:禁用 1:启用
    /// </summary>
    [Column("F_STATUS")]
    public int F_Status { get; set; } = 1;

    /// <summary>
    /// 排序号
    /// </summary>
    [Column("F_SORT_ORDER")]
    public int F_SortOrder { get; set; }

    #region 扩展字段 - 预留10个扩展字段
    [StringLength(500)]
    [Column("F_EXP_01")]
    public string? F_Exp_01 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_02")]
    public string? F_Exp_02 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_03")]
    public string? F_Exp_03 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_04")]
    public string? F_Exp_04 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_05")]
    public string? F_Exp_05 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_06")]
    public string? F_Exp_06 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_07")]
    public string? F_Exp_07 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_08")]
    public string? F_Exp_08 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_09")]
    public string? F_Exp_09 { get; set; }

    [StringLength(500)]
    [Column("F_EXP_10")]
    public string? F_Exp_10 { get; set; }
    #endregion

    protected Site()
    {
        F_SiteCode = string.Empty;
        F_SiteName = string.Empty;
    }

    public Site(
        Guid id,
        Guid enterpriseId,
        string siteCode,
        string siteName,
        string? shortName = null,
        string? siteType = null) : base(id)
    {
        F_EnterpriseId = enterpriseId;
        F_SiteCode = siteCode;
        F_SiteName = siteName;
        F_ShortName = shortName;
        F_SiteType = siteType;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
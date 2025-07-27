using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业实体 - 表名：T_ENTERPRISES
/// </summary>
[Table("T_ENTERPRISES")]
public class Enterprise : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 企业编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_ENTERPRISE_CODE")]
    public string F_EnterpriseCode { get; set; }

    /// <summary>
    /// 企业名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_ENTERPRISE_NAME")]
    public string F_EnterpriseName { get; set; }

    /// <summary>
    /// 企业简称
    /// </summary>
    [StringLength(128)]
    [Column("F_SHORT_NAME")]
    public string? F_ShortName { get; set; }

    /// <summary>
    /// 企业类型 - 制造业、化工、电力等
    /// </summary>
    [StringLength(64)]
    [Column("F_ENTERPRISE_TYPE")]
    public string? F_EnterpriseType { get; set; }

    /// <summary>
    /// 联系人
    /// </summary>
    [StringLength(128)]
    [Column("F_CONTACT_PERSON")]
    public string? F_ContactPerson { get; set; }

    /// <summary>
    /// 联系电话
    /// </summary>
    [StringLength(32)]
    [Column("F_CONTACT_PHONE")]
    public string? F_ContactPhone { get; set; }

    /// <summary>
    /// 企业地址
    /// </summary>
    [StringLength(512)]
    [Column("F_ADDRESS")]
    public string? F_Address { get; set; }

    /// <summary>
    /// 企业描述
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

    protected Enterprise()
    {
        F_EnterpriseCode = string.Empty;
        F_EnterpriseName = string.Empty;
    }

    public Enterprise(
        Guid id,
        string enterpriseCode,
        string enterpriseName,
        string? shortName = null,
        string? enterpriseType = null) : base(id)
    {
        F_EnterpriseCode = enterpriseCode;
        F_EnterpriseName = enterpriseName;
        F_ShortName = shortName;
        F_EnterpriseType = enterpriseType;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
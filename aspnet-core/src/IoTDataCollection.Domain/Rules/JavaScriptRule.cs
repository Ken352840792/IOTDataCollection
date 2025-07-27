using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Rules;

/// <summary>
/// JavaScript规则实体 - 表名：T_JAVASCRIPT_RULES
/// </summary>
[Table("T_JAVASCRIPT_RULES")]
public class JavaScriptRule : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 设备ID - 外键关联到T_DEVICES (可为空表示全局规则)
    /// </summary>
    [Column("F_DEVICE_ID")]
    public Guid? F_DeviceId { get; set; }

    /// <summary>
    /// 规则编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_RULE_CODE")]
    public string F_RuleCode { get; set; }

    /// <summary>
    /// 规则名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_RULE_NAME")]
    public string F_RuleName { get; set; }

    /// <summary>
    /// 规则类型 - 数据处理、告警、控制等
    /// </summary>
    [StringLength(64)]
    [Column("F_RULE_TYPE")]
    public string? F_RuleType { get; set; }

    /// <summary>
    /// 触发条件 - 时间触发、数据变化触发、事件触发
    /// </summary>
    [StringLength(128)]
    [Column("F_TRIGGER_CONDITION")]
    public string? F_TriggerCondition { get; set; }

    /// <summary>
    /// JavaScript脚本代码
    /// </summary>
    [Required]
    [Column("F_SCRIPT_CODE", TypeName = "LONGTEXT")]
    public string F_ScriptCode { get; set; }

    /// <summary>
    /// 脚本版本号
    /// </summary>
    [Required]
    [StringLength(32)]
    [Column("F_VERSION")]
    public string F_Version { get; set; }

    /// <summary>
    /// 执行优先级 - 1:高 2:中 3:低
    /// </summary>
    [Column("F_EXECUTION_PRIORITY")]
    public int F_ExecutionPriority { get; set; } = 2;

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    [Column("F_TIMEOUT")]
    public int F_Timeout { get; set; } = 5000;

    /// <summary>
    /// 内存限制（字节）
    /// </summary>
    [Column("F_MEMORY_LIMIT")]
    public long F_MemoryLimit { get; set; } = 10485760; // 10MB

    /// <summary>
    /// 是否启用热更新
    /// </summary>
    [Column("F_IS_HOT_UPDATE_ENABLED")]
    public bool F_IsHotUpdateEnabled { get; set; } = true;

    /// <summary>
    /// 最后执行时间
    /// </summary>
    [Column("F_LAST_EXECUTION_TIME")]
    public DateTime? F_LastExecutionTime { get; set; }

    /// <summary>
    /// 执行次数
    /// </summary>
    [Column("F_EXECUTION_COUNT")]
    public long F_ExecutionCount { get; set; } = 0;

    /// <summary>
    /// 最后执行结果 - 成功、失败、超时
    /// </summary>
    [StringLength(32)]
    [Column("F_LAST_EXECUTION_RESULT")]
    public string? F_LastExecutionResult { get; set; }

    /// <summary>
    /// 最后错误信息
    /// </summary>
    [StringLength(2000)]
    [Column("F_LAST_ERROR_MESSAGE")]
    public string? F_LastErrorMessage { get; set; }

    /// <summary>
    /// 规则描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 是否启用规则 - 0:禁用 1:启用
    /// </summary>
    [Column("F_IS_ENABLED")]
    public bool F_IsEnabled { get; set; } = true;

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

    protected JavaScriptRule()
    {
        F_RuleCode = string.Empty;
        F_RuleName = string.Empty;
        F_ScriptCode = string.Empty;
        F_Version = string.Empty;
    }

    public JavaScriptRule(
        Guid id,
        string ruleCode,
        string ruleName,
        string scriptCode,
        string version,
        Guid? deviceId = null,
        string? ruleType = null) : base(id)
    {
        F_DeviceId = deviceId;
        F_RuleCode = ruleCode;
        F_RuleName = ruleName;
        F_ScriptCode = scriptCode;
        F_Version = version;
        F_RuleType = ruleType;
        F_ExecutionPriority = 2;
        F_Timeout = 5000;
        F_MemoryLimit = 10485760;
        F_IsHotUpdateEnabled = true;
        F_ExecutionCount = 0;
        F_IsEnabled = true;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
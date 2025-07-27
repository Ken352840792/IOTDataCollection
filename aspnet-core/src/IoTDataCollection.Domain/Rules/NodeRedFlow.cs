using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Volo.Abp.Domain.Entities.Auditing;

namespace IoTDataCollection.Rules;

/// <summary>
/// Node-RED流程实体 - 表名：T_NODERED_FLOWS
/// </summary>
[Table("T_NODERED_FLOWS")]
public class NodeRedFlow : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// 流程编码 - 唯一标识
    /// </summary>
    [Required]
    [StringLength(64)]
    [Column("F_FLOW_CODE")]
    public string F_FlowCode { get; set; }

    /// <summary>
    /// 流程名称
    /// </summary>
    [Required]
    [StringLength(256)]
    [Column("F_FLOW_NAME")]
    public string F_FlowName { get; set; }

    /// <summary>
    /// 流程类型 - 数据汇集、数据转换、告警处理、自动控制等
    /// </summary>
    [StringLength(64)]
    [Column("F_FLOW_TYPE")]
    public string? F_FlowType { get; set; }

    /// <summary>
    /// 流程分类 - 实时处理、批量处理、定时任务等
    /// </summary>
    [StringLength(64)]
    [Column("F_FLOW_CATEGORY")]
    public string? F_FlowCategory { get; set; }

    /// <summary>
    /// Node-RED流程JSON配置
    /// </summary>
    [Required]
    [Column("F_FLOW_CONFIG", TypeName = "LONGTEXT")]
    public string F_FlowConfig { get; set; }

    /// <summary>
    /// 流程版本号
    /// </summary>
    [Required]
    [StringLength(32)]
    [Column("F_VERSION")]
    public string F_Version { get; set; }

    /// <summary>
    /// Node-RED标签页ID
    /// </summary>
    [StringLength(64)]
    [Column("F_TAB_ID")]
    public string? F_TabId { get; set; }

    /// <summary>
    /// 执行优先级 - 1:高 2:中 3:低
    /// </summary>
    [Column("F_EXECUTION_PRIORITY")]
    public int F_ExecutionPriority { get; set; } = 2;

    /// <summary>
    /// 最大处理速率（条/秒）
    /// </summary>
    [Column("F_MAX_PROCESSING_RATE")]
    public int F_MaxProcessingRate { get; set; } = 1000;

    /// <summary>
    /// 是否启用热部署
    /// </summary>
    [Column("F_IS_HOT_DEPLOY_ENABLED")]
    public bool F_IsHotDeployEnabled { get; set; } = true;

    /// <summary>
    /// 最后部署时间
    /// </summary>
    [Column("F_LAST_DEPLOY_TIME")]
    public DateTime? F_LastDeployTime { get; set; }

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
    /// 处理的消息总数
    /// </summary>
    [Column("F_PROCESSED_MESSAGE_COUNT")]
    public long F_ProcessedMessageCount { get; set; } = 0;

    /// <summary>
    /// 错误消息数
    /// </summary>
    [Column("F_ERROR_MESSAGE_COUNT")]
    public long F_ErrorMessageCount { get; set; } = 0;

    /// <summary>
    /// 最后部署状态 - 成功、失败、部署中
    /// </summary>
    [StringLength(32)]
    [Column("F_LAST_DEPLOY_STATUS")]
    public string? F_LastDeployStatus { get; set; }

    /// <summary>
    /// 最后错误信息
    /// </summary>
    [StringLength(2000)]
    [Column("F_LAST_ERROR_MESSAGE")]
    public string? F_LastErrorMessage { get; set; }

    /// <summary>
    /// 流程描述
    /// </summary>
    [StringLength(1000)]
    [Column("F_DESCRIPTION")]
    public string? F_Description { get; set; }

    /// <summary>
    /// 流程作者
    /// </summary>
    [StringLength(128)]
    [Column("F_AUTHOR")]
    public string? F_Author { get; set; }

    /// <summary>
    /// 关联设备列表（JSON格式）
    /// </summary>
    [Column("F_RELATED_DEVICES", TypeName = "TEXT")]
    public string? F_RelatedDevices { get; set; }

    /// <summary>
    /// 输入主题列表（JSON格式）
    /// </summary>
    [Column("F_INPUT_TOPICS", TypeName = "TEXT")]
    public string? F_InputTopics { get; set; }

    /// <summary>
    /// 输出主题列表（JSON格式）
    /// </summary>
    [Column("F_OUTPUT_TOPICS", TypeName = "TEXT")]
    public string? F_OutputTopics { get; set; }

    /// <summary>
    /// 是否启用流程 - 0:禁用 1:启用
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

    protected NodeRedFlow()
    {
        F_FlowCode = string.Empty;
        F_FlowName = string.Empty;
        F_FlowConfig = string.Empty;
        F_Version = string.Empty;
    }

    public NodeRedFlow(
        Guid id,
        string flowCode,
        string flowName,
        string flowConfig,
        string version,
        string? flowType = null,
        string? author = null) : base(id)
    {
        F_FlowCode = flowCode;
        F_FlowName = flowName;
        F_FlowConfig = flowConfig;
        F_Version = version;
        F_FlowType = flowType;
        F_Author = author;
        F_ExecutionPriority = 2;
        F_MaxProcessingRate = 1000;
        F_IsHotDeployEnabled = true;
        F_ExecutionCount = 0;
        F_ProcessedMessageCount = 0;
        F_ErrorMessageCount = 0;
        F_IsEnabled = true;
        F_Status = 1;
        F_SortOrder = 0;
    }
} 
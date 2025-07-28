using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 规则引擎接口
/// </summary>
public interface IRuleEngine
{
    /// <summary>
    /// 执行规则
    /// </summary>
    /// <param name="ruleCode">规则代码</param>
    /// <param name="data">输入数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则执行结果</returns>
    Task<RuleResult> ExecuteRuleAsync(string ruleCode, List<DataPoint> data, CancellationToken cancellationToken = default);

    /// <summary>
    /// 验证规则
    /// </summary>
    /// <param name="ruleCode">规则代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>验证结果</returns>
    Task<bool> ValidateRuleAsync(string ruleCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="ruleCode">规则代码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新结果</returns>
    Task<bool> UpdateRuleAsync(string ruleId, string ruleCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则列表
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则列表</returns>
    Task<List<RuleInfo>> GetRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default);
}

/// <summary>
/// 规则执行结果
/// </summary>
public class RuleResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 执行结果数据
    /// </summary>
    public List<DataPoint> OutputData { get; set; } = new();

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 执行时间（毫秒）
    /// </summary>
    public long ExecutionTimeMs { get; set; }

    /// <summary>
    /// 执行时间戳
    /// </summary>
    public DateTime ExecutionTime { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 规则ID
    /// </summary>
    public string RuleId { get; set; } = string.Empty;
}

/// <summary>
/// 规则信息
/// </summary>
public class RuleInfo
{
    /// <summary>
    /// 规则ID
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// 规则名称
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 规则描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 规则代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 是否启用
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 版本号
    /// </summary>
    public int Version { get; set; } = 1;
} 
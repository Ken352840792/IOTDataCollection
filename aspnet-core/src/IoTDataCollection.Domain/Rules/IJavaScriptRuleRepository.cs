using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Rules;

/// <summary>
/// JavaScript规则仓储接口
/// </summary>
public interface IJavaScriptRuleRepository : IRepository<JavaScriptRule, Guid>
{
    /// <summary>
    /// 根据规则编码查找规则
    /// </summary>
    /// <param name="ruleCode">规则编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>JavaScript规则实体</returns>
    Task<JavaScriptRule?> FindByCodeAsync(
        string ruleCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 根据设备ID获取规则列表
    /// </summary>
    /// <param name="deviceId">设备ID</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则列表</returns>
    Task<List<JavaScriptRule>> GetListByDeviceAsync(
        Guid? deviceId,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_Priority DESC, F_RuleName",
        string? filter = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="ruleType">规则类型过滤</param>
    /// <param name="isEnabled">是否启用过滤</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则列表</returns>
    Task<List<JavaScriptRule>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_Priority DESC, F_RuleName",
        string? filter = null,
        Guid? deviceId = null,
        string? ruleType = null,
        bool? isEnabled = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取启用的规则列表
    /// </summary>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="ruleType">规则类型过滤</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>启用的规则列表</returns>
    Task<List<JavaScriptRule>> GetEnabledRulesAsync(
        Guid? deviceId = null,
        string? ruleType = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_Priority DESC, F_RuleName",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="deviceId">设备ID过滤</param>
    /// <param name="ruleType">规则类型过滤</param>
    /// <param name="isEnabled">是否启用过滤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        Guid? deviceId = null,
        string? ruleType = null,
        bool? isEnabled = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查规则编码是否已存在
    /// </summary>
    /// <param name="ruleCode">规则编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        string ruleCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新规则执行状态
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="lastExecutionTime">最后执行时间</param>
    /// <param name="executionCount">执行次数</param>
    /// <param name="lastExecutionResult">最后执行结果</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateExecutionStatusAsync(
        Guid ruleId,
        DateTime? lastExecutionTime = null,
        long? executionCount = null,
        string? lastExecutionResult = null,
        CancellationToken cancellationToken = default);
} 
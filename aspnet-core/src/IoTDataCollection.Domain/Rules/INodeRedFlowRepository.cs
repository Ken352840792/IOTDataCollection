using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace IoTDataCollection.Rules;

/// <summary>
/// Node-RED流程仓储接口
/// </summary>
public interface INodeRedFlowRepository : IRepository<NodeRedFlow, Guid>
{
    /// <summary>
    /// 根据流程编码查找流程
    /// </summary>
    /// <param name="flowCode">流程编码</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>Node-RED流程实体</returns>
    Task<NodeRedFlow?> FindByCodeAsync(
        string flowCode, 
        bool includeDetails = true,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流程列表
    /// </summary>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="filter">过滤条件</param>
    /// <param name="flowType">流程类型过滤</param>
    /// <param name="isEnabled">是否启用过滤</param>
    /// <param name="includeDetails">是否包含详细信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>流程列表</returns>
    Task<List<NodeRedFlow>> GetListAsync(
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_Priority DESC, F_FlowName",
        string? filter = null,
        string? flowType = null,
        bool? isEnabled = null,
        bool includeDetails = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取启用的流程列表
    /// </summary>
    /// <param name="flowType">流程类型过滤</param>
    /// <param name="skipCount">跳过数量</param>
    /// <param name="maxResultCount">最大结果数</param>
    /// <param name="sorting">排序</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>启用的流程列表</returns>
    Task<List<NodeRedFlow>> GetEnabledFlowsAsync(
        string? flowType = null,
        int skipCount = 0,
        int maxResultCount = int.MaxValue,
        string sorting = "F_Priority DESC, F_FlowName",
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取流程总数
    /// </summary>
    /// <param name="filter">过滤条件</param>
    /// <param name="flowType">流程类型过滤</param>
    /// <param name="isEnabled">是否启用过滤</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>总数</returns>
    Task<long> GetCountAsync(
        string? filter = null,
        string? flowType = null,
        bool? isEnabled = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查流程编码是否已存在
    /// </summary>
    /// <param name="flowCode">流程编码</param>
    /// <param name="excludeId">排除的ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(
        string flowCode,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新流程执行状态
    /// </summary>
    /// <param name="flowId">流程ID</param>
    /// <param name="lastExecutionTime">最后执行时间</param>
    /// <param name="executionCount">执行次数</param>
    /// <param name="lastExecutionResult">最后执行结果</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateExecutionStatusAsync(
        Guid flowId,
        DateTime? lastExecutionTime = null,
        long? executionCount = null,
        string? lastExecutionResult = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 更新流程版本信息
    /// </summary>
    /// <param name="flowId">流程ID</param>
    /// <param name="version">版本号</param>
    /// <param name="flowDefinition">流程定义JSON</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task UpdateVersionAsync(
        Guid flowId,
        int version,
        string flowDefinition,
        CancellationToken cancellationToken = default);
} 
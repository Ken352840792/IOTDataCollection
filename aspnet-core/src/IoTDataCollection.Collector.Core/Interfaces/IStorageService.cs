using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Core.Interfaces;

/// <summary>
/// 存储服务接口
/// </summary>
public interface IStorageService
{
    /// <summary>
    /// 初始化存储
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>初始化结果</returns>
    Task<bool> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存数据点
    /// </summary>
    /// <param name="dataPoints">数据点列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>保存结果</returns>
    Task<bool> SaveDataPointsAsync(List<DataPoint> dataPoints, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取未发送的数据点
    /// </summary>
    /// <param name="limit">限制数量</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>数据点列表</returns>
    Task<List<DataPoint>> GetUnsentDataPointsAsync(int limit = 1000, CancellationToken cancellationToken = default);

    /// <summary>
    /// 标记数据点为已发送
    /// </summary>
    /// <param name="dataPointIds">数据点ID列表</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>标记结果</returns>
    Task<bool> MarkDataPointsAsSentAsync(List<string> dataPointIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除过期数据
    /// </summary>
    /// <param name="retentionDays">保留天数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteExpiredDataAsync(int retentionDays = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存设备配置
    /// </summary>
    /// <param name="deviceConfig">设备配置</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>保存结果</returns>
    Task<bool> SaveDeviceConfigAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取设备配置
    /// </summary>
    /// <param name="deviceCode">设备编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备配置</returns>
    Task<DeviceConfig?> GetDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有设备配置
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备配置列表</returns>
    Task<List<DeviceConfig>> GetAllDeviceConfigsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 保存规则
    /// </summary>
    /// <param name="ruleInfo">规则信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>保存结果</returns>
    Task<bool> SaveRuleAsync(RuleInfo ruleInfo, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则信息</returns>
    Task<RuleInfo?> GetRuleAsync(string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取所有规则
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>规则列表</returns>
    Task<List<RuleInfo>> GetAllRulesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除规则
    /// </summary>
    /// <param name="ruleId">规则ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>删除结果</returns>
    Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取存储统计信息
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>统计信息</returns>
    Task<StorageStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 存储统计信息
/// </summary>
public class StorageStatistics
{
    /// <summary>
    /// 总数据点数
    /// </summary>
    public long TotalDataPoints { get; set; }

    /// <summary>
    /// 未发送数据点数
    /// </summary>
    public long UnsentDataPoints { get; set; }

    /// <summary>
    /// 已发送数据点数
    /// </summary>
    public long SentDataPoints { get; set; }

    /// <summary>
    /// 设备配置数量
    /// </summary>
    public int DeviceConfigCount { get; set; }

    /// <summary>
    /// 规则数量
    /// </summary>
    public int RuleCount { get; set; }

    /// <summary>
    /// 数据库大小（字节）
    /// </summary>
    public long DatabaseSizeBytes { get; set; }

    /// <summary>
    /// 统计时间
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
} 
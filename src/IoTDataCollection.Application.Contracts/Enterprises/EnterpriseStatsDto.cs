using System.Collections.Generic;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业统计信息DTO
/// </summary>
public class EnterpriseStatsDto
{
    /// <summary>
    /// 企业总数
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 启用的企业数量
    /// </summary>
    public int EnabledCount { get; set; }

    /// <summary>
    /// 禁用的企业数量
    /// </summary>
    public int DisabledCount { get; set; }

    /// <summary>
    /// 按企业类型分组统计
    /// </summary>
    public Dictionary<string, int> CountByType { get; set; } = new();

    /// <summary>
    /// 最近创建的企业数量（最近30天）
    /// </summary>
    public int RecentlyCreatedCount { get; set; }
} 
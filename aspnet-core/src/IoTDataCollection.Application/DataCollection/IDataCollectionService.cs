using System;
using System.Threading;
using System.Threading.Tasks;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// 数据采集服务接口
/// </summary>
public interface IDataCollectionService
{
    /// <summary>
    /// 获取数据库配置信息
    /// </summary>
    /// <returns>配置信息</returns>
    Task<DatabaseConfigurationInfo> GetDatabaseConfigurationInfoAsync();

    /// <summary>
    /// 测试数据库连接状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>连接测试结果</returns>
    Task<DatabaseConnectionStatus> TestDatabaseConnectionsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 数据库配置信息
/// </summary>
public class DatabaseConfigurationInfo
{
    /// <summary>
    /// MySQL连接字符串（隐藏密码）
    /// </summary>
    public string MySqlConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// InfluxDB连接字符串
    /// </summary>
    public string InfluxDbConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// InfluxDB组织
    /// </summary>
    public string InfluxDbOrganization { get; set; } = string.Empty;

    /// <summary>
    /// InfluxDB Bucket
    /// </summary>
    public string InfluxDbBucket { get; set; } = string.Empty;

    /// <summary>
    /// Redis连接字符串
    /// </summary>
    public string RedisConnectionString { get; set; } = string.Empty;
}

/// <summary>
/// 数据库连接状态
/// </summary>
public class DatabaseConnectionStatus
{
    /// <summary>
    /// MySQL连接状态
    /// </summary>
    public bool MySqlConnection { get; set; }

    /// <summary>
    /// InfluxDB连接状态
    /// </summary>
    public bool InfluxDbConnection { get; set; }

    /// <summary>
    /// Redis连接状态
    /// </summary>
    public bool RedisConnection { get; set; }

    /// <summary>
    /// 总体连接状态
    /// </summary>
    public bool AllConnections => MySqlConnection && InfluxDbConnection && RedisConnection;

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
} 
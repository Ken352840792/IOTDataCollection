using System.Threading;
using System.Threading.Tasks;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// InfluxDB初始化服务接口
/// </summary>
public interface IInfluxDbInitializationService
{
    /// <summary>
    /// 初始化InfluxDB数据库
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>初始化结果</returns>
    Task<InfluxDbInitializationResult> InitializeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 检查InfluxDB状态
    /// </summary>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>状态信息</returns>
    Task<InfluxDbStatusInfo> GetStatusAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// InfluxDB初始化结果
/// </summary>
public class InfluxDbInitializationResult
{
    /// <summary>
    /// 是否成功
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// 组织是否创建
    /// </summary>
    public bool OrganizationCreated { get; set; }

    /// <summary>
    /// Bucket是否创建
    /// </summary>
    public bool BucketCreated { get; set; }

    /// <summary>
    /// 连接是否正常
    /// </summary>
    public bool ConnectionOk { get; set; }
}

/// <summary>
/// InfluxDB状态信息
/// </summary>
public class InfluxDbStatusInfo
{
    /// <summary>
    /// 连接状态
    /// </summary>
    public bool ConnectionOk { get; set; }

    /// <summary>
    /// 组织名称
    /// </summary>
    public string Organization { get; set; } = string.Empty;

    /// <summary>
    /// Bucket名称
    /// </summary>
    public string Bucket { get; set; } = string.Empty;

    /// <summary>
    /// 组织是否存在
    /// </summary>
    public bool OrganizationExists { get; set; }

    /// <summary>
    /// Bucket是否存在
    /// </summary>
    public bool BucketExists { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public string? ErrorMessage { get; set; }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// 数据采集服务实现
/// </summary>
public class DataCollectionService : IDataCollectionService, ITransientDependency
{
    private readonly ILogger<DataCollectionService> _logger;
    private readonly IConfiguration _configuration;

    public DataCollectionService(
        ILogger<DataCollectionService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<DatabaseConfigurationInfo> GetDatabaseConfigurationInfoAsync()
    {
        try
        {
            var mySqlConnectionString = _configuration.GetConnectionString("Default") ?? string.Empty;
            var influxDbConnectionString = _configuration.GetConnectionString("InfluxDB") ?? string.Empty;
            var redisConnectionString = _configuration.GetConnectionString("Redis") ?? 
                                      _configuration["Redis:Configuration"] ?? string.Empty;
            var influxDbOrganization = _configuration["InfluxDB:Organization"] ?? string.Empty;
            var influxDbBucket = _configuration["InfluxDB:Bucket"] ?? string.Empty;

            // 隐藏MySQL密码
            if (!string.IsNullOrEmpty(mySqlConnectionString) && mySqlConnectionString.Contains("Pwd="))
            {
                var parts = mySqlConnectionString.Split(';');
                for (int i = 0; i < parts.Length; i++)
                {
                    if (parts[i].Trim().StartsWith("Pwd="))
                    {
                        parts[i] = "Pwd=***";
                        break;
                    }
                }
                mySqlConnectionString = string.Join(";", parts);
            }

            return new DatabaseConfigurationInfo
            {
                MySqlConnectionString = mySqlConnectionString,
                InfluxDbConnectionString = influxDbConnectionString,
                InfluxDbOrganization = influxDbOrganization,
                InfluxDbBucket = influxDbBucket,
                RedisConnectionString = redisConnectionString
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取数据库配置信息失败");
            return new DatabaseConfigurationInfo();
        }
    }

    public async Task<DatabaseConnectionStatus> TestDatabaseConnectionsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("开始测试数据库连接状态...");

            // 注意：实际的数据库连接测试应该在专门的测试项目中实现
            // 这里只返回配置状态，不进行实际的连接测试
            var config = await GetDatabaseConfigurationInfoAsync();
            
            var status = new DatabaseConnectionStatus
            {
                MySqlConnection = !string.IsNullOrEmpty(config.MySqlConnectionString),
                InfluxDbConnection = !string.IsNullOrEmpty(config.InfluxDbConnectionString),
                RedisConnection = !string.IsNullOrEmpty(config.RedisConnectionString),
                ErrorMessage = "数据库连接测试需要在专门的测试项目中执行"
            };

            _logger.LogInformation("数据库配置状态检查完成: MySQL={MySql}, InfluxDB={InfluxDb}, Redis={Redis}", 
                status.MySqlConnection, status.InfluxDbConnection, status.RedisConnection);

            return status;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据库连接状态测试异常");
            return new DatabaseConnectionStatus
            {
                ErrorMessage = $"测试异常: {ex.Message}"
            };
        }
    }
} 
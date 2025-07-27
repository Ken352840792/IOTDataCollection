using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// InfluxDB初始化服务实现
/// </summary>
public class InfluxDbInitializationService : IInfluxDbInitializationService, ITransientDependency
{
    private readonly ILogger<InfluxDbInitializationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly InfluxDBClient _influxDbClient;
    private readonly string _organization;
    private readonly string _bucket;

    public InfluxDbInitializationService(
        ILogger<InfluxDbInitializationService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
        
        var connectionString = _configuration.GetConnectionString("InfluxDB") ?? 
                              "http://localhost:8086";
        var token = _configuration["InfluxDB:Token"] ?? 
                   "iot-admin-token-12345678901234567890";
        _bucket = _configuration["InfluxDB:Bucket"] ?? "iot_data";
        _organization = _configuration["InfluxDB:Organization"] ?? "IOTDataCollection";

        _influxDbClient = new InfluxDBClient(connectionString, token);
    }

    public async Task<InfluxDbInitializationResult> InitializeAsync(CancellationToken cancellationToken = default)
    {
        var result = new InfluxDbInitializationResult();

        try
        {
            _logger.LogInformation("开始初始化InfluxDB数据库...");

            // 1. 测试连接
            var pingResult = await _influxDbClient.PingAsync();
            result.ConnectionOk = true;
            _logger.LogInformation("InfluxDB连接测试成功，响应时间: {ResponseTime}ms", pingResult);

            // 2. 确保组织存在
            var organizationsApi = _influxDbClient.GetOrganizationsApi();
            var organizations = await organizationsApi.FindOrganizationsAsync();
            var organization = organizations.FirstOrDefault(o => o.Name == _organization);
            
            if (organization == null)
            {
                _logger.LogInformation("创建InfluxDB组织: {Organization}", _organization);
                organization = await organizationsApi.CreateOrganizationAsync(_organization);
                result.OrganizationCreated = true;
            }
            else
            {
                _logger.LogDebug("InfluxDB组织已存在: {Organization}", _organization);
            }

            // 3. 确保Bucket存在
            var bucketsApi = _influxDbClient.GetBucketsApi();
            var buckets = await bucketsApi.FindBucketsAsync();
            var bucket = buckets.FirstOrDefault(b => b.Name == _bucket);
            
            if (bucket == null)
            {
                _logger.LogInformation("创建InfluxDB Bucket: {Bucket}", _bucket);
                await bucketsApi.CreateBucketAsync(_bucket, organization);
                result.BucketCreated = true;
            }
            else
            {
                _logger.LogDebug("InfluxDB Bucket已存在: {Bucket}", _bucket);
            }

            result.Success = true;
            _logger.LogInformation("InfluxDB数据库初始化完成");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            _logger.LogError(ex, "InfluxDB数据库初始化失败");
        }

        return result;
    }

    public async Task<InfluxDbStatusInfo> GetStatusAsync(CancellationToken cancellationToken = default)
    {
        var status = new InfluxDbStatusInfo
        {
            Organization = _organization,
            Bucket = _bucket
        };

        try
        {
            // 1. 测试连接
            var pingResult = await _influxDbClient.PingAsync();
            status.ConnectionOk = true;

            // 2. 检查组织是否存在
            var organizationsApi = _influxDbClient.GetOrganizationsApi();
            var organizations = await organizationsApi.FindOrganizationsAsync();
            var organization = organizations.FirstOrDefault(o => o.Name == _organization);
            status.OrganizationExists = organization != null;

            // 3. 检查Bucket是否存在
            var bucketsApi = _influxDbClient.GetBucketsApi();
            var buckets = await bucketsApi.FindBucketsAsync();
            var bucket = buckets.FirstOrDefault(b => b.Name == _bucket);
            status.BucketExists = bucket != null;

            _logger.LogDebug("InfluxDB状态检查完成: 连接={Connection}, 组织={OrgExists}, Bucket={BucketExists}", 
                status.ConnectionOk, status.OrganizationExists, status.BucketExists);
        }
        catch (Exception ex)
        {
            status.ConnectionOk = false;
            status.ErrorMessage = ex.Message;
            _logger.LogError(ex, "InfluxDB状态检查失败");
        }

        return status;
    }
} 
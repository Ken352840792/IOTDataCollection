using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Writes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;
using IoTDataCollection.TimeSeriesData;

namespace IoTDataCollection.EntityFrameworkCore.TimeSeriesData;

/// <summary>
/// InfluxDB时序数据仓储实现
/// </summary>
public class InfluxDbTimeSeriesRepository : ITimeSeriesRepository, ITransientDependency, IDisposable
{
    private readonly ILogger<InfluxDbTimeSeriesRepository> _logger;
    private readonly IConfiguration _configuration;
    private readonly InfluxDBClient _influxDbClient;
    private readonly string _bucket;
    private readonly string _organization;

    public InfluxDbTimeSeriesRepository(
        ILogger<InfluxDbTimeSeriesRepository> logger,
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

    #region 设备数据点操作

    public async Task WriteDeviceDataPointAsync(DeviceDataPoint dataPoint, CancellationToken cancellationToken = default)
    {
        try
        {
            var writeApi = _influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementAsync(dataPoint, WritePrecision.Ms, _bucket, _organization, cancellationToken);
            
            _logger.LogDebug("写入设备数据点成功: {DeviceCode}.{PointCode} = {Value}", 
                dataPoint.DeviceCode, dataPoint.PointCode, dataPoint.NumericValue?.ToString() ?? dataPoint.StringValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写入设备数据点失败: {DeviceCode}.{PointCode}", 
                dataPoint.DeviceCode, dataPoint.PointCode);
            throw;
        }
    }

    public async Task WriteDeviceDataPointsAsync(IEnumerable<DeviceDataPoint> dataPoints, CancellationToken cancellationToken = default)
    {
        try
        {
            var writeApi = _influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementsAsync<DeviceDataPoint>(dataPoints.ToList(), WritePrecision.Ms, _bucket, _organization, cancellationToken);
            
            _logger.LogDebug("批量写入设备数据点成功，数量: {Count}", dataPoints.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量写入设备数据点失败，数量: {Count}", dataPoints.Count());
            throw;
        }
    }

    public async Task<List<DeviceDataPoint>> QueryDeviceDataAsync(
        string deviceCode, 
        string pointCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: {startTime:yyyy-MM-ddTHH:mm:ssZ}, stop: {endTime:yyyy-MM-ddTHH:mm:ssZ})
                  |> filter(fn: (r) => r[""_measurement""] == ""device_data"")
                  |> filter(fn: (r) => r[""device_code""] == ""{deviceCode}"")
                  |> filter(fn: (r) => r[""point_code""] == ""{pointCode}"")
                  |> sort(columns: [""_time""])";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<DeviceDataPoint>(query, _organization, cancellationToken);
            
            _logger.LogDebug("查询设备数据成功: {DeviceCode}.{PointCode}, 记录数: {Count}", 
                deviceCode, pointCode, result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询设备数据失败: {DeviceCode}.{PointCode}", deviceCode, pointCode);
            throw;
        }
    }

    public async Task<DeviceDataPoint?> QueryLatestDeviceDataAsync(
        string deviceCode, 
        string pointCode, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -24h)
                  |> filter(fn: (r) => r[""_measurement""] == ""device_data"")
                  |> filter(fn: (r) => r[""device_code""] == ""{deviceCode}"")
                  |> filter(fn: (r) => r[""point_code""] == ""{pointCode}"")
                  |> last()";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<DeviceDataPoint>(query, _organization, cancellationToken);
            
            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询设备最新数据失败: {DeviceCode}.{PointCode}", deviceCode, pointCode);
            throw;
        }
    }

    public async Task<List<DeviceDataPoint>> QueryLatestSiteDataAsync(
        string siteCode, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1h)
                  |> filter(fn: (r) => r[""_measurement""] == ""device_data"")
                  |> filter(fn: (r) => r[""site_code""] == ""{siteCode}"")
                  |> group(columns: [""device_code"", ""point_code""])
                  |> last()";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<DeviceDataPoint>(query, _organization, cancellationToken);
            
            _logger.LogDebug("查询站点最新数据成功: {SiteCode}, 记录数: {Count}", siteCode, result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询站点最新数据失败: {SiteCode}", siteCode);
            throw;
        }
    }

    public async Task<DeviceDataStatistics> QueryDeviceDataStatisticsAsync(
        string deviceCode, 
        string pointCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                data = from(bucket: ""{_bucket}"")
                  |> range(start: {startTime:yyyy-MM-ddTHH:mm:ssZ}, stop: {endTime:yyyy-MM-ddTHH:mm:ssZ})
                  |> filter(fn: (r) => r[""_measurement""] == ""device_data"")
                  |> filter(fn: (r) => r[""device_code""] == ""{deviceCode}"")
                  |> filter(fn: (r) => r[""point_code""] == ""{pointCode}"")
                  |> filter(fn: (r) => r[""_field""] == ""numeric_value"")

                count = data |> count() |> yield(name: ""count"")
                min = data |> min() |> yield(name: ""min"")  
                max = data |> max() |> yield(name: ""max"")
                mean = data |> mean() |> yield(name: ""mean"")
                first = data |> first() |> yield(name: ""first"")
                last = data |> last() |> yield(name: ""last"")";

            var queryApi = _influxDbClient.GetQueryApi();
            var tables = await queryApi.QueryAsync(query, _organization, cancellationToken);

            var statistics = new DeviceDataStatistics();
            
            foreach (var table in tables)
            {
                foreach (var record in table.Records)
                {
                    var resultName = record.GetValueByKey("result");
                    var value = record.GetValue();
                    var time = record.GetTime();

                    switch (resultName)
                    {
                        case "count":
                            statistics.Count = Convert.ToInt64(value);
                            break;
                        case "min":
                            statistics.MinValue = Convert.ToDouble(value);
                            break;
                        case "max":
                            statistics.MaxValue = Convert.ToDouble(value);
                            break;
                        case "mean":
                            statistics.AverageValue = Convert.ToDouble(value);
                            break;
                        case "first":
                            statistics.FirstTime = time?.ToDateTimeUtc();
                            break;
                        case "last":
                            statistics.LastTime = time?.ToDateTimeUtc();
                            break;
                    }
                }
            }

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询设备数据统计失败: {DeviceCode}.{PointCode}", deviceCode, pointCode);
            throw;
        }
    }

    #endregion

    #region 系统监控数据操作

    public async Task WriteSystemMonitoringDataAsync(SystemMonitoringData monitoringData, CancellationToken cancellationToken = default)
    {
        try
        {
            var writeApi = _influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementAsync(monitoringData, WritePrecision.Ms, _bucket, _organization, cancellationToken);
            
            _logger.LogDebug("写入系统监控数据成功: {NodeCode}, CPU: {CpuUsage}%, Memory: {MemoryUsage}%", 
                monitoringData.NodeCode, monitoringData.CpuUsage, monitoringData.MemoryUsage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "写入系统监控数据失败: {NodeCode}", monitoringData.NodeCode);
            throw;
        }
    }

    public async Task WriteSystemMonitoringDataAsync(IEnumerable<SystemMonitoringData> monitoringDataList, CancellationToken cancellationToken = default)
    {
        try
        {
            var writeApi = _influxDbClient.GetWriteApiAsync();
            await writeApi.WriteMeasurementsAsync<SystemMonitoringData>(monitoringDataList.ToList(), WritePrecision.Ms, _bucket, _organization, cancellationToken);
            
            _logger.LogDebug("批量写入系统监控数据成功，数量: {Count}", monitoringDataList.Count());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "批量写入系统监控数据失败，数量: {Count}", monitoringDataList.Count());
            throw;
        }
    }

    public async Task<List<SystemMonitoringData>> QuerySystemMonitoringDataAsync(
        string nodeCode, 
        DateTime startTime, 
        DateTime endTime, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: {startTime:yyyy-MM-ddTHH:mm:ssZ}, stop: {endTime:yyyy-MM-ddTHH:mm:ssZ})
                  |> filter(fn: (r) => r[""_measurement""] == ""system_monitoring"")
                  |> filter(fn: (r) => r[""node_code""] == ""{nodeCode}"")
                  |> sort(columns: [""_time""])";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<SystemMonitoringData>(query, _organization, cancellationToken);
            
            _logger.LogDebug("查询系统监控数据成功: {NodeCode}, 记录数: {Count}", nodeCode, result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询系统监控数据失败: {NodeCode}", nodeCode);
            throw;
        }
    }

    public async Task<SystemMonitoringData?> QueryLatestSystemMonitoringDataAsync(
        string nodeCode, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1h)
                  |> filter(fn: (r) => r[""_measurement""] == ""system_monitoring"")
                  |> filter(fn: (r) => r[""node_code""] == ""{nodeCode}"")
                  |> last()";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<SystemMonitoringData>(query, _organization, cancellationToken);
            
            return result.FirstOrDefault();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询节点最新监控数据失败: {NodeCode}", nodeCode);
            throw;
        }
    }

    public async Task<List<SystemMonitoringData>> QueryAllNodesLatestMonitoringDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var query = $@"
                from(bucket: ""{_bucket}"")
                  |> range(start: -1h)
                  |> filter(fn: (r) => r[""_measurement""] == ""system_monitoring"")
                  |> group(columns: [""node_code""])
                  |> last()";

            var queryApi = _influxDbClient.GetQueryApi();
            var result = await queryApi.QueryAsync<SystemMonitoringData>(query, _organization, cancellationToken);
            
            _logger.LogDebug("查询所有节点最新监控数据成功，节点数: {Count}", result.Count);
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "查询所有节点最新监控数据失败");
            throw;
        }
    }

    #endregion

    #region 数据库管理

    public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var pingResult = await _influxDbClient.PingAsync();
            _logger.LogInformation("InfluxDB连接测试成功，响应时间: {ResponseTime}ms", pingResult);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InfluxDB连接测试失败");
            return false;
        }
    }

    public async Task EnsureDatabaseAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var bucketsApi = _influxDbClient.GetBucketsApi();
            var bucket = await bucketsApi.FindBucketByNameAsync(_bucket, cancellationToken);
            
            if (bucket == null)
            {
                _logger.LogInformation("创建InfluxDB Bucket: {Bucket}", _bucket);
                await bucketsApi.CreateBucketAsync(_bucket, _organization, cancellationToken);
            }
            else
            {
                _logger.LogDebug("InfluxDB Bucket已存在: {Bucket}", _bucket);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "确保InfluxDB数据库存在失败");
            throw;
        }
    }

    public async Task DeleteExpiredDataAsync(int retentionDays, CancellationToken cancellationToken = default)
    {
        try
        {
            var deleteApi = _influxDbClient.GetDeleteApi();
            var start = DateTime.UtcNow.AddDays(-retentionDays - 365); // 删除超过保留期的数据
            var stop = DateTime.UtcNow.AddDays(-retentionDays);

            await deleteApi.Delete(start, stop, "", _bucket, _organization, cancellationToken);
            
            _logger.LogInformation("删除过期数据成功，保留天数: {RetentionDays}", retentionDays);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除过期数据失败，保留天数: {RetentionDays}", retentionDays);
            throw;
        }
    }

    #endregion

    public void Dispose()
    {
        _influxDbClient?.Dispose();
    }
} 
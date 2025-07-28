using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;

namespace IoTDataCollection.Collector.Storage;

/// <summary>
/// SQLite存储服务
/// </summary>
public class SqliteStorageService : IStorageService, IDisposable
{
    private readonly ILogger<SqliteStorageService> _logger;
    private readonly StorageConfiguration _config;
    private readonly string _connectionString;
    private readonly SqliteConnection _connection;

    public SqliteStorageService(
        ILogger<SqliteStorageService> logger,
        IOptions<StorageConfiguration> config)
    {
        _logger = logger;
        _config = config.Value;
        _connectionString = $"Data Source={_config.DatabasePath};Mode=ReadWriteCreate;";
        _connection = new SqliteConnection(_connectionString);
    }

    /// <summary>
    /// 初始化存储
    /// </summary>
    public async Task<bool> InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("初始化SQLite存储，数据库路径: {Path}", _config.DatabasePath);

            // 确保目录存在
            var directory = Path.GetDirectoryName(_config.DatabasePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            await _connection.OpenAsync(cancellationToken);

            // 创建表结构
            await CreateTablesAsync(cancellationToken);

            _logger.LogInformation("SQLite存储初始化成功");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SQLite存储初始化失败");
            return false;
        }
    }

    /// <summary>
    /// 保存数据点
    /// </summary>
    public async Task<bool> SaveDataPointsAsync(List<DataPoint> dataPoints, CancellationToken cancellationToken = default)
    {
        if (dataPoints == null || dataPoints.Count == 0)
        {
            return true;
        }

        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            using var transaction = _connection.BeginTransaction();
            try
            {
                var command = _connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    INSERT INTO DataPoints (Id, DeviceCode, PointCode, PointName, Address, DataType, 
                                          NumericValue, StringValue, BooleanValue, RawValue, Quality, 
                                          Unit, Timestamp, IsSent)
                    VALUES (@Id, @DeviceCode, @PointCode, @PointName, @Address, @DataType,
                           @NumericValue, @StringValue, @BooleanValue, @RawValue, @Quality,
                           @Unit, @Timestamp, @IsSent)";

                foreach (var dataPoint in dataPoints)
                {
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@Id", dataPoint.Id);
                    command.Parameters.AddWithValue("@DeviceCode", dataPoint.DeviceCode);
                    command.Parameters.AddWithValue("@PointCode", dataPoint.PointCode);
                    command.Parameters.AddWithValue("@PointName", dataPoint.PointName);
                    command.Parameters.AddWithValue("@Address", dataPoint.Address);
                    command.Parameters.AddWithValue("@DataType", (int)dataPoint.DataType);
                    command.Parameters.AddWithValue("@NumericValue", dataPoint.NumericValue ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@StringValue", dataPoint.StringValue ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@BooleanValue", dataPoint.BooleanValue ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@RawValue", dataPoint.RawValue ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Quality", dataPoint.Quality);
                    command.Parameters.AddWithValue("@Unit", dataPoint.Unit ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Timestamp", dataPoint.Timestamp);
                    command.Parameters.AddWithValue("@IsSent", false);

                    await command.ExecuteNonQueryAsync(cancellationToken);
                }

                transaction.Commit();
                _logger.LogDebug("保存数据点成功，数量: {Count}", dataPoints.Count);
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存数据点失败");
            return false;
        }
    }

    /// <summary>
    /// 获取未发送的数据点
    /// </summary>
    public async Task<List<DataPoint>> GetUnsentDataPointsAsync(int limit = 1000, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, DeviceCode, PointCode, PointName, Address, DataType,
                       NumericValue, StringValue, BooleanValue, RawValue, Quality,
                       Unit, Timestamp, IsSent
                FROM DataPoints 
                WHERE IsSent = 0 
                ORDER BY Timestamp ASC 
                LIMIT @Limit";

            command.Parameters.AddWithValue("@Limit", limit);

            var dataPoints = new List<DataPoint>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var dataPoint = new DataPoint
                {
                    Id = reader.GetString("Id"),
                    DeviceCode = reader.GetString("DeviceCode"),
                    PointCode = reader.GetString("PointCode"),
                    PointName = reader.GetString("PointName"),
                    Address = reader.GetString("Address"),
                    DataType = (DataType)reader.GetInt32("DataType"),
                    Quality = reader.GetInt32("Quality"),
                    Timestamp = reader.GetDateTime("Timestamp"),
                    IsEnabled = true
                };

                // 设置值
                if (!reader.IsDBNull("NumericValue"))
                    dataPoint.NumericValue = reader.GetDouble("NumericValue");
                if (!reader.IsDBNull("StringValue"))
                    dataPoint.StringValue = reader.GetString("StringValue");
                if (!reader.IsDBNull("BooleanValue"))
                    dataPoint.BooleanValue = reader.GetBoolean("BooleanValue");
                if (!reader.IsDBNull("RawValue"))
                    dataPoint.RawValue = reader.GetString("RawValue");
                if (!reader.IsDBNull("Unit"))
                    dataPoint.Unit = reader.GetString("Unit");

                dataPoints.Add(dataPoint);
            }

            _logger.LogDebug("获取未发送数据点成功，数量: {Count}", dataPoints.Count);
            return dataPoints;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取未发送数据点失败");
            return new List<DataPoint>();
        }
    }

    /// <summary>
    /// 标记数据点为已发送
    /// </summary>
    public async Task<bool> MarkDataPointsAsSentAsync(List<string> dataPointIds, CancellationToken cancellationToken = default)
    {
        if (dataPointIds == null || dataPointIds.Count == 0)
        {
            return true;
        }

        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                UPDATE DataPoints 
                SET IsSent = 1 
                WHERE Id IN (" + string.Join(",", dataPointIds.Select((_, i) => $"@Id{i}")) + ")";

            for (int i = 0; i < dataPointIds.Count; i++)
            {
                command.Parameters.AddWithValue($"@Id{i}", dataPointIds[i]);
            }

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogDebug("标记数据点为已发送成功，数量: {Count}", affectedRows);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "标记数据点为已发送失败");
            return false;
        }
    }

    /// <summary>
    /// 删除过期数据
    /// </summary>
    public async Task<bool> DeleteExpiredDataAsync(int retentionDays = 30, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM DataPoints WHERE Timestamp < @CutoffDate";
            command.Parameters.AddWithValue("@CutoffDate", cutoffDate);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogInformation("删除过期数据成功，删除数量: {Count}, 保留天数: {Days}", affectedRows, retentionDays);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除过期数据失败");
            return false;
        }
    }

    /// <summary>
    /// 保存设备配置
    /// </summary>
    public async Task<bool> SaveDeviceConfigAsync(DeviceConfig deviceConfig, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO DeviceConfigs (DeviceCode, DeviceName, DeviceType, ConnectionConfig, 
                                                    DataPoints, SamplingInterval, IsEnabled, CreatedAt, UpdatedAt)
                VALUES (@DeviceCode, @DeviceName, @DeviceType, @ConnectionConfig,
                       @DataPoints, @SamplingInterval, @IsEnabled, @CreatedAt, @UpdatedAt)";

            command.Parameters.AddWithValue("@DeviceCode", deviceConfig.DeviceCode);
            command.Parameters.AddWithValue("@DeviceName", deviceConfig.DeviceName);
            command.Parameters.AddWithValue("@DeviceType", (int)deviceConfig.DeviceType);
            command.Parameters.AddWithValue("@ConnectionConfig", JsonConvert.SerializeObject(deviceConfig.ConnectionConfig));
            command.Parameters.AddWithValue("@DataPoints", JsonConvert.SerializeObject(deviceConfig.DataPoints));
            command.Parameters.AddWithValue("@SamplingInterval", deviceConfig.SamplingInterval);
            command.Parameters.AddWithValue("@IsEnabled", deviceConfig.IsEnabled);
            command.Parameters.AddWithValue("@CreatedAt", deviceConfig.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", deviceConfig.UpdatedAt);

            await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogDebug("保存设备配置成功: {DeviceCode}", deviceConfig.DeviceCode);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存设备配置失败: {DeviceCode}", deviceConfig.DeviceCode);
            return false;
        }
    }

    /// <summary>
    /// 获取设备配置
    /// </summary>
    public async Task<DeviceConfig?> GetDeviceConfigAsync(string deviceCode, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT DeviceCode, DeviceName, DeviceType, ConnectionConfig, 
                       DataPoints, SamplingInterval, IsEnabled, CreatedAt, UpdatedAt
                FROM DeviceConfigs 
                WHERE DeviceCode = @DeviceCode";

            command.Parameters.AddWithValue("@DeviceCode", deviceCode);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                var deviceConfig = new DeviceConfig
                {
                    DeviceCode = reader.GetString("DeviceCode"),
                    DeviceName = reader.GetString("DeviceName"),
                    DeviceType = (DeviceType)reader.GetInt32("DeviceType"),
                    SamplingInterval = reader.GetInt32("SamplingInterval"),
                    IsEnabled = reader.GetBoolean("IsEnabled"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };

                // 反序列化配置
                var connectionConfigJson = reader.GetString("ConnectionConfig");
                deviceConfig.ConnectionConfig = JsonConvert.DeserializeObject<ConnectionConfig>(connectionConfigJson) ?? new ConnectionConfig();

                var dataPointsJson = reader.GetString("DataPoints");
                deviceConfig.DataPoints = JsonConvert.DeserializeObject<List<DataPointConfig>>(dataPointsJson) ?? new List<DataPointConfig>();

                return deviceConfig;
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取设备配置失败: {DeviceCode}", deviceCode);
            return null;
        }
    }

    /// <summary>
    /// 获取所有设备配置
    /// </summary>
    public async Task<List<DeviceConfig>> GetAllDeviceConfigsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT DeviceCode, DeviceName, DeviceType, ConnectionConfig, 
                       DataPoints, SamplingInterval, IsEnabled, CreatedAt, UpdatedAt
                FROM DeviceConfigs";

            var deviceConfigs = new List<DeviceConfig>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var deviceConfig = new DeviceConfig
                {
                    DeviceCode = reader.GetString("DeviceCode"),
                    DeviceName = reader.GetString("DeviceName"),
                    DeviceType = (DeviceType)reader.GetInt32("DeviceType"),
                    SamplingInterval = reader.GetInt32("SamplingInterval"),
                    IsEnabled = reader.GetBoolean("IsEnabled"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt")
                };

                // 反序列化配置
                var connectionConfigJson = reader.GetString("ConnectionConfig");
                deviceConfig.ConnectionConfig = JsonConvert.DeserializeObject<ConnectionConfig>(connectionConfigJson) ?? new ConnectionConfig();

                var dataPointsJson = reader.GetString("DataPoints");
                deviceConfig.DataPoints = JsonConvert.DeserializeObject<List<DataPointConfig>>(dataPointsJson) ?? new List<DataPointConfig>();

                deviceConfigs.Add(deviceConfig);
            }

            return deviceConfigs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有设备配置失败");
            return new List<DeviceConfig>();
        }
    }

    /// <summary>
    /// 保存规则
    /// </summary>
    public async Task<bool> SaveRuleAsync(RuleInfo ruleInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                INSERT OR REPLACE INTO Rules (Id, Name, Description, Code, IsEnabled, CreatedAt, UpdatedAt, Version)
                VALUES (@Id, @Name, @Description, @Code, @IsEnabled, @CreatedAt, @UpdatedAt, @Version)";

            command.Parameters.AddWithValue("@Id", ruleInfo.Id);
            command.Parameters.AddWithValue("@Name", ruleInfo.Name);
            command.Parameters.AddWithValue("@Description", ruleInfo.Description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Code", ruleInfo.Code);
            command.Parameters.AddWithValue("@IsEnabled", ruleInfo.IsEnabled);
            command.Parameters.AddWithValue("@CreatedAt", ruleInfo.CreatedAt);
            command.Parameters.AddWithValue("@UpdatedAt", ruleInfo.UpdatedAt);
            command.Parameters.AddWithValue("@Version", ruleInfo.Version);

            await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogDebug("保存规则成功: {RuleId}", ruleInfo.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "保存规则失败: {RuleId}", ruleInfo.Id);
            return false;
        }
    }

    /// <summary>
    /// 获取规则
    /// </summary>
    public async Task<RuleInfo?> GetRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Description, Code, IsEnabled, CreatedAt, UpdatedAt, Version
                FROM Rules 
                WHERE Id = @RuleId";

            command.Parameters.AddWithValue("@RuleId", ruleId);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                return new RuleInfo
                {
                    Id = reader.GetString("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Code = reader.GetString("Code"),
                    IsEnabled = reader.GetBoolean("IsEnabled"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt"),
                    Version = reader.GetInt32("Version")
                };
            }

            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取规则失败: {RuleId}", ruleId);
            return null;
        }
    }

    /// <summary>
    /// 获取所有规则
    /// </summary>
    public async Task<List<RuleInfo>> GetAllRulesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = @"
                SELECT Id, Name, Description, Code, IsEnabled, CreatedAt, UpdatedAt, Version
                FROM Rules";

            var rules = new List<RuleInfo>();
            using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                rules.Add(new RuleInfo
                {
                    Id = reader.GetString("Id"),
                    Name = reader.GetString("Name"),
                    Description = reader.IsDBNull("Description") ? null : reader.GetString("Description"),
                    Code = reader.GetString("Code"),
                    IsEnabled = reader.GetBoolean("IsEnabled"),
                    CreatedAt = reader.GetDateTime("CreatedAt"),
                    UpdatedAt = reader.GetDateTime("UpdatedAt"),
                    Version = reader.GetInt32("Version")
                });
            }

            return rules;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取所有规则失败");
            return new List<RuleInfo>();
        }
    }

    /// <summary>
    /// 删除规则
    /// </summary>
    public async Task<bool> DeleteRuleAsync(string ruleId, CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var command = _connection.CreateCommand();
            command.CommandText = "DELETE FROM Rules WHERE Id = @RuleId";
            command.Parameters.AddWithValue("@RuleId", ruleId);

            var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);
            _logger.LogDebug("删除规则成功: {RuleId}, 影响行数: {Count}", ruleId, affectedRows);
            return affectedRows > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "删除规则失败: {RuleId}", ruleId);
            return false;
        }
    }

    /// <summary>
    /// 获取存储统计信息
    /// </summary>
    public async Task<StorageStatistics> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // 确保连接已打开
            await EnsureConnectionOpenAsync(cancellationToken);

            var statistics = new StorageStatistics();

            // 获取数据点统计
            var dataPointsCommand = _connection.CreateCommand();
            dataPointsCommand.CommandText = @"
                SELECT 
                    COUNT(*) as TotalCount,
                    SUM(CASE WHEN IsSent = 0 THEN 1 ELSE 0 END) as UnsentCount,
                    SUM(CASE WHEN IsSent = 1 THEN 1 ELSE 0 END) as SentCount
                FROM DataPoints";

            using (var reader = await dataPointsCommand.ExecuteReaderAsync(cancellationToken))
            {
                if (await reader.ReadAsync(cancellationToken))
                {
                    statistics.TotalDataPoints = reader.GetInt64("TotalCount");
                    statistics.UnsentDataPoints = reader.GetInt64("UnsentCount");
                    statistics.SentDataPoints = reader.GetInt64("SentCount");
                }
            }

            // 获取设备配置数量
            var deviceConfigCommand = _connection.CreateCommand();
            deviceConfigCommand.CommandText = "SELECT COUNT(*) FROM DeviceConfigs";
            statistics.DeviceConfigCount = Convert.ToInt32(await deviceConfigCommand.ExecuteScalarAsync(cancellationToken));

            // 获取规则数量
            var rulesCommand = _connection.CreateCommand();
            rulesCommand.CommandText = "SELECT COUNT(*) FROM Rules";
            statistics.RuleCount = Convert.ToInt32(await rulesCommand.ExecuteScalarAsync(cancellationToken));

            // 获取数据库大小
            if (File.Exists(_config.DatabasePath))
            {
                var fileInfo = new FileInfo(_config.DatabasePath);
                statistics.DatabaseSizeBytes = fileInfo.Length;
            }

            statistics.Timestamp = DateTime.UtcNow;

            return statistics;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取存储统计信息失败");
            return new StorageStatistics();
        }
    }

    /// <summary>
    /// 确保连接已打开
    /// </summary>
    private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
    {
        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _logger.LogDebug("数据库连接未打开，正在重新打开连接");
            await _connection.OpenAsync(cancellationToken);
        }
    }

    /// <summary>
    /// 创建表结构
    /// </summary>
    private async Task CreateTablesAsync(CancellationToken cancellationToken)
    {
        // 创建数据点表
        var createDataPointsTable = @"
            CREATE TABLE IF NOT EXISTS DataPoints (
                Id TEXT PRIMARY KEY,
                DeviceCode TEXT NOT NULL,
                PointCode TEXT NOT NULL,
                PointName TEXT NOT NULL,
                Address TEXT NOT NULL,
                DataType INTEGER NOT NULL,
                NumericValue REAL,
                StringValue TEXT,
                BooleanValue INTEGER,
                RawValue TEXT,
                Quality INTEGER NOT NULL,
                Unit TEXT,
                Timestamp TEXT NOT NULL,
                IsSent INTEGER NOT NULL DEFAULT 0
            )";

        // 创建设备配置表
        var createDeviceConfigsTable = @"
            CREATE TABLE IF NOT EXISTS DeviceConfigs (
                DeviceCode TEXT PRIMARY KEY,
                DeviceName TEXT NOT NULL,
                DeviceType INTEGER NOT NULL,
                ConnectionConfig TEXT NOT NULL,
                DataPoints TEXT NOT NULL,
                SamplingInterval INTEGER NOT NULL,
                IsEnabled INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            )";

        // 创建规则表
        var createRulesTable = @"
            CREATE TABLE IF NOT EXISTS Rules (
                Id TEXT PRIMARY KEY,
                Name TEXT NOT NULL,
                Description TEXT,
                Code TEXT NOT NULL,
                IsEnabled INTEGER NOT NULL,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL,
                Version INTEGER NOT NULL
            )";

        var command = _connection.CreateCommand();
        command.CommandText = createDataPointsTable + ";" + createDeviceConfigsTable + ";" + createRulesTable;
        await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogDebug("数据库表结构创建完成");
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        _connection?.Dispose();
    }
}

/// <summary>
/// 存储配置
/// </summary>
public class StorageConfiguration
{
    /// <summary>
    /// 数据库路径
    /// </summary>
    public string DatabasePath { get; set; } = "data/collector.db";

    /// <summary>
    /// 数据保留天数
    /// </summary>
    public int RetentionDays { get; set; } = 30;

    /// <summary>
    /// 最大数据库大小（MB）
    /// </summary>
    public int MaxDatabaseSizeMB { get; set; } = 1000;
} 
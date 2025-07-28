using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IoTDataCollection.Collector.Console.Services;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Core.Models;
using IoTDataCollection.Collector.Core.Services;
using IoTDataCollection.Collector.Storage;
using IoTDataCollection.Collector.Communication;
using IoTDataCollection.Collector.Rules;
using IoTDataCollection.Collector.Protocols;
using IoTDataCollection.Collector.Protocols.PLC;
using Xunit;

namespace IoTDataCollection.Collector.Tests.Integration;

public class CollectorIntegrationTests : IDisposable
{
    private readonly IHost _host;
    private readonly IServiceProvider _serviceProvider;

    public CollectorIntegrationTests()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // 注册测试服务
                services.AddSingleton<IConfigurationService, ConfigurationService>();
                services.AddSingleton<ICollectorService, CollectorService>();
                services.AddScoped<IProtocolFactory, ProtocolFactory>();
                services.AddScoped<PlcProtocolAdapter>();
                services.AddSingleton<ICommunicationService, MqttCommunicationService>();
                services.AddSingleton<IRuleEngine, JintRuleEngine>();
                services.AddSingleton<IStorageService, SqliteStorageService>();

                // 配置测试选项
                services.Configure<MqttConfiguration>(config =>
                {
                    config.Server = "localhost";
                    config.Port = 1883;
                    config.ClientId = "test_collector";
                });

                services.Configure<StorageConfiguration>(config =>
                {
                    config.DatabasePath = "test_data/test_collector.db";
                });

                services.Configure<CollectorConfiguration>(config =>
                {
                    config.SamplingIntervalSeconds = 1;
                    config.HeartbeatIntervalSeconds = 5;
                    config.EnableMqttCommunication = false; // 禁用MQTT进行测试
                    config.EnableLocalStorage = true;
                    config.EnableRuleEngine = true;
                });
            })
            .Build();

        _serviceProvider = _host.Services;
    }

    [Fact]
    public async Task CollectorService_Should_Initialize_Successfully()
    {
        // Arrange
        var collectorService = _serviceProvider.GetRequiredService<ICollectorService>();

        // Act
        var result = await collectorService.InitializeAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task StorageService_Should_Save_And_Retrieve_DataPoints()
    {
        // Arrange
        var storageService = _serviceProvider.GetRequiredService<IStorageService>();
        await storageService.InitializeAsync();

        var dataPoints = new List<DataPoint>
        {
            new()
            {
                DeviceCode = "TEST001",
                PointCode = "TEMP",
                PointName = "Temperature",
                NumericValue = 25.5,
                DataType = DataType.Float,
                Unit = "°C"
            }
        };

        // Act
        var saveResult = await storageService.SaveDataPointsAsync(dataPoints);
        var retrievedDataPoints = await storageService.GetUnsentDataPointsAsync();

        // Assert
        saveResult.Should().BeTrue();
        retrievedDataPoints.Should().HaveCount(1);
        retrievedDataPoints[0].DeviceCode.Should().Be("TEST001");
        retrievedDataPoints[0].PointCode.Should().Be("TEMP");
        retrievedDataPoints[0].NumericValue.Should().Be(25.5);
    }

    [Fact]
    public async Task ConfigurationService_Should_Save_And_Retrieve_DeviceConfig()
    {
        // Arrange
        var storageService = _serviceProvider.GetRequiredService<IStorageService>();
        var configService = _serviceProvider.GetRequiredService<IConfigurationService>();
        
        await storageService.InitializeAsync();
        await configService.InitializeAsync();

        var deviceConfig = new DeviceConfig
        {
            DeviceCode = "TEST001",
            DeviceName = "Test PLC",
            DeviceType = DeviceType.PLC,
            ConnectionConfig = new ConnectionConfig
            {
                IpAddress = "192.168.1.100",
                Port = 102
            },
            DataPoints = new List<DataPointConfig>
            {
                new()
                {
                    PointCode = "TEMP",
                    PointName = "Temperature",
                    Address = "DB1.DBD0",
                    DataType = DataType.Float,
                    Unit = "°C"
                }
            }
        };

        // Act
        var saveResult = await configService.SaveDeviceConfigAsync(deviceConfig);
        var retrievedConfig = await configService.GetDeviceConfigAsync("TEST001");

        // Assert
        saveResult.Should().BeTrue();
        retrievedConfig.Should().NotBeNull();
        retrievedConfig!.DeviceCode.Should().Be("TEST001");
        retrievedConfig.DeviceName.Should().Be("Test PLC");
        retrievedConfig.DataPoints.Should().HaveCount(1);
    }

    [Fact]
    public async Task RuleEngine_Should_Process_Data_With_JavaScript_Rules()
    {
        // Arrange
        var ruleEngine = _serviceProvider.GetRequiredService<IRuleEngine>();
        var dataPoints = new List<DataPoint>
        {
            new() { DeviceCode = "TEST001", PointCode = "TEMP", NumericValue = 25.0, DataType = DataType.Float },
            new() { DeviceCode = "TEST001", PointCode = "HUMIDITY", NumericValue = 60.0, DataType = DataType.Float }
        };

        var ruleCode = @"
            var temp = input.find(x => x.pointCode === 'TEMP').numericValue;
            var humidity = input.find(x => x.pointCode === 'HUMIDITY').numericValue;
            var comfortIndex = temp + humidity * 0.1;
            
            output.push({
                deviceCode: 'TEST001',
                pointCode: 'COMFORT_INDEX',
                pointName: 'Comfort Index',
                dataType: 3,
                value: comfortIndex,
                unit: '°C'
            });
        ";

        // Act
        var result = await ruleEngine.ExecuteRuleAsync(ruleCode, dataPoints);

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.OutputData.Should().HaveCount(1);
        result.OutputData[0].PointCode.Should().Be("COMFORT_INDEX");
        result.OutputData[0].NumericValue.Should().Be(31.0); // 25.0 + 60.0 * 0.1
    }

    public void Dispose()
    {
        _host?.Dispose();
    }
} 
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using IoTDataCollection.Collector.Console.Services;
using IoTDataCollection.Collector.Core.Services;
using IoTDataCollection.Collector.Core.Interfaces;
using IoTDataCollection.Collector.Storage;
using IoTDataCollection.Collector.Communication;
using IoTDataCollection.Collector.Rules;
using IoTDataCollection.Collector.Protocols;
using IoTDataCollection.Collector.Protocols.PLC;

namespace IoTDataCollection.Collector.Console;

/// <summary>
/// 采集端控制台应用程序
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            // 配置Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/collector-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("启动IoT数据采集端应用程序");

            // 创建主机
            var host = CreateHostBuilder(args).Build();

            // 运行应用程序
            await host.RunAsync();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "应用程序启动失败");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    /// <summary>
    /// 创建主机构建器
    /// </summary>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                // 配置选项
                ConfigureOptions(hostContext, services);

                // 注册核心服务
                RegisterCoreServices(services);

                // 注册协议服务
                RegisterProtocolServices(services);

                // 注册通信服务
                RegisterCommunicationServices(services);

                // 注册规则服务
                RegisterRuleServices(services);

                // 注册存储服务
                RegisterStorageServices(services);

                // 注册后台服务
                RegisterBackgroundServices(services);
            });

    /// <summary>
    /// 配置选项
    /// </summary>
    private static void ConfigureOptions(HostBuilderContext hostContext, IServiceCollection services)
    {
        var configuration = hostContext.Configuration;

        // 配置MQTT
        services.Configure<MqttConfiguration>(configuration.GetSection("Mqtt"));

        // 配置存储
        services.Configure<StorageConfiguration>(configuration.GetSection("Storage"));

        // 配置采集器
        services.Configure<CollectorConfiguration>(configuration.GetSection("Collector"));
    }

    /// <summary>
    /// 注册核心服务
    /// </summary>
    private static void RegisterCoreServices(IServiceCollection services)
    {
        services.AddSingleton<IConfigurationService, ConfigurationService>();
        services.AddSingleton<ICollectorService, CollectorService>();
    }

    /// <summary>
    /// 注册协议服务
    /// </summary>
    private static void RegisterProtocolServices(IServiceCollection services)
    {
        services.AddScoped<IProtocolFactory, ProtocolFactory>();
        services.AddScoped<PlcProtocolAdapter>();
    }

    /// <summary>
    /// 注册通信服务
    /// </summary>
    private static void RegisterCommunicationServices(IServiceCollection services)
    {
        services.AddSingleton<ICommunicationService, MqttCommunicationService>();
    }

    /// <summary>
    /// 注册规则服务
    /// </summary>
    private static void RegisterRuleServices(IServiceCollection services)
    {
        services.AddSingleton<IRuleEngine, JintRuleEngine>();
    }

    /// <summary>
    /// 注册存储服务
    /// </summary>
    private static void RegisterStorageServices(IServiceCollection services)
    {
        services.AddSingleton<IStorageService, SqliteStorageService>();
    }

    /// <summary>
    /// 注册后台服务
    /// </summary>
    private static void RegisterBackgroundServices(IServiceCollection services)
    {
        services.AddHostedService<CollectorHostedService>();
    }
}

/// <summary>
/// 采集器配置
/// </summary>
public class CollectorConfiguration
{
    /// <summary>
    /// 采集间隔（秒）
    /// </summary>
    public int SamplingIntervalSeconds { get; set; } = 5;

    /// <summary>
    /// 心跳间隔（秒）
    /// </summary>
    public int HeartbeatIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// 数据发送间隔（秒）
    /// </summary>
    public int DataSendIntervalSeconds { get; set; } = 10;

    /// <summary>
    /// 最大重试次数
    /// </summary>
    public int MaxRetryCount { get; set; } = 3;

    /// <summary>
    /// 重试间隔（秒）
    /// </summary>
    public int RetryIntervalSeconds { get; set; } = 5;

    /// <summary>
    /// 是否启用规则引擎
    /// </summary>
    public bool EnableRuleEngine { get; set; } = true;

    /// <summary>
    /// 是否启用本地存储
    /// </summary>
    public bool EnableLocalStorage { get; set; } = true;

    /// <summary>
    /// 是否启用MQTT通信
    /// </summary>
    public bool EnableMqttCommunication { get; set; } = true;
} 
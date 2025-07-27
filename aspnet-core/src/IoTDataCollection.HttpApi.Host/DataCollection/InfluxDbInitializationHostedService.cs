using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IoTDataCollection.DataCollection;

namespace IoTDataCollection.DataCollection;

/// <summary>
/// InfluxDB初始化启动服务
/// </summary>
public class InfluxDbInitializationHostedService : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<InfluxDbInitializationHostedService> _logger;

    public InfluxDbInitializationHostedService(
        IServiceProvider serviceProvider,
        ILogger<InfluxDbInitializationHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("开始InfluxDB初始化...");

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var initializationService = scope.ServiceProvider.GetRequiredService<IInfluxDbInitializationService>();

            var result = await initializationService.InitializeAsync(cancellationToken);

            if (result.Success)
            {
                _logger.LogInformation("InfluxDB初始化成功: 连接={Connection}, 组织创建={OrgCreated}, Bucket创建={BucketCreated}", 
                    result.ConnectionOk, result.OrganizationCreated, result.BucketCreated);
            }
            else
            {
                _logger.LogWarning("InfluxDB初始化失败: {ErrorMessage}", result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InfluxDB初始化过程中发生异常");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("InfluxDB初始化服务停止");
        return Task.CompletedTask;
    }
} 
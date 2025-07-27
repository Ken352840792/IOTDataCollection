using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IoTDataCollection.DataCollection;

namespace IoTDataCollection.Controllers;

/// <summary>
/// 数据采集控制器
/// </summary>
[Route("api/data-collection")]
[ApiController]
[Authorize]
public class DataCollectionController : ControllerBase
{
    private readonly IDataCollectionService _dataCollectionService;
    private readonly IInfluxDbInitializationService _influxDbInitializationService;

    public DataCollectionController(
        IDataCollectionService dataCollectionService,
        IInfluxDbInitializationService influxDbInitializationService)
    {
        _dataCollectionService = dataCollectionService;
        _influxDbInitializationService = influxDbInitializationService;
    }

    /// <summary>
    /// 获取数据库配置信息
    /// </summary>
    /// <returns>配置信息</returns>
    [HttpGet("database-config")]
    public async Task<IActionResult> GetDatabaseConfiguration()
    {
        try
        {
            var config = await _dataCollectionService.GetDatabaseConfigurationInfoAsync();
            return Ok(new { Success = true, Data = config });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"获取数据库配置异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 检查数据库配置状态
    /// </summary>
    /// <returns>配置状态</returns>
    [HttpGet("database-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var status = await _dataCollectionService.TestDatabaseConnectionsAsync();
            return Ok(new { Success = true, Data = status });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"检查数据库状态异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 初始化InfluxDB数据库
    /// </summary>
    /// <returns>初始化结果</returns>
    [HttpPost("initialize-influxdb")]
    public async Task<IActionResult> InitializeInfluxDb()
    {
        try
        {
            var result = await _influxDbInitializationService.InitializeAsync();
            return Ok(new { Success = result.Success, Data = result, Message = result.Success ? "InfluxDB初始化成功" : $"InfluxDB初始化失败: {result.ErrorMessage}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"InfluxDB初始化异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 获取InfluxDB状态
    /// </summary>
    /// <returns>状态信息</returns>
    [HttpGet("influxdb-status")]
    public async Task<IActionResult> GetInfluxDbStatus()
    {
        try
        {
            var status = await _influxDbInitializationService.GetStatusAsync();
            return Ok(new { Success = true, Data = status });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"获取InfluxDB状态异常: {ex.Message}" });
        }
    }
} 
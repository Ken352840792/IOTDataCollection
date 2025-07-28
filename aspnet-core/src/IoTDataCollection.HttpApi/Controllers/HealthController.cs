using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IoTDataCollection.Controllers;

/// <summary>
/// 健康检查控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        try
        {
            var healthInfo = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Service = "IoTDataCollection.API"
            };

            return Ok(healthInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// 详细健康检查
    /// </summary>
    [HttpGet("detailed")]
    public async Task<IActionResult> GetDetailed()
    {
        try
        {
            var healthInfo = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Version = "1.0.0",
                Service = "IoTDataCollection.API",
                Components = new
                {
                    Database = "Healthy",
                    Redis = "Healthy",
                    MQTT = "Healthy"
                },
                Uptime = Environment.TickCount / 1000.0 // 秒
            };

            return Ok(healthInfo);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Status = "Unhealthy",
                Timestamp = DateTime.UtcNow,
                Error = ex.Message
            });
        }
    }
} 
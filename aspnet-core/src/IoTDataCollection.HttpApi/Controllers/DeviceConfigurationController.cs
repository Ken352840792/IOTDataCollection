using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IoTDataCollection.Devices;
using IoTDataCollection.Application.Devices;

namespace IoTDataCollection.Controllers;

/// <summary>
/// 设备配置控制器
/// </summary>
[Route("api/device-configurations")]
[ApiController]
public class DeviceConfigurationController : ControllerBase
{
    private readonly IDeviceConfigurationAppService _deviceConfigurationAppService;

    public DeviceConfigurationController(IDeviceConfigurationAppService deviceConfigurationAppService)
    {
        _deviceConfigurationAppService = deviceConfigurationAppService;
    }

    /// <summary>
    /// 获取设备配置同步数据
    /// </summary>
    /// <param name="request">同步请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>配置同步响应</returns>
    [HttpPost("sync")]
    public async Task<IActionResult> GetDeviceConfigurationsForSyncAsync(
        [FromBody] DeviceConfigurationSyncRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(request.CollectorNodeCode))
            {
                return BadRequest(new { Success = false, Message = "采集端节点编码不能为空" });
            }

            var response = await _deviceConfigurationAppService.GetDeviceConfigurationsForSyncAsync(
                request, cancellationToken);

            if (response.Success)
            {
                return Ok(new { Success = true, Data = response });
            }
            else
            {
                return BadRequest(new { Success = false, Message = response.ErrorMessage });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"获取设备配置同步数据异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 更新设备配置
    /// </summary>
    /// <param name="request">更新请求</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>更新结果</returns>
    [HttpPost("update")]
    [Authorize]
    public async Task<IActionResult> UpdateDeviceConfigurationAsync(
        [FromBody] DeviceConfigurationUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Configuration == null)
            {
                return BadRequest(new { Success = false, Message = "设备配置不能为空" });
            }

            if (string.IsNullOrEmpty(request.Configuration.DeviceCode))
            {
                return BadRequest(new { Success = false, Message = "设备编码不能为空" });
            }

            var result = await _deviceConfigurationAppService.UpdateDeviceConfigurationAsync(
                request, cancellationToken);

            if (result)
            {
                return Ok(new { Success = true, Message = "设备配置更新成功" });
            }
            else
            {
                return BadRequest(new { Success = false, Message = "设备配置更新失败" });
            }
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"更新设备配置异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 获取设备配置列表
    /// </summary>
    /// <param name="collectorNodeCode">采集端节点编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备配置列表</returns>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDeviceConfigurationsAsync(
        [FromQuery] string? collectorNodeCode = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var configurations = await _deviceConfigurationAppService.GetDeviceConfigurationsAsync(
                collectorNodeCode, cancellationToken);

            return Ok(new { Success = true, Data = configurations });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"获取设备配置列表异常: {ex.Message}" });
        }
    }

    /// <summary>
    /// 获取指定采集端节点的设备配置列表
    /// </summary>
    /// <param name="collectorNodeCode">采集端节点编码</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>设备配置列表</returns>
    [HttpGet("collector/{collectorNodeCode}")]
    [Authorize]
    public async Task<IActionResult> GetDeviceConfigurationsByCollectorAsync(
        string collectorNodeCode,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(collectorNodeCode))
            {
                return BadRequest(new { Success = false, Message = "采集端节点编码不能为空" });
            }

            var configurations = await _deviceConfigurationAppService.GetDeviceConfigurationsAsync(
                collectorNodeCode, cancellationToken);

            return Ok(new { Success = true, Data = configurations });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Success = false, Message = $"获取采集端设备配置列表异常: {ex.Message}" });
        }
    }
} 
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using IoTDataCollection.Enterprises;

namespace IoTDataCollection.Controllers;

/// <summary>
/// 企业管理API控制器
/// </summary>
[Area(IoTDataCollectionRemoteServiceConsts.ModuleName)]
[RemoteService(Name = IoTDataCollectionRemoteServiceConsts.RemoteServiceName)]
[Route("api/enterprises")]
public class EnterprisesController : AbpControllerBase, IEnterpriseAppService
{
    private readonly IEnterpriseAppService _enterpriseAppService;

    public EnterprisesController(IEnterpriseAppService enterpriseAppService)
    {
        _enterpriseAppService = enterpriseAppService;
    }

    /// <summary>
    /// 获取企业列表
    /// </summary>
    /// <param name="input">查询参数</param>
    /// <returns>分页企业列表</returns>
    [HttpGet]
    public virtual Task<PagedResultDto<EnterpriseDto>> GetListAsync(GetEnterpriseListDto input)
    {
        return _enterpriseAppService.GetListAsync(input);
    }

    /// <summary>
    /// 根据ID获取企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns>企业信息</returns>
    [HttpGet("{id}")]
    public virtual Task<EnterpriseDto> GetAsync(Guid id)
    {
        return _enterpriseAppService.GetAsync(id);
    }

    /// <summary>
    /// 创建企业
    /// </summary>
    /// <param name="input">创建参数</param>
    /// <returns>创建的企业信息</returns>
    [HttpPost]
    public virtual Task<EnterpriseDto> CreateAsync(CreateEnterpriseDto input)
    {
        return _enterpriseAppService.CreateAsync(input);
    }

    /// <summary>
    /// 更新企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <param name="input">更新参数</param>
    /// <returns>更新后的企业信息</returns>
    [HttpPut("{id}")]
    public virtual Task<EnterpriseDto> UpdateAsync(Guid id, UpdateEnterpriseDto input)
    {
        return _enterpriseAppService.UpdateAsync(id, input);
    }

    /// <summary>
    /// 删除企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return _enterpriseAppService.DeleteAsync(id);
    }

    /// <summary>
    /// 根据企业编码获取企业
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <returns>企业信息</returns>
    [HttpGet("by-code/{enterpriseCode}")]
    public virtual Task<EnterpriseDto?> FindByCodeAsync(string enterpriseCode)
    {
        return _enterpriseAppService.FindByCodeAsync(enterpriseCode);
    }

    /// <summary>
    /// 根据企业名称获取企业
    /// </summary>
    /// <param name="enterpriseName">企业名称</param>
    /// <returns>企业信息</returns>
    [HttpGet("by-name/{enterpriseName}")]
    public virtual Task<EnterpriseDto?> FindByNameAsync(string enterpriseName)
    {
        return _enterpriseAppService.FindByNameAsync(enterpriseName);
    }

    /// <summary>
    /// 检查企业编码是否已存在
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <param name="excludeId">排除的企业ID</param>
    /// <returns>是否存在</returns>
    [HttpGet("check-code")]
    public virtual Task<bool> IsCodeExistAsync(string enterpriseCode, Guid? excludeId = null)
    {
        return _enterpriseAppService.IsCodeExistAsync(enterpriseCode, excludeId);
    }

    /// <summary>
    /// 启用企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns></returns>
    [HttpPost("{id}/enable")]
    public virtual Task EnableAsync(Guid id)
    {
        return _enterpriseAppService.EnableAsync(id);
    }

    /// <summary>
    /// 禁用企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns></returns>
    [HttpPost("{id}/disable")]
    public virtual Task DisableAsync(Guid id)
    {
        return _enterpriseAppService.DisableAsync(id);
    }

    /// <summary>
    /// 获取企业统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    [HttpGet("stats")]
    public virtual Task<EnterpriseStatsDto> GetStatsAsync()
    {
        return _enterpriseAppService.GetStatsAsync();
    }
} 
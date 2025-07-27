using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业应用服务接口
/// </summary>
public interface IEnterpriseAppService : ICrudAppService<
    EnterpriseDto,
    Guid,
    GetEnterpriseListDto,
    CreateEnterpriseDto,
    UpdateEnterpriseDto>
{
    /// <summary>
    /// 根据企业编码获取企业
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <returns>企业信息</returns>
    Task<EnterpriseDto?> FindByCodeAsync(string enterpriseCode);

    /// <summary>
    /// 根据企业名称获取企业
    /// </summary>
    /// <param name="enterpriseName">企业名称</param>
    /// <returns>企业信息</returns>
    Task<EnterpriseDto?> FindByNameAsync(string enterpriseName);

    /// <summary>
    /// 检查企业编码是否已存在
    /// </summary>
    /// <param name="enterpriseCode">企业编码</param>
    /// <param name="excludeId">排除的企业ID</param>
    /// <returns>是否存在</returns>
    Task<bool> IsCodeExistAsync(string enterpriseCode, Guid? excludeId = null);

    /// <summary>
    /// 启用企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns></returns>
    Task EnableAsync(Guid id);

    /// <summary>
    /// 禁用企业
    /// </summary>
    /// <param name="id">企业ID</param>
    /// <returns></returns>
    Task DisableAsync(Guid id);

    /// <summary>
    /// 获取企业统计信息
    /// </summary>
    /// <returns>统计信息</returns>
    Task<EnterpriseStatsDto> GetStatsAsync();
} 
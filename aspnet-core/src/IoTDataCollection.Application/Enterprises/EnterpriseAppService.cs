using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using IoTDataCollection.Permissions;

namespace IoTDataCollection.Enterprises;

/// <summary>
/// 企业应用服务实现
/// </summary>
[Authorize(IoTDataCollectionPermissions.Enterprises.Default)]
public class EnterpriseAppService : CrudAppService<
        Enterprise,
        EnterpriseDto,
        Guid,
        GetEnterpriseListDto,
        CreateEnterpriseDto,
        UpdateEnterpriseDto>,
    IEnterpriseAppService
{
    private readonly IEnterpriseRepository _enterpriseRepository;

    public EnterpriseAppService(
        IEnterpriseRepository repository)
        : base(repository)
    {
        _enterpriseRepository = repository;
        
        GetPolicyName = IoTDataCollectionPermissions.Enterprises.Default;
        GetListPolicyName = IoTDataCollectionPermissions.Enterprises.Default;
        CreatePolicyName = IoTDataCollectionPermissions.Enterprises.Create;
        UpdatePolicyName = IoTDataCollectionPermissions.Enterprises.Edit;
        DeletePolicyName = IoTDataCollectionPermissions.Enterprises.Delete;
    }

    protected override async Task<IQueryable<Enterprise>> CreateFilteredQueryAsync(GetEnterpriseListDto input)
    {
        var query = await ReadOnlyRepository.GetQueryableAsync();

        return query
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), 
                x => x.F_EnterpriseName.Contains(input.Filter!) ||
                     x.F_EnterpriseCode.Contains(input.Filter!) ||
                     x.F_ShortName!.Contains(input.Filter!))
            .WhereIf(!input.EnterpriseType.IsNullOrWhiteSpace(),
                x => x.F_EnterpriseType == input.EnterpriseType)
            .WhereIf(input.IsEnabled.HasValue,
                x => x.F_IsEnabled == input.IsEnabled.Value);
    }

    public override async Task<EnterpriseDto> CreateAsync(CreateEnterpriseDto input)
    {
        await CheckPolicyAsync(IoTDataCollectionPermissions.Enterprises.Create);

        // 检查企业编码是否已存在
        if (await _enterpriseRepository.IsCodeExistAsync(input.EnterpriseCode))
        {
            throw new UserFriendlyException($"企业编码 '{input.EnterpriseCode}' 已存在");
        }

        var enterprise = new Enterprise(
            GuidGenerator.Create(),
            input.EnterpriseCode,
            input.EnterpriseName,
            input.ShortName,
            input.EnterpriseType
        );

        // 映射其他属性
        enterprise.F_ContactPerson = input.ContactPerson;
        enterprise.F_ContactPhone = input.ContactPhone;
        enterprise.F_Address = input.Address;
        enterprise.F_Description = input.Description;
        enterprise.F_SortOrder = input.SortOrder;
        enterprise.F_IsEnabled = input.IsEnabled;

        var insertedEnterprise = await _enterpriseRepository.InsertAsync(enterprise, autoSave: true);

        return ObjectMapper.Map<Enterprise, EnterpriseDto>(insertedEnterprise);
    }

    public override async Task<EnterpriseDto> UpdateAsync(Guid id, UpdateEnterpriseDto input)
    {
        await CheckPolicyAsync(IoTDataCollectionPermissions.Enterprises.Edit);

        var enterprise = await _enterpriseRepository.GetAsync(id);

        // 检查企业编码是否已存在（排除当前记录）
        if (await _enterpriseRepository.IsCodeExistAsync(input.EnterpriseCode, id))
        {
            throw new UserFriendlyException($"企业编码 '{input.EnterpriseCode}' 已存在");
        }

        // 更新属性
        enterprise.F_EnterpriseCode = input.EnterpriseCode;
        enterprise.F_EnterpriseName = input.EnterpriseName;
        enterprise.F_ShortName = input.ShortName;
        enterprise.F_EnterpriseType = input.EnterpriseType;
        enterprise.F_ContactPerson = input.ContactPerson;
        enterprise.F_ContactPhone = input.ContactPhone;
        enterprise.F_Address = input.Address;
        enterprise.F_Description = input.Description;
        enterprise.F_SortOrder = input.SortOrder;
        enterprise.F_IsEnabled = input.IsEnabled;

        var updatedEnterprise = await _enterpriseRepository.UpdateAsync(enterprise, autoSave: true);

        return ObjectMapper.Map<Enterprise, EnterpriseDto>(updatedEnterprise);
    }

    public async Task<EnterpriseDto?> FindByCodeAsync(string enterpriseCode)
    {
        var enterprise = await _enterpriseRepository.FindByCodeAsync(enterpriseCode);
        return enterprise != null 
            ? ObjectMapper.Map<Enterprise, EnterpriseDto>(enterprise) 
            : null;
    }

    public async Task<EnterpriseDto?> FindByNameAsync(string enterpriseName)
    {
        var enterprise = await _enterpriseRepository.FindByNameAsync(enterpriseName);
        return enterprise != null 
            ? ObjectMapper.Map<Enterprise, EnterpriseDto>(enterprise) 
            : null;
    }

    public async Task<bool> IsCodeExistAsync(string enterpriseCode, Guid? excludeId = null)
    {
        return await _enterpriseRepository.IsCodeExistAsync(enterpriseCode, excludeId);
    }

    [Authorize(IoTDataCollectionPermissions.Enterprises.Edit)]
    public async Task EnableAsync(Guid id)
    {
        var enterprise = await _enterpriseRepository.GetAsync(id);
        enterprise.F_IsEnabled = true;
        await _enterpriseRepository.UpdateAsync(enterprise, autoSave: true);
    }

    [Authorize(IoTDataCollectionPermissions.Enterprises.Edit)]
    public async Task DisableAsync(Guid id)
    {
        var enterprise = await _enterpriseRepository.GetAsync(id);
        enterprise.F_IsEnabled = false;
        await _enterpriseRepository.UpdateAsync(enterprise, autoSave: true);
    }

    public async Task<EnterpriseStatsDto> GetStatsAsync()
    {
        var enterprises = await _enterpriseRepository.GetListAsync();
        var recentDate = DateTime.Now.AddDays(-30);

        var stats = new EnterpriseStatsDto
        {
            TotalCount = enterprises.Count,
            EnabledCount = enterprises.Count(x => x.F_IsEnabled),
            DisabledCount = enterprises.Count(x => !x.F_IsEnabled),
            RecentlyCreatedCount = enterprises.Count(x => x.CreationTime >= recentDate)
        };

        // 按企业类型分组统计
        stats.CountByType = enterprises
            .GroupBy(x => x.F_EnterpriseType)
            .ToDictionary(g => g.Key, g => g.Count());

        return stats;
    }
} 
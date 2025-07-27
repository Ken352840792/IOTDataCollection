using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using IoTDataCollection.Enterprises;
using IoTDataCollection.Sites;
using System.Linq;

namespace IoTDataCollection.Data;

/// <summary>
/// IoT数据种子贡献者
/// </summary>
public class IoTDataCollectionDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Enterprise, Guid> _enterpriseRepository;
    private readonly IRepository<Site, Guid> _siteRepository;

    public IoTDataCollectionDataSeedContributor(
        IRepository<Enterprise, Guid> enterpriseRepository,
        IRepository<Site, Guid> siteRepository)
    {
        _enterpriseRepository = enterpriseRepository;
        _siteRepository = siteRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedEnterprisesAsync();
        await SeedSitesAsync();
    }

    private async Task SeedEnterprisesAsync()
    {
        // 检查是否已有企业数据
        var existingCount = await _enterpriseRepository.GetCountAsync();
        if (existingCount > 0)
        {
            return;
        }

        // 创建示例企业
        var demoEnterprise = new Enterprise(
            id: Guid.NewGuid(),
            enterpriseCode: "DEMO_ENT_001",
            enterpriseName: "示例制造企业",
            shortName: "示例企业",
            enterpriseType: "制造业"
        );
        demoEnterprise.F_ContactPerson = "张经理";
        demoEnterprise.F_ContactPhone = "13800138000";
        demoEnterprise.F_Address = "北京市海淀区中关村科技园";
        demoEnterprise.F_Description = "这是一个用于演示IoT数据采集平台功能的示例企业";

        await _enterpriseRepository.InsertAsync(demoEnterprise, autoSave: true);

        // 创建第二个示例企业
        var chemicalEnterprise = new Enterprise(
            id: Guid.NewGuid(),
            enterpriseCode: "CHEM_ENT_001",
            enterpriseName: "化工集团有限公司",
            shortName: "化工集团",
            enterpriseType: "化工"
        );
        chemicalEnterprise.F_ContactPerson = "李总";
        chemicalEnterprise.F_ContactPhone = "13900139000";
        chemicalEnterprise.F_Address = "上海市浦东新区张江高科技园";
        chemicalEnterprise.F_Description = "专业从事化工产品生产的大型企业集团";

        await _enterpriseRepository.InsertAsync(chemicalEnterprise, autoSave: true);
    }

    private async Task SeedSitesAsync()
    {
        // 检查是否已有站点数据
        var existingCount = await _siteRepository.GetCountAsync();
        if (existingCount > 0)
        {
            return;
        }

        // 获取示例企业
        var enterprises = await _enterpriseRepository.GetListAsync();
        if (enterprises.Count == 0)
        {
            return;
        }

        var demoEnterprise = enterprises.FirstOrDefault(e => e.F_EnterpriseCode == "DEMO_ENT_001");
        if (demoEnterprise != null)
        {
            // 为示例企业创建站点
            var mainFactory = new Site(
                id: Guid.NewGuid(),
                enterpriseId: demoEnterprise.Id,
                siteCode: "MAIN_FACTORY",
                siteName: "主生产车间",
                shortName: "主车间",
                siteType: "生产车间"
            );
            mainFactory.F_Address = "北京市海淀区中关村科技园1号楼";
            mainFactory.F_Manager = "王车间主任";
            mainFactory.F_ContactPhone = "13700137000";
            mainFactory.F_Description = "主要生产车间，包含多条自动化生产线";

            await _siteRepository.InsertAsync(mainFactory, autoSave: true);

            var warehouse = new Site(
                id: Guid.NewGuid(),
                enterpriseId: demoEnterprise.Id,
                siteCode: "WAREHOUSE",
                siteName: "中央仓库",
                shortName: "仓库",
                siteType: "仓储"
            );
            warehouse.F_Address = "北京市海淀区中关村科技园2号楼";
            warehouse.F_Manager = "赵仓库主管";
            warehouse.F_ContactPhone = "13600136000";
            warehouse.F_Description = "中央仓库，负责原材料和成品的存储管理";

            await _siteRepository.InsertAsync(warehouse, autoSave: true);
        }

        var chemicalEnterprise = enterprises.FirstOrDefault(e => e.F_EnterpriseCode == "CHEM_ENT_001");
        if (chemicalEnterprise != null)
        {
            // 为化工企业创建站点
            var chemicalPlant = new Site(
                id: Guid.NewGuid(),
                enterpriseId: chemicalEnterprise.Id,
                siteCode: "CHEM_PLANT_01",
                siteName: "化工生产装置1号",
                shortName: "1号装置",
                siteType: "化工装置"
            );
            chemicalPlant.F_Address = "上海市浦东新区张江高科技园A区";
            chemicalPlant.F_Manager = "陈工程师";
            chemicalPlant.F_ContactPhone = "13500135000";
            chemicalPlant.F_Description = "主要化工生产装置，包含反应器、分离器等关键设备";

            await _siteRepository.InsertAsync(chemicalPlant, autoSave: true);
        }
    }
} 
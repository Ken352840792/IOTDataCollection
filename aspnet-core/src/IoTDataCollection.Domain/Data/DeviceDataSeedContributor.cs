using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using IoTDataCollection.Devices;
using IoTDataCollection.Monitoring;
using IoTDataCollection.Sites;
using IoTDataCollection.DataPoints;
using System.Linq;

namespace IoTDataCollection.Data;

/// <summary>
/// 设备数据种子贡献者
/// </summary>
public class DeviceDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<Device, Guid> _deviceRepository;
    private readonly IRepository<CollectorNode, Guid> _collectorNodeRepository;
    private readonly IRepository<Site, Guid> _siteRepository;
    private readonly IRepository<DataPoint, Guid> _dataPointRepository;

    public DeviceDataSeedContributor(
        IRepository<Device, Guid> deviceRepository,
        IRepository<CollectorNode, Guid> collectorNodeRepository,
        IRepository<Site, Guid> siteRepository,
        IRepository<DataPoint, Guid> dataPointRepository)
    {
        _deviceRepository = deviceRepository;
        _collectorNodeRepository = collectorNodeRepository;
        _siteRepository = siteRepository;
        _dataPointRepository = dataPointRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await SeedCollectorNodesAsync();
        await SeedDevicesAsync();
        await SeedDataPointsAsync();
    }

    private async Task SeedCollectorNodesAsync()
    {
        // 检查是否已有采集节点数据
        var existingCount = await _collectorNodeRepository.GetCountAsync();
        if (existingCount > 0)
        {
            return;
        }

        // 创建示例采集节点
        var collectorNode1 = new CollectorNode(
            id: Guid.NewGuid(),
            nodeCode: "COLLECTOR_001",
            nodeName: "主车间采集节点",
            nodeType: "PLC采集器",
            ipAddress: "192.168.1.100"
        );
        collectorNode1.F_Description = "负责主车间PLC设备的数据采集";
        collectorNode1.F_IsMonitoringEnabled = true;
        collectorNode1.F_ConnectionStatus = 1; // 在线
        collectorNode1.F_NodeStatus = 1; // 正常

        await _collectorNodeRepository.InsertAsync(collectorNode1, autoSave: true);

        // 创建第二个采集节点
        var collectorNode2 = new CollectorNode(
            id: Guid.NewGuid(),
            nodeCode: "COLLECTOR_002",
            nodeName: "包装车间采集节点",
            nodeType: "PLC采集器",
            ipAddress: "192.168.1.101"
        );
        collectorNode2.F_Description = "负责包装车间PLC设备的数据采集";
        collectorNode2.F_IsMonitoringEnabled = true;
        collectorNode2.F_ConnectionStatus = 1; // 在线
        collectorNode2.F_NodeStatus = 1; // 正常

        await _collectorNodeRepository.InsertAsync(collectorNode2, autoSave: true);
    }

    private async Task SeedDevicesAsync()
    {
        // 检查是否已有设备数据
        var existingCount = await _deviceRepository.GetCountAsync();
        if (existingCount > 0)
        {
            return;
        }

        // 获取站点
        var sites = await _siteRepository.GetListAsync();
        if (sites.Count == 0)
        {
            return;
        }

        var mainSite = sites.FirstOrDefault(s => s.F_SiteCode == "MAIN_FACTORY");
        if (mainSite == null)
        {
            return;
        }

        // 获取采集节点
        var collectorNodes = await _collectorNodeRepository.GetListAsync();
        if (collectorNodes.Count == 0)
        {
            return;
        }

        var collectorNode1 = collectorNodes.FirstOrDefault(n => n.F_NodeCode == "COLLECTOR_001");
        if (collectorNode1 == null)
        {
            return;
        }

        // 创建示例设备 - PLC设备
        var plcDevice = new Device(
            id: Guid.NewGuid(),
            siteId: mainSite.Id,
            deviceCode: "PLC_001",
            deviceName: "西门子S7-1200 PLC",
            deviceType: "PLC",
            communicationProtocol: "S7"
        );
        plcDevice.F_DeviceModel = "S7-1200";
        plcDevice.F_Manufacturer = "西门子";
        plcDevice.F_IpAddress = "192.168.1.10";
        plcDevice.F_Port = 102;
        plcDevice.F_CollectionInterval = 1000; // 1秒
        plcDevice.F_Timeout = 5000; // 5秒
        plcDevice.F_RetryCount = 3;
        plcDevice.F_Location = "主车间控制柜";
        plcDevice.F_Description = "主车间生产线控制PLC";
        plcDevice.F_IsCollectionEnabled = true;
        plcDevice.F_Status = 1; // 启用
        plcDevice.F_CollectorNodeCode = collectorNode1.F_NodeCode;

        await _deviceRepository.InsertAsync(plcDevice, autoSave: true);

        // 创建第二个设备 - 温度传感器
        var tempSensor = new Device(
            id: Guid.NewGuid(),
            siteId: mainSite.Id,
            deviceCode: "TEMP_001",
            deviceName: "温度传感器",
            deviceType: "传感器",
            communicationProtocol: "Modbus"
        );
        tempSensor.F_DeviceModel = "PT100";
        tempSensor.F_Manufacturer = "欧姆龙";
        tempSensor.F_IpAddress = "192.168.1.11";
        tempSensor.F_Port = 502;
        tempSensor.F_SlaveAddress = 1;
        tempSensor.F_CollectionInterval = 2000; // 2秒
        tempSensor.F_Timeout = 3000; // 3秒
        tempSensor.F_RetryCount = 3;
        tempSensor.F_Location = "主车间温度监测点";
        tempSensor.F_Description = "主车间环境温度监测传感器";
        tempSensor.F_IsCollectionEnabled = true;
        tempSensor.F_Status = 1; // 启用
        tempSensor.F_CollectorNodeCode = collectorNode1.F_NodeCode;

        await _deviceRepository.InsertAsync(tempSensor, autoSave: true);

        // 创建第三个设备 - 压力传感器
        var pressureSensor = new Device(
            id: Guid.NewGuid(),
            siteId: mainSite.Id,
            deviceCode: "PRESS_001",
            deviceName: "压力传感器",
            deviceType: "传感器",
            communicationProtocol: "Modbus"
        );
        pressureSensor.F_DeviceModel = "4-20mA";
        pressureSensor.F_Manufacturer = "霍尼韦尔";
        pressureSensor.F_IpAddress = "192.168.1.12";
        pressureSensor.F_Port = 502;
        pressureSensor.F_SlaveAddress = 2;
        pressureSensor.F_CollectionInterval = 3000; // 3秒
        pressureSensor.F_Timeout = 3000; // 3秒
        pressureSensor.F_RetryCount = 3;
        pressureSensor.F_Location = "主车间压力监测点";
        pressureSensor.F_Description = "主车间管道压力监测传感器";
        pressureSensor.F_IsCollectionEnabled = true;
        pressureSensor.F_Status = 1; // 启用
        pressureSensor.F_CollectorNodeCode = collectorNode1.F_NodeCode;

        await _deviceRepository.InsertAsync(pressureSensor, autoSave: true);
    }

    private async Task SeedDataPointsAsync()
    {
        // 检查是否已有数据点数据
        var existingCount = await _dataPointRepository.GetCountAsync();
        if (existingCount > 0)
        {
            return;
        }

        // 获取设备
        var devices = await _deviceRepository.GetListAsync();
        if (devices.Count == 0)
        {
            return;
        }

        var plcDevice = devices.FirstOrDefault(d => d.F_DeviceCode == "PLC_001");
        var tempSensor = devices.FirstOrDefault(d => d.F_DeviceCode == "TEMP_001");
        var pressureSensor = devices.FirstOrDefault(d => d.F_DeviceCode == "PRESS_001");

        // 创建PLC设备的数据点
        if (plcDevice != null)
        {
            // PLC状态数据点
            var plcStatusDataPoint = new DataPoint(
                id: Guid.NewGuid(),
                deviceId: plcDevice.Id,
                pointCode: "PLC_STATUS",
                pointName: "PLC运行状态",
                dataType: "Int32",
                registerAddress: "DB1.DBX0.0", // Siemens S7地址格式
                registerType: "DB"
            );
            plcStatusDataPoint.F_Description = "PLC设备运行状态";
            plcStatusDataPoint.F_Unit = "状态";
            plcStatusDataPoint.F_IsCollectionEnabled = true;
            plcStatusDataPoint.F_Status = 1;
            plcStatusDataPoint.F_SortOrder = 1;

            await _dataPointRepository.InsertAsync(plcStatusDataPoint, autoSave: true);

            // PLC温度数据点
            var plcTempDataPoint = new DataPoint(
                id: Guid.NewGuid(),
                deviceId: plcDevice.Id,
                pointCode: "PLC_TEMP",
                pointName: "PLC温度",
                dataType: "Float",
                registerAddress: "DB1.DBD2", // Siemens S7地址格式
                registerType: "DB"
            );
            plcTempDataPoint.F_Description = "PLC内部温度";
            plcTempDataPoint.F_Unit = "°C";
            plcTempDataPoint.F_ScaleFactor = 1.0;
            plcTempDataPoint.F_Offset = 0.0;
            plcTempDataPoint.F_IsCollectionEnabled = true;
            plcTempDataPoint.F_Status = 1;
            plcTempDataPoint.F_SortOrder = 2;

            await _dataPointRepository.InsertAsync(plcTempDataPoint, autoSave: true);
        }

        // 创建温度传感器的数据点
        if (tempSensor != null)
        {
            var tempDataPoint = new DataPoint(
                id: Guid.NewGuid(),
                deviceId: tempSensor.Id,
                pointCode: "TEMP_VALUE",
                pointName: "温度值",
                dataType: "Float",
                registerAddress: "40001", // Modbus地址格式
                registerType: "HOLDING"
            );
            tempDataPoint.F_Description = "环境温度监测值";
            tempDataPoint.F_Unit = "°C";
            tempDataPoint.F_ScaleFactor = 0.1; // 缩放因子
            tempDataPoint.F_Offset = 0.0;
            tempDataPoint.F_IsCollectionEnabled = true;
            tempDataPoint.F_Status = 1;
            tempDataPoint.F_SortOrder = 1;

            await _dataPointRepository.InsertAsync(tempDataPoint, autoSave: true);
        }

        // 创建压力传感器的数据点
        if (pressureSensor != null)
        {
            var pressureDataPoint = new DataPoint(
                id: Guid.NewGuid(),
                deviceId: pressureSensor.Id,
                pointCode: "PRESS_VALUE",
                pointName: "压力值",
                dataType: "Float",
                registerAddress: "40002", // Modbus地址格式
                registerType: "HOLDING"
            );
            pressureDataPoint.F_Description = "管道压力监测值";
            pressureDataPoint.F_Unit = "kPa";
            pressureDataPoint.F_ScaleFactor = 1.0;
            pressureDataPoint.F_Offset = 0.0;
            pressureDataPoint.F_IsCollectionEnabled = true;
            pressureDataPoint.F_Status = 1;
            pressureDataPoint.F_SortOrder = 1;

            await _dataPointRepository.InsertAsync(pressureDataPoint, autoSave: true);
        }
    }
} 
-- IoT工业数据采集平台 - 模拟数据插入脚本
-- 生成时间: 2025-01-15
-- 说明: 此脚本将插入完整的测试数据，包括企业、站点、采集节点、设备、数据点等

USE IoTDataCollection_Main;

-- 清空现有数据（可选，谨慎使用）
-- DELETE FROM T_DATA_POINTS;
-- DELETE FROM T_DEVICES;
-- DELETE FROM T_COLLECTOR_NODES;
-- DELETE FROM T_SITES;
-- DELETE FROM T_ENTERPRISES;

-- ========================================
-- 1. 插入企业数据
-- ========================================
INSERT INTO T_ENTERPRISES (
    Id, F_ENTERPRISE_CODE, F_ENTERPRISE_NAME, F_SHORT_NAME, F_ENTERPRISE_TYPE, 
    F_CONTACT_PERSON, F_CONTACT_PHONE, F_ADDRESS, F_DESCRIPTION, F_STATUS, F_IS_ENABLED, F_SORT_ORDER,
    ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime
) VALUES
-- 示例制造企业
(UUID(), 'DEMO_ENT_001', '示例制造企业', '示例企业', '制造业', 
 '张经理', '13800138000', '北京市海淀区中关村科技园', '这是一个用于演示IoT数据采集平台功能的示例企业', 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 化工企业
(UUID(), 'CHEM_ENT_001', '化工集团有限公司', '化工集团', '化工', 
 '李总', '13900139000', '上海市浦东新区张江高科技园', '专业从事化工产品生产的大型企业集团', 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 电力企业
(UUID(), 'POWER_ENT_001', '新能源电力公司', '新能源电力', '电力', 
 '王工程师', '13700137000', '深圳市南山区科技园', '专注于新能源发电和智能电网建设', 1, 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL);

-- ========================================
-- 2. 插入站点数据
-- ========================================
INSERT INTO T_SITES (
    Id, F_ENTERPRISE_ID, F_SITE_CODE, F_SITE_NAME, F_SHORT_NAME, F_SITE_TYPE, 
    F_ADDRESS, F_MANAGER, F_CONTACT_PHONE, F_DESCRIPTION, F_STATUS, F_SORT_ORDER,
    ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime
) VALUES
-- 示例企业的站点
(UUID(), (SELECT Id FROM T_ENTERPRISES WHERE F_ENTERPRISE_CODE = 'DEMO_ENT_001' LIMIT 1), 
 'MAIN_FACTORY', '主生产车间', '主车间', '生产车间', 
 '北京市海淀区中关村科技园A座', '刘主任', '13800138001', '主要生产车间，包含多条自动化生产线', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_ENTERPRISES WHERE F_ENTERPRISE_CODE = 'DEMO_ENT_001' LIMIT 1), 
 'PACKAGING_FACTORY', '包装车间', '包装车间', '包装车间', 
 '北京市海淀区中关村科技园B座', '陈主任', '13800138002', '产品包装和质检车间', 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 化工企业的站点
(UUID(), (SELECT Id FROM T_ENTERPRISES WHERE F_ENTERPRISE_CODE = 'CHEM_ENT_001' LIMIT 1), 
 'CHEM_PLANT_01', '化工生产车间1', '化工厂1', '生产车间', 
 '上海市浦东新区张江高科技园C区', '赵主任', '13900139001', '主要化工产品生产线', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 电力企业的站点
(UUID(), (SELECT Id FROM T_ENTERPRISES WHERE F_ENTERPRISE_CODE = 'POWER_ENT_001' LIMIT 1), 
 'SOLAR_PLANT_01', '太阳能发电站', '太阳能电站', '发电站', 
 '深圳市南山区科技园D区', '孙工程师', '13700137001', '太阳能光伏发电站', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL);

-- ========================================
-- 3. 插入采集节点数据
-- ========================================
INSERT INTO T_COLLECTOR_NODES (
    Id, F_NODE_CODE, F_NODE_NAME, F_DESCRIPTION, F_NODE_TYPE, F_IP_ADDRESS, 
    F_MANAGEMENT_PORT, F_DATA_PORT, F_OPERATING_SYSTEM, F_SOFTWARE_VERSION, F_HARDWARE_INFO,
    F_NODE_STATUS, F_CONNECTION_STATUS, F_LAST_HEARTBEAT_TIME, F_HEARTBEAT_INTERVAL, F_TIMEOUT_THRESHOLD,
    F_CPU_USAGE, F_MEMORY_USAGE, F_DISK_USAGE, F_NETWORK_TRAFFIC, F_DEVICE_COUNT, F_ONLINE_DEVICE_COUNT,
    F_DATA_COLLECTION_RATE, F_ERROR_DATA_COUNT, F_LOCATION, F_RESPONSIBLE_PERSON, F_CONTACT_PHONE,
    F_IS_MONITORING_ENABLED, F_IS_ALERT_ENABLED, F_SORT_ORDER,
    ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime
) VALUES
-- 示例企业的采集节点
(UUID(), 'COLLECTOR_001', '主车间采集节点', '负责主车间PLC设备的数据采集', 'Edge', '192.168.1.100',
 8080, 8081, 'Windows 10', 'v2.1.0', 'Intel i5, 8GB RAM, 256GB SSD',
 1, 1, NOW(), 30, 180,
 25.5, 45.2, 60.8, 1024.5, 3, 3,
 150.0, 0, '主车间控制室', '张技术员', '13800138003',
 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), 'COLLECTOR_002', '包装车间采集节点', '负责包装车间PLC设备的数据采集', 'Edge', '192.168.1.101',
 8080, 8081, 'Windows 10', 'v2.1.0', 'Intel i3, 4GB RAM, 128GB SSD',
 1, 1, NOW(), 30, 180,
 20.3, 35.8, 45.2, 512.8, 2, 2,
 80.0, 0, '包装车间控制室', '李技术员', '13800138004',
 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 化工企业的采集节点
(UUID(), 'COLLECTOR_003', '化工车间采集节点', '负责化工车间设备的数据采集', 'Edge', '192.168.2.100',
 8080, 8081, 'Linux Ubuntu 20.04', 'v2.1.0', 'ARM Cortex-A72, 4GB RAM, 64GB eMMC',
 1, 1, NOW(), 30, 180,
 30.2, 55.6, 70.1, 2048.3, 4, 4,
 200.0, 1, '化工车间控制室', '王技术员', '13900139002',
 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 电力企业的采集节点
(UUID(), 'COLLECTOR_004', '太阳能电站采集节点', '负责太阳能电站设备的数据采集', 'Edge', '192.168.3.100',
 8080, 8081, 'Linux CentOS 7', 'v2.1.0', 'Intel Atom, 2GB RAM, 32GB SSD',
 1, 1, NOW(), 30, 180,
 15.8, 28.4, 40.5, 768.9, 5, 5,
 120.0, 0, '太阳能电站控制室', '孙技术员', '13700137002',
 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL);

-- ========================================
-- 4. 插入设备数据
-- ========================================
INSERT INTO T_DEVICES (
    Id, F_SITE_ID, F_DEVICE_CODE, F_DEVICE_NAME, F_DEVICE_TYPE, F_DEVICE_MODEL, F_MANUFACTURER,
    F_COMMUNICATION_PROTOCOL, F_IP_ADDRESS, F_PORT, F_SLAVE_ADDRESS, F_COLLECTION_INTERVAL, F_TIMEOUT, F_RETRY_COUNT,
    F_LOCATION, F_DESCRIPTION, F_CONNECTION_STATUS, F_LAST_COMMUNICATION_TIME, F_IS_COLLECTION_ENABLED, F_COLLECTOR_NODE_CODE, F_STATUS, F_SORT_ORDER,
    ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime
) VALUES
-- 示例企业主车间的设备
(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'MAIN_FACTORY' LIMIT 1), 
 'PLC_001', '西门子S7-1200 PLC', 'PLC', 'S7-1200', '西门子',
 'S7', '192.168.1.10', 102, NULL, 1000, 5000, 3,
 '主车间控制柜', '主车间生产线控制PLC', 1, NOW(), 1, 'COLLECTOR_001', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'MAIN_FACTORY' LIMIT 1), 
 'TEMP_001', '温度传感器', '传感器', 'PT100', '欧姆龙',
 'Modbus', '192.168.1.11', 502, 1, 2000, 3000, 3,
 '主车间温度监测点', '主车间环境温度监测传感器', 1, NOW(), 1, 'COLLECTOR_001', 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'MAIN_FACTORY' LIMIT 1), 
 'PRESS_001', '压力传感器', '传感器', '4-20mA', '霍尼韦尔',
 'Modbus', '192.168.1.12', 502, 2, 3000, 3000, 3,
 '主车间压力监测点', '主车间管道压力监测传感器', 1, NOW(), 1, 'COLLECTOR_001', 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 示例企业包装车间的设备
(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'PACKAGING_FACTORY' LIMIT 1), 
 'PLC_002', '西门子S7-200 PLC', 'PLC', 'S7-200', '西门子',
 'S7', '192.168.1.20', 102, NULL, 1000, 5000, 3,
 '包装车间控制柜', '包装车间控制PLC', 1, NOW(), 1, 'COLLECTOR_002', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'PACKAGING_FACTORY' LIMIT 1), 
 'FLOW_001', '流量计', '仪表', '电磁流量计', '科隆',
 'Modbus', '192.168.1.21', 502, 3, 2000, 3000, 3,
 '包装车间管道', '包装车间流量监测', 1, NOW(), 1, 'COLLECTOR_002', 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 化工企业的设备
(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'CHEM_PLANT_01' LIMIT 1), 
 'PLC_003', '罗克韦尔PLC', 'PLC', 'ControlLogix', '罗克韦尔',
 'EtherNet/IP', '192.168.2.10', 44818, NULL, 1000, 5000, 3,
 '化工车间控制柜', '化工车间控制PLC', 1, NOW(), 1, 'COLLECTOR_003', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'CHEM_PLANT_01' LIMIT 1), 
 'PH_001', 'pH传感器', '传感器', 'pH电极', '梅特勒',
 'Modbus', '192.168.2.11', 502, 4, 2000, 3000, 3,
 '化工车间反应釜', 'pH值监测传感器', 1, NOW(), 1, 'COLLECTOR_003', 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'CHEM_PLANT_01' LIMIT 1), 
 'LEVEL_001', '液位传感器', '传感器', '超声波液位计', '西门子',
 'Modbus', '192.168.2.12', 502, 5, 2000, 3000, 3,
 '化工车间储罐', '液位监测传感器', 1, NOW(), 1, 'COLLECTOR_003', 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'CHEM_PLANT_01' LIMIT 1), 
 'VALVE_001', '电动阀门', '执行器', '电动球阀', '费希尔',
 'Modbus', '192.168.2.13', 502, 6, 1000, 3000, 3,
 '化工车间管道', '流量控制阀门', 1, NOW(), 1, 'COLLECTOR_003', 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- 电力企业的设备
(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'SOLAR_PLANT_01' LIMIT 1), 
 'INVERTER_001', '光伏逆变器', '逆变器', '组串式逆变器', '华为',
 'Modbus', '192.168.3.10', 502, 7, 5000, 3000, 3,
 '太阳能电站1号区域', '光伏发电逆变器', 1, NOW(), 1, 'COLLECTOR_004', 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'SOLAR_PLANT_01' LIMIT 1), 
 'INVERTER_002', '光伏逆变器', '逆变器', '组串式逆变器', '华为',
 'Modbus', '192.168.3.11', 502, 8, 5000, 3000, 3,
 '太阳能电站2号区域', '光伏发电逆变器', 1, NOW(), 1, 'COLLECTOR_004', 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'SOLAR_PLANT_01' LIMIT 1), 
 'WEATHER_001', '气象站', '传感器', '气象监测站', '维萨拉',
 'Modbus', '192.168.3.12', 502, 9, 10000, 3000, 3,
 '太阳能电站气象站', '环境气象监测', 1, NOW(), 1, 'COLLECTOR_004', 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'SOLAR_PLANT_01' LIMIT 1), 
 'METER_001', '电能表', '仪表', '智能电能表', '威胜',
 'Modbus', '192.168.3.13', 502, 10, 5000, 3000, 3,
 '太阳能电站配电室', '发电量计量表', 1, NOW(), 1, 'COLLECTOR_004', 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_SITES WHERE F_SITE_CODE = 'SOLAR_PLANT_01' LIMIT 1), 
 'BATTERY_001', '储能电池', '储能设备', '磷酸铁锂电池', '比亚迪',
 'Modbus', '192.168.3.14', 502, 11, 5000, 3000, 3,
 '太阳能电站储能室', '储能电池系统', 1, NOW(), 1, 'COLLECTOR_004', 1, 5,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL);

-- ========================================
-- 5. 插入数据点数据
-- ========================================
INSERT INTO T_DATA_POINTS (
    Id, F_DEVICE_ID, F_POINT_CODE, F_POINT_NAME, F_DATA_TYPE, F_REGISTER_ADDRESS, F_REGISTER_TYPE,
    F_DATA_LENGTH, F_IS_SIGNED, F_BYTE_ORDER, F_SCALE_FACTOR, F_OFFSET, F_UNIT, F_DESCRIPTION,
    F_MIN_VALUE, F_MAX_VALUE, F_DEFAULT_VALUE, F_IS_READ_ONLY, F_IS_COLLECTION_ENABLED, F_COLLECTION_PRIORITY, F_STATUS, F_SORT_ORDER,
    ExtraProperties, ConcurrencyStamp, CreationTime, CreatorId, LastModificationTime, LastModifierId, IsDeleted, DeleterId, DeletionTime
) VALUES
-- PLC_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_001' LIMIT 1), 
 'PLC_TEMP_001', 'PLC温度1', 'Float', 'DB1.DBD0', 'DB',
 4, 1, 'BigEndian', 1.0, 0.0, '°C', 'PLC温度传感器1', -50.0, 150.0, '25.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_001' LIMIT 1), 
 'PLC_TEMP_002', 'PLC温度2', 'Float', 'DB1.DBD4', 'DB',
 4, 1, 'BigEndian', 1.0, 0.0, '°C', 'PLC温度传感器2', -50.0, 150.0, '25.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_001' LIMIT 1), 
 'PLC_PRESSURE_001', 'PLC压力1', 'Float', 'DB1.DBD8', 'DB',
 4, 1, 'BigEndian', 1.0, 0.0, 'MPa', 'PLC压力传感器1', 0.0, 10.0, '1.0', 1, 1, 1, 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_001' LIMIT 1), 
 'PLC_FLOW_001', 'PLC流量1', 'Float', 'DB1.DBD12', 'DB',
 4, 1, 'BigEndian', 1.0, 0.0, 'L/min', 'PLC流量传感器1', 0.0, 1000.0, '100.0', 1, 1, 1, 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_001' LIMIT 1), 
 'PLC_STATUS_001', 'PLC运行状态', 'Int16', 'DB1.DBX16.0', 'DB',
 2, 0, 'BigEndian', 1.0, 0.0, '', 'PLC运行状态', 0.0, 1.0, '1', 1, 1, 1, 1, 5,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- TEMP_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'TEMP_001' LIMIT 1), 
 'TEMP_AMBIENT', '环境温度', 'Float', '40001', 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '°C', '环境温度监测', -50.0, 150.0, '25.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- PRESS_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PRESS_001' LIMIT 1), 
 'PRESS_PIPE', '管道压力', 'Float', '40002', 'Holding',
 4, 1, 'BigEndian', 0.01, 0.0, 'MPa', '管道压力监测', 0.0, 10.0, '1.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- PLC_002的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_002' LIMIT 1), 
 'PLC_SPEED_001', '包装速度', 'Float', 'DB2.DBD0', 'DB',
 4, 1, 'BigEndian', 1.0, 0.0, 'pcs/min', '包装机运行速度', 0.0, 200.0, '100.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_002' LIMIT 1), 
 'PLC_COUNT_001', '包装计数', 'Int32', 'DB2.DBD4', 'DB',
 4, 0, 'BigEndian', 1.0, 0.0, 'pcs', '包装产品计数', 0.0, 999999.0, '0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- FLOW_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'FLOW_001' LIMIT 1), 
 'FLOW_RATE', '流量', 'Float', '40003', 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'L/min', '管道流量', 0.0, 500.0, '50.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- PLC_003的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_003' LIMIT 1), 
 'CHEM_TEMP_001', '反应温度', 'Float', '40004', 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '°C', '化学反应温度', 0.0, 200.0, '80.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PLC_003' LIMIT 1), 
 'CHEM_PRESSURE_001', '反应压力', 'Float', '40005', 'Holding',
 4, 1, 'BigEndian', 0.01, 0.0, 'MPa', '化学反应压力', 0.0, 5.0, '1.5', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- PH_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'PH_001' LIMIT 1), 
 'PH_VALUE', 'pH值', 'Float', '40006', 'Holding',
 4, 1, 'BigEndian', 0.01, 0.0, '', 'pH值', 0.0, 14.0, '7.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- LEVEL_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'LEVEL_001' LIMIT 1), 
 'LEVEL_VALUE', '液位', 'Float', '40007', 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '%', '储罐液位', 0.0, 100.0, '50.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- VALVE_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'VALVE_001' LIMIT 1), 
 'VALVE_POSITION', '阀门开度', 'Float', '40008', 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '%', '阀门开度', 0.0, 100.0, '0.0', 0, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'VALVE_001' LIMIT 1), 
 'VALVE_STATUS', '阀门状态', 'Int16', '40009', 'Holding',
 2, 0, 'BigEndian', 1.0, 0.0, '', '阀门开关状态', 0.0, 1.0, '0', 0, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- INVERTER_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_001' LIMIT 1), 
 'INV_POWER', '输出功率', 'Float', 0, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'kW', '逆变器输出功率', 0.0, 100.0, '0.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_001' LIMIT 1), 
 'INV_VOLTAGE', '输出电压', 'Float', 4, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'V', '逆变器输出电压', 0.0, 400.0, '220.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_001' LIMIT 1), 
 'INV_CURRENT', '输出电流', 'Float', 8, 'Holding',
 4, 1, 'BigEndian', 0.01, 0.0, 'A', '逆变器输出电流', 0.0, 200.0, '0.0', 1, 1, 1, 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_001' LIMIT 1), 
 'INV_FREQUENCY', '输出频率', 'Float', 12, 'Holding',
 4, 1, 'BigEndian', 0.01, 0.0, 'Hz', '逆变器输出频率', 45.0, 55.0, '50.0', 1, 1, 1, 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_001' LIMIT 1), 
 'INV_STATUS', '运行状态', 'Int16', 16, 'Holding',
 2, 0, 'BigEndian', 1.0, 0.0, '', '逆变器运行状态', 0.0, 1.0, '1', 1, 1, 1, 1, 5,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- INVERTER_002的数据点（类似INVERTER_001）
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_002' LIMIT 1), 
 'INV2_POWER', '输出功率', 'Float', 0, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'kW', '逆变器输出功率', 0.0, 100.0, '0.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'INVERTER_002' LIMIT 1), 
 'INV2_VOLTAGE', '输出电压', 'Float', 4, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'V', '逆变器输出电压', 0.0, 400.0, '220.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- WEATHER_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'WEATHER_001' LIMIT 1), 
 'WEATHER_TEMP', '环境温度', 'Float', 0, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '°C', '环境温度', -50.0, 60.0, '25.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'WEATHER_001' LIMIT 1), 
 'WEATHER_HUMIDITY', '环境湿度', 'Float', 4, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '%', '环境湿度', 0.0, 100.0, '60.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'WEATHER_001' LIMIT 1), 
 'WEATHER_WIND_SPEED', '风速', 'Float', 8, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'm/s', '风速', 0.0, 50.0, '2.0', 1, 1, 1, 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'WEATHER_001' LIMIT 1), 
 'WEATHER_SOLAR_RADIATION', '太阳辐射', 'Float', 12, 'Holding',
 4, 1, 'BigEndian', 1.0, 0.0, 'W/m²', '太阳辐射强度', 0.0, 1200.0, '800.0', 1, 1, 1, 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- METER_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'METER_001' LIMIT 1), 
 'METER_ACTIVE_POWER', '有功功率', 'Float', 0, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'kW', '有功功率', 0.0, 1000.0, '0.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'METER_001' LIMIT 1), 
 'METER_ENERGY', '累计电量', 'Float', 4, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'kWh', '累计发电量', 0.0, 999999.0, '0.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

-- BATTERY_001的数据点
(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'BATTERY_001' LIMIT 1), 
 'BATTERY_SOC', '电池电量', 'Float', 0, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '%', '电池剩余电量', 0.0, 100.0, '80.0', 1, 1, 1, 1, 1,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'BATTERY_001' LIMIT 1), 
 'BATTERY_VOLTAGE', '电池电压', 'Float', 4, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'V', '电池电压', 0.0, 1000.0, '400.0', 1, 1, 1, 1, 2,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'BATTERY_001' LIMIT 1), 
 'BATTERY_CURRENT', '电池电流', 'Float', 8, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, 'A', '电池电流', -200.0, 200.0, '0.0', 1, 1, 1, 1, 3,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL),

(UUID(), (SELECT Id FROM T_DEVICES WHERE F_DEVICE_CODE = 'BATTERY_001' LIMIT 1), 
 'BATTERY_TEMP', '电池温度', 'Float', 12, 'Holding',
 4, 1, 'BigEndian', 0.1, 0.0, '°C', '电池温度', -20.0, 60.0, '25.0', 1, 1, 1, 1, 4,
 '{}', UUID(), NOW(), NULL, NULL, NULL, 0, NULL, NULL);

-- ========================================
-- 6. 显示插入的数据统计
-- ========================================
SELECT '数据插入完成！' as Message;

SELECT 'Enterprises' as TableName, COUNT(*) as Count FROM T_ENTERPRISES
UNION ALL
SELECT 'Sites' as TableName, COUNT(*) as Count FROM T_SITES
UNION ALL
SELECT 'CollectorNodes' as TableName, COUNT(*) as Count FROM T_COLLECTOR_NODES
UNION ALL
SELECT 'Devices' as TableName, COUNT(*) as Count FROM T_DEVICES
UNION ALL
SELECT 'DataPoints' as TableName, COUNT(*) as Count FROM T_DATA_POINTS;

-- ========================================
-- 7. 验证数据关联关系
-- ========================================
SELECT 
    '企业-站点关联' as CheckType,
    e.F_ENTERPRISE_CODE,
    e.F_ENTERPRISE_NAME,
    COUNT(s.Id) as SiteCount
FROM T_ENTERPRISES e
LEFT JOIN T_SITES s ON e.Id = s.F_ENTERPRISE_ID
GROUP BY e.Id, e.F_ENTERPRISE_CODE, e.F_ENTERPRISE_NAME;

SELECT 
    '站点-设备关联' as CheckType,
    s.F_SITE_CODE,
    s.F_SITE_NAME,
    COUNT(d.Id) as DeviceCount
FROM T_SITES s
LEFT JOIN T_DEVICES d ON s.Id = d.F_SITE_ID
GROUP BY s.Id, s.F_SITE_CODE, s.F_SITE_NAME;

SELECT 
    '设备-数据点关联' as CheckType,
    d.F_DEVICE_CODE,
    d.F_DEVICE_NAME,
    COUNT(dp.Id) as DataPointCount
FROM T_DEVICES d
LEFT JOIN T_DATA_POINTS dp ON d.Id = dp.F_DEVICE_ID
GROUP BY d.Id, d.F_DEVICE_CODE, d.F_DEVICE_NAME;

SELECT 
    '采集节点-设备关联' as CheckType,
    cn.F_NODE_CODE,
    cn.F_NODE_NAME,
    COUNT(d.Id) as DeviceCount
FROM T_COLLECTOR_NODES cn
LEFT JOIN T_DEVICES d ON cn.F_NODE_CODE = d.F_COLLECTOR_NODE_CODE
GROUP BY cn.Id, cn.F_NODE_CODE, cn.F_NODE_NAME; 
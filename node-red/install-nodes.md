# Node-RED IoT节点包安装指南

## 必需的节点包

通过Node-RED Web界面安装以下节点包：

### 1. 数据库连接节点
- **node-red-node-mysql** - MySQL数据库连接
- **node-red-contrib-influxdb** - InfluxDB时序数据库连接

### 2. 协议和通信节点  
- **node-red-contrib-modbus** - Modbus协议支持
- **node-red-contrib-opcua** - OPC-UA协议支持

### 3. 实时监控界面节点
- **node-red-dashboard** - 基础仪表板
- **node-red-contrib-ui-led** - LED状态指示器
- **node-red-contrib-ui-level** - 水平仪表
- **node-red-node-ui-table** - 数据表格显示

### 4. 数据处理节点
- **node-red-contrib-json** - JSON数据处理
- **node-red-contrib-moment** - 时间处理

## 安装步骤

1. 访问 http://localhost:1880
2. 点击右上角菜单 ≡ (三条线)
3. 选择 "管理面板 (Manage palette)"
4. 点击 "安装" 标签页
5. 在搜索框中输入节点包名称
6. 点击 "安装" 按钮

## 验证安装

安装完成后，在节点面板左侧应该能看到：
- 存储分类下的MySQL节点
- 网络分类下的Modbus/OPC-UA节点  
- 仪表板分类下的UI节点

## 配置数据库连接

MySQL连接配置：
- 主机: `iot-mysql`
- 端口: `3306`
- 用户: `root`
- 密码: `iot123456`
- 数据库: `IoTDataCollection_Main`

InfluxDB连接配置：
- URL: `http://iot-influxdb:8086`
- Token: `iot-admin-token-12345678901234567890`
- 组织: `IOTDataCollection`
- 桶: `iot_data` 
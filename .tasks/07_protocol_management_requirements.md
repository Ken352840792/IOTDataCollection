# 协议管理需求规范

## 🎯 协议管理概述

IoT工业数据采集平台支持协议插件化架构，允许用户动态上传、管理和下发自定义协议插件，实现对各种工业设备的灵活支持。

## 📋 核心需求

### 1. 协议插件上传
- **文件格式**：支持.dll协议插件文件上传
- **接口验证**：自动验证协议是否实现IDeviceProtocol接口
- **版本管理**：支持同一协议的多版本并存
- **依赖检查**：检查协议插件的.NET依赖项
- **安全扫描**：基础安全性检查，防止恶意代码

### 2. 协议生命周期管理
- **协议注册**：上传后自动注册到协议工厂
- **状态管理**：启用/禁用/删除协议
- **测试验证**：协议功能测试和兼容性验证
- **更新机制**：协议版本升级和回滚
- **使用统计**：跟踪协议使用情况和性能指标

### 3. 协议配置管理
- **参数配置**：每个协议的特定参数设置
- **连接模板**：预设连接配置模板
- **设备映射**：协议与设备类型的映射关系
- **性能调优**：协议性能参数优化

### 4. 协议分发机制
- **选择性下发**：根据设备类型选择需要下发的采集端
- **全量下发**：向所有采集端推送协议更新
- **增量更新**：仅下发变更的协议文件
- **下发状态跟踪**：监控协议下发和加载状态

## 🏗️ 协议管理领域模型设计

### 协议包聚合根
**核心职责**：管理协议插件的完整生命周期
- **聚合根**：ProtocolPackage（协议包）
- **实体**：ProtocolVersion（协议版本）、ProtocolConfiguration（协议配置）
- **值对象**：ProtocolInfo、DependencyInfo、DistributionTarget

**业务规则**：
- 协议包名称全局唯一
- 同一时间只能有一个版本处于激活状态
- 协议删除前必须检查是否有设备在使用
- 协议更新需要验证向后兼容性
- 下发失败的协议需要自动回滚

### 协议分发聚合根
**核心职责**：管理协议向采集端的分发过程
- **聚合根**：ProtocolDistribution（协议分发）
- **实体**：DistributionTarget（分发目标）、DistributionLog（分发日志）
- **值对象**：DistributionStatus、TargetFilter、DistributionResult

**业务规则**：
- 分发目标根据设备编码或设备类型筛选
- 分发过程需要记录详细日志
- 分发失败自动重试3次
- 采集端确认加载成功后才标记分发完成

## 📱 协议管理界面设计

### 1. 协议管理主界面
**界面功能**：
- **协议列表**：显示所有已上传的协议插件
- **状态展示**：协议状态（启用/禁用/测试中）
- **版本信息**：协议版本、上传时间、使用设备数量
- **操作按钮**：上传、编辑、测试、删除、下发

**列表字段**：
- 协议名称、版本号、状态、支持设备类型
- 上传时间、最后更新时间、使用设备数量
- 文件大小、依赖项、兼容性等级

### 2. 协议上传界面
**上传功能**：
- **文件选择**：支持拖拽上传.dll文件
- **信息填写**：协议名称、版本号、描述信息
- **配置定义**：协议参数配置schema定义
- **兼容性设置**：支持的设备类型和协议标准

**验证流程**：
- 文件格式验证（.dll文件）
- 接口实现验证（IDeviceProtocol）
- 依赖项检查（.NET版本、第三方库）
- 安全性扫描（基础恶意代码检测）
- 命名冲突检查（协议名称唯一性）

### 3. 协议配置界面
**配置管理**：
- **参数定义**：协议特定参数的配置界面
- **连接模板**：常用连接配置的模板管理
- **默认值设置**：协议参数的默认值配置
- **验证规则**：参数值的验证规则定义

**配置示例**：
- Modbus协议：波特率、数据位、停止位、校验位
- OPC-UA协议：端点URL、安全策略、证书配置
- 自定义协议：厂商特定的通信参数

### 4. 协议分发界面
**分发控制**：
- **目标选择**：选择需要下发的采集端
- **分发策略**：全量下发、增量更新、指定设备
- **分发监控**：实时显示分发进度和状态
- **分发历史**：历史分发记录和结果查询

**分发选项**：
- **按设备编码**：选择特定设备的采集端
- **按设备类型**：选择支持特定协议的所有设备
- **按地理位置**：选择特定区域的采集端
- **全量分发**：向所有在线采集端分发

### 5. 协议测试界面
**测试功能**：
- **接口测试**：验证协议接口实现的正确性
- **连接测试**：测试协议与模拟设备的连接
- **性能测试**：评估协议的通信性能和稳定性
- **兼容性测试**：与现有系统的兼容性验证

**测试报告**：
- 测试结果概要（成功/失败/警告）
- 详细测试日志和错误信息
- 性能指标（连接时间、数据传输速率）
- 兼容性评级和建议

## 🔄 协议分发流程设计

### 分发触发方式
1. **手动分发**：用户在界面上主动触发分发
2. **自动分发**：新设备注册时自动下发匹配的协议
3. **定时分发**：定期检查协议更新并自动分发
4. **事件分发**：协议状态变更时触发分发

### 分发MQTT主题架构
**协议下发主题**：
- `protocol/download/{device_code}` - 向特定设备下发协议
- `protocol/download/broadcast` - 向所有设备广播协议
- `protocol/update/{device_code}` - 协议更新通知
- `protocol/status/{device_code}` - 协议加载状态反馈

**消息格式设计**：
```json
{
  "messageId": "uuid",
  "protocolName": "CustomModbus",
  "version": "1.2.0",
  "downloadUrl": "http://server/protocols/custom-modbus-1.2.0.dll",
  "checksum": "sha256hash",
  "dependencies": ["System.IO.Ports"],
  "targetDevices": ["device_001", "device_002"],
  "forceUpdate": false
}
```

### 分发确认机制
**采集端反馈**：
- 协议下载确认（下载成功/失败）
- 协议加载确认（加载成功/失败/依赖缺失）
- 协议测试结果（功能测试通过/失败）
- 协议运行状态（正常运行/异常/性能问题）

**分发重试策略**：
- 下载失败：自动重试3次，间隔5分钟
- 加载失败：检查依赖项，提供修复建议
- 超时处理：30分钟无响应视为分发失败
- 错误上报：详细错误信息回传到管理端

## 📊 数据存储设计

### MySQL协议管理表群

#### T_PROTOCOL_PACKAGES（协议包表）
**核心字段**：
- F_ID（主键，GUID）、F_PROTOCOL_NAME（协议名称，唯一）
- F_DESCRIPTION（协议描述）、F_VENDOR（厂商信息）
- F_CURRENT_VERSION_ID（当前版本ID，外键GUID）
- F_STATUS（协议状态：Active/Inactive/Testing）
- F_SUPPORTED_DEVICE_TYPES（支持设备类型，JSON格式）
- F_USAGE_COUNT（使用设备数量）、F_LAST_DISTRIBUTED（最后分发时间）

#### T_PROTOCOL_VERSIONS（协议版本表）
**核心字段**：
- F_ID（主键，GUID）、F_PROTOCOL_ID（协议ID，外键GUID）
- F_VERSION（版本号）、F_FILE_PATH（文件存储路径）
- F_FILE_SIZE（文件大小）、F_FILE_CHECKSUM（文件校验和）
- F_DEPENDENCIES（依赖项，JSON格式）、F_IS_COMPATIBLE（兼容性标记）
- F_UPLOAD_USER_ID（上传用户ID）、F_STATUS（版本状态）

#### T_PROTOCOL_CONFIGURATIONS（协议配置表）
**核心字段**：
- F_ID（主键，GUID）、F_PROTOCOL_ID（协议ID，外键GUID）
- F_CONFIG_NAME（配置名称）、F_CONFIG_SCHEMA（配置Schema，JSON）
- F_DEFAULT_VALUES（默认值，JSON）、F_VALIDATION_RULES（验证规则，JSON）
- F_IS_REQUIRED（是否必填）、F_DESCRIPTION（配置说明）

#### T_PROTOCOL_DISTRIBUTIONS（协议分发表）
**核心字段**：
- F_ID（主键，GUID）、F_PROTOCOL_VERSION_ID（协议版本ID，外键GUID）
- F_DISTRIBUTION_TYPE（分发类型：Manual/Auto/Scheduled）
- F_TARGET_FILTER（目标过滤条件，JSON）、F_TARGET_COUNT（目标数量）
- F_STATUS（分发状态：Pending/InProgress/Completed/Failed）
- F_START_TIME（分发开始时间）、F_COMPLETION_TIME（完成时间）

#### T_PROTOCOL_DISTRIBUTION_LOGS（协议分发日志表）
**核心字段**：
- F_ID（主键，GUID）、F_DISTRIBUTION_ID（分发ID，外键GUID）
- F_DEVICE_CODE（设备编码）、F_STATUS（分发状态）
- F_START_TIME（开始时间）、F_COMPLETION_TIME（完成时间）
- F_ERROR_MESSAGE（错误信息）、F_RETRY_COUNT（重试次数）

### InfluxDB协议监控数据

#### protocol_performance（协议性能）
**Tags**：protocol_name, version, device_code
**Fields**：connection_time, data_transfer_rate, error_count, success_rate
**用途**：监控协议运行性能和稳定性

#### protocol_distribution_events（协议分发事件）
**Tags**：distribution_id, device_code, event_type
**Fields**：event_timestamp, status, error_code, retry_count
**用途**：记录协议分发过程的详细事件

## 🔒 安全性设计

### 协议安全验证
- **数字签名验证**：验证协议包的数字签名
- **代码扫描**：基础恶意代码和病毒扫描
- **接口限制**：仅允许实现IDeviceProtocol接口的协议
- **沙箱测试**：在隔离环境中测试协议功能
- **权限控制**：协议上传和管理需要特定权限

### 分发安全机制
- **传输加密**：协议文件传输使用TLS加密
- **完整性校验**：使用SHA256校验文件完整性
- **访问控制**：采集端需要认证才能下载协议
- **审计日志**：记录所有协议操作的审计日志

## 🎯 用户体验设计

### 操作便利性
- **拖拽上传**：支持协议文件的拖拽上传
- **批量操作**：支持多个协议的批量管理
- **快速测试**：一键测试协议功能
- **智能推荐**：根据设备类型推荐适合的协议

### 状态可视化
- **进度显示**：实时显示分发进度和状态
- **状态图标**：直观的协议状态图标显示
- **统计报表**：协议使用情况的统计分析
- **告警提示**：协议异常的及时告警提示

这个协议管理功能为IoT工业数据采集平台提供了完整的协议插件化支持，实现了协议的全生命周期管理和智能分发机制。 
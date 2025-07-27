# DDD领域设计

## 🎯 领域驱动设计概述

IoT工业数据采集平台采用DDD（领域驱动设计）架构，将复杂的业务逻辑分解为9个独立的有界上下文（Bounded Context），每个领域专注于特定的业务职责。

## 🏗️ 领域架构分层

### 1. 领域层 (Domain Layer)
包含业务规则、聚合根、实体、值对象和领域服务，代表核心业务逻辑。

### 2. 应用层 (Application Layer)  
协调领域对象执行用例，处理跨领域的业务流程。

### 3. 基础设施层 (Infrastructure Layer)
提供技术能力支撑，包括数据持久化、外部系统集成、消息通信等。

### 4. 接口层 (Interface Layer)
暴露系统功能，包括Web API、用户界面、消息接口等。

## 📋 核心领域识别

### 领域1：组织管理领域 (Organization Domain)

**业务职责**：管理企业组织架构和层次关系

**核心概念**：
- **聚合根**：Enterprise（企业）
- **实体**：Site（站点）、Area（区域）、ProductionLine（生产线）、WorkCell（工作单元）
- **值对象**：OrganizationCode、OrganizationName、Location、ContactInfo

**业务规则**：
- 企业编码在系统内全局唯一
- 组织层次结构严格按照五级架构：企业→站点→区域→生产线→工作单元
- 下级组织编码在上级组织内唯一
- 组织删除需要检查是否有关联设备

**领域事件**：
- OrganizationCreated（组织创建）
- OrganizationStructureChanged（组织结构变更）
- OrganizationDeactivated（组织停用）

### 领域2：设备管理领域 (Device Domain)

**业务职责**：管理工业设备的基本信息、配置和状态

**核心概念**：
- **聚合根**：Device（设备）
- **实体**：DataPoint（数据点）
- **值对象**：DeviceCode、DeviceType、ConnectionConfig、DeviceStatus、ProtocolType
- **领域服务**：DeviceValidationService、DeviceConfigurationService

**业务规则**：
- 设备编码全局唯一且不可变更
- 设备必须关联到具体的工作单元
- 数据点地址在设备内唯一
- 设备状态变更必须记录变更历史
- 设备删除前需要停止所有采集任务

**领域事件**：
- DeviceRegistered（设备注册）
- DeviceConfigurationChanged（设备配置变更）
- DeviceStatusChanged（设备状态变更）
- DataPointAdded（数据点添加）

### 领域3：数据采集领域 (Data Collection Domain)

**业务职责**：管理数据采集任务的执行和数据质量控制

**核心概念**：
- **聚合根**：CollectionTask（采集任务）
- **实体**：CollectionRecord（采集记录）、CollectionSession（采集会话）
- **值对象**：SamplingInterval、DataValue、QualityCode、Timestamp
- **领域服务**：DataCollectionService、DataValidationService、DataQualityService

**业务规则**：
- 采集任务必须关联有效的设备和数据点
- 采集间隔不能小于设备协议的最小支持周期
- 数据采集失败时自动执行重试策略
- 采集数据必须通过质量验证
- 超出数据范围的值需要标记质量状态

**领域事件**：
- CollectionTaskStarted（采集任务启动）
- DataCollected（数据采集完成）
- DataValidationFailed（数据验证失败）
- CollectionTaskStopped（采集任务停止）

### 领域4：系统监控领域 (System Monitoring Domain)

**业务职责**：监控采集端系统资源和设备运行状态

**核心概念**：
- **聚合根**：CollectorNode（采集节点）
- **实体**：SystemMetric（系统指标）、PerformanceCounter（性能计数器）
- **值对象**：ResourceUsage、SystemHealth、AlertThreshold、AlertLevel
- **领域服务**：SystemMetricsCollectionService、HealthEvaluationService、AlertService

**业务规则**：
- 系统资源监控频率固定为30秒
- 告警阈值：CPU 80%、内存 85%、磁盘 90%
- 连续3个周期超过阈值才触发告警
- 告警状态变更必须记录历史
- 监控数据保留期限为3个月

**领域事件**：
- SystemResourceAlertRaised（系统资源告警触发）
- SystemHealthDegraded（系统健康状态下降）
- MonitoringConfigChanged（监控配置变更）
- AlertResolved（告警解除）

### 领域5：实时查询领域 (Real-time Query Domain)

**业务职责**：提供实时数据查询和历史数据分析能力

**核心概念**：
- **聚合根**：QuerySession（查询会话）
- **实体**：RealTimeQuery（实时查询）、QueryResult（查询结果）
- **值对象**：QueryCriteria、TimeRange、DataFilter、RefreshInterval
- **领域服务**：RealTimeQueryEngine、DataStreamService、QueryOptimizationService

**业务规则**：
- 实时查询刷新间隔可配置（1-5秒）
- 查询结果无分页限制，但有最大行数限制
- 查询权限基于用户的数据访问范围
- 查询性能必须在3秒内响应
- 支持多种数据展示格式

**领域事件**：
- QuerySessionStarted（查询会话开始）
- RealTimeDataUpdated（实时数据更新）
- QueryPerformanceWarning（查询性能警告）
- QuerySessionEnded（查询会话结束）

### 领域6：设备调试领域 (Device Debug Domain)

**业务职责**：提供设备连接测试、数据读写测试和数据库查询调试

**核心概念**：
- **聚合根**：DebugSession（调试会话）
- **实体**：DebugOperation（调试操作）、TestResult（测试结果）
- **值对象**：DebugCommand、ConnectionTest、DataPointTest、DatabaseQuery
- **领域服务**：DeviceTestService、ProtocolDiagnosticService、DatabaseQueryService

**业务规则**：
- 所有用户都可以创建调试会话，无权限限制
- 数据库查询仅允许SELECT、SHOW、DESCRIBE操作
- 查询结果最大返回10000行
- 调试操作超时时间为30秒
- 所有调试操作都必须记录审计日志

**领域事件**：
- DebugSessionStarted（调试会话开始）
- DeviceConnectionTested（设备连接测试）
- DatabaseQueryExecuted（数据库查询执行）
- DebugSessionEnded（调试会话结束）

### 领域7：规则引擎领域 (Rule Engine Domain)

**业务职责**：管理JavaScript规则的版本控制、编译和执行

**核心概念**：
- **聚合根**：Rule（规则）
- **实体**：RuleVersion（规则版本）、RuleExecution（规则执行）
- **值对象**：RuleCode、RuleStatus、ExecutionResult、PerformanceMetrics
- **领域服务**：RuleCompilationService、RuleExecutionService、RuleVersionControlService

**业务规则**：
- 规则版本采用语义化版本控制（Major.Minor.Patch）
- 同一时间只能有一个活跃版本
- 规则执行超时时间为5秒
- 规则变更需要通过语法验证
- 支持原子性热更新和快速回滚

**领域事件**：
- RuleCreated（规则创建）
- RuleVersionDeployed（规则版本部署）
- RuleExecutionCompleted（规则执行完成）
- RuleRollbackPerformed（规则回滚执行）

### 领域8：协议管理领域 (Protocol Management Domain)

**业务职责**：管理工业通信协议的插件化上传、版本控制和分发

**核心概念**：
- **聚合根**：ProtocolPackage（协议包）、ProtocolDistribution（协议分发）
- **实体**：ProtocolVersion（协议版本）、DistributionTarget（分发目标）、DistributionLog（分发日志）
- **值对象**：ProtocolInfo、DependencyInfo、DistributionStatus、TargetFilter
- **领域服务**：ProtocolValidationService、ProtocolDistributionService、ProtocolTestService

**业务规则**：
- 协议包名称全局唯一且不可变更
- 同一时间只能有一个版本处于激活状态
- 协议删除前必须检查是否有设备在使用
- 协议更新需要验证向后兼容性
- 分发失败的协议需要自动回滚
- 协议文件必须实现IDeviceProtocol接口

**领域事件**：
- ProtocolUploaded（协议上传）
- ProtocolVersionActivated（协议版本激活）
- ProtocolDistributionStarted（协议分发开始）
- ProtocolDistributionCompleted（协议分发完成）
- ProtocolLoadedOnCollector（协议在采集端加载）

### 领域9：身份权限领域 (Identity & Authorization Domain)

**业务职责**：管理用户身份认证和基于角色的权限控制

**核心概念**：
- **聚合根**：User（用户）
- **实体**：Role（角色）、Permission（权限）
- **值对象**：Username、PasswordHash、PermissionScope、DataAccessRange
- **领域服务**：AuthenticationService、AuthorizationService、PasswordPolicyService

**业务规则**：
- 用户名全局唯一且不可变更
- 密码必须符合复杂度要求
- 权限按模块和操作细分
- 数据权限按组织架构层次控制
- 用户状态变更记录审计日志

**领域事件**：
- UserRegistered（用户注册）
- UserLoggedIn（用户登录）
- PermissionGranted（权限授予）
- UserDeactivated（用户停用）

## 🔗 领域间协作关系

### 主要依赖关系
- **设备管理领域** 依赖 **组织管理领域** 的工作单元信息
- **数据采集领域** 依赖 **设备管理领域** 的设备和数据点信息
- **系统监控领域** 依赖 **设备管理领域** 的设备标识
- **实时查询领域** 依赖 **身份权限领域** 的数据访问控制
- **设备调试领域** 依赖 **设备管理领域** 的设备配置信息
- **协议管理领域** 依赖 **设备管理领域** 的设备类型信息
- **数据采集领域** 依赖 **协议管理领域** 的协议实现

### 领域事件协作
- 设备状态变更事件会触发监控领域的状态更新
- 采集任务启动事件会通知监控领域开始性能监控
- 用户权限变更事件会影响查询领域的数据访问范围
- 规则执行事件会记录到调试领域的操作历史
- 协议上传事件会触发设备管理领域的协议选项更新
- 协议分发事件会通知采集领域更新可用协议列表

## 🎯 DDD设计原则遵循

### 1. 单一职责原则
每个领域专注于单一业务职责，避免职责重叠和边界模糊。

### 2. 高内聚低耦合
领域内部高度内聚，领域间通过事件和接口进行松耦合协作。

### 3. 业务语言一致性
使用统一的业务术语，确保技术实现与业务概念的一致性。

### 4. 防腐层保护
通过防腐层隔离外部系统的复杂性，保护领域模型的纯洁性。

这个DDD领域设计为IoT工业数据采集平台提供了清晰的业务边界和职责划分，确保系统的可维护性和可扩展性。 
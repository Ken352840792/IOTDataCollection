# 数据库设计

## 📊 数据库架构概述

IoT工业数据采集平台采用多数据库混合架构，根据数据特性和访问模式选择最适合的存储技术，确保系统的高性能和高可用性。

## 🎯 数据库选型策略

### MySQL关系型数据库
**用途**：存储结构化配置数据和关系数据
- **组织架构数据**：企业、站点、区域、生产线、工作单元
- **设备配置信息**：设备基础信息、数据点配置、协议参数
- **用户权限数据**：用户、角色、权限、授权关系
- **系统配置数据**：监控配置、调试设置、规则定义
- **业务关系数据**：设备归属关系、组织层级关系

### InfluxDB时序数据库
**用途**：存储时间序列数据和监控指标
- **实时采集数据**：设备数据点的实时值和历史数据
- **系统监控数据**：CPU、内存、磁盘、网络使用率
- **设备状态数据**：设备连接状态、通信质量、运行状态
- **告警记录数据**：告警触发记录、处理记录、恢复记录
- **性能指标数据**：系统性能指标、查询性能记录

### SQLite本地数据库
**用途**：采集端本地数据缓存和断网保障
- **本地配置缓存**：设备配置的本地副本
- **待上传数据**：网络中断时的数据缓存
- **采集任务状态**：本地采集任务的执行状态
- **系统运行日志**：采集端运行日志和错误记录

### Redis缓存数据库
**用途**：高频访问数据缓存和会话存储
- **用户会话缓存**：JWT令牌、用户权限信息
- **热点数据缓存**：频繁查询的设备配置、组织架构
- **实时数据缓存**：最新的设备数据值、状态信息
- **计算结果缓存**：复杂查询结果、统计分析结果

## 📋 数据库命名规范

### 核心命名规则
- **表名前缀**：所有表名以 `T_` 开头
- **视图名前缀**：所有视图名以 `V_` 开头  
- **存储过程前缀**：所有存储过程以 `G_` 开头
- **字段名前缀**：所有字段名以 `F_` 开头
- **主键字段**：统一命名为 `F_ID`，GUID格式
- **外键字段**：以 `_ID` 结尾，GUID格式（如 F_DEVICE_ID）
- **索引命名**：`IDX_表名_字段名` 或 `UK_表名_字段名`（唯一索引）

### 标准字段要求
每个表必须包含以下标准字段：
- **F_ID**：主键，CHAR(36)，GUID格式（必须）
- **F_CREATED_BY**：创建人，VARCHAR(50)
- **F_CREATED_AT**：创建时间，TIMESTAMP
- **F_UPDATED_BY**：修改人，VARCHAR(50)  
- **F_UPDATED_AT**：修改时间，TIMESTAMP（自动更新）

### 主键设计规范
**GUID主键要求**：
- 所有表的主键字段统一命名为 `F_ID`
- 数据类型：CHAR(36)，存储标准GUID格式
- 示例格式：`550e8400-e29b-41d4-a716-446655440000`
- 生成策略：应用层生成UUID，确保全局唯一性
- 外键引用：所有外键字段也使用CHAR(36)类型

**GUID设计注意事项**：
- 使用标准UUID v4格式，确保随机性和唯一性
- 存储格式统一使用带连字符的36位字符串
- 应用层生成GUID，避免数据库层依赖
- 索引性能考虑：GUID作为聚集索引可能影响插入性能，建议业务频繁查询的表考虑复合索引优化
- 与InfluxDB关联时，推荐使用业务编码(device_code)而非GUID，提高查询性能

### 扩展字段预留
每个表预留10个扩展字段，便于后续功能扩展：
- **F_EXP_01** 到 **F_EXP_10**：VARCHAR(200)

## 🏢 MySQL数据库表结构设计

### 组织架构表群
**设计原则**：五级层次结构，支持组织架构的完整管理

#### T_ENTERPRISES（企业表）
**核心字段**：
- F_ID（主键，GUID）、F_CODE（企业编码，全局唯一）
- F_NAME（企业名称）、F_DESCRIPTION（企业描述）
- F_CONTACT_PERSON（联系人）、F_CONTACT_PHONE（联系电话）
- F_ADDRESS（企业地址）、F_IS_ENABLED（是否启用）

#### T_SITES（站点表）  
**核心字段**：
- F_ID（主键，GUID）、F_ENTERPRISE_ID（企业ID，外键GUID）
- F_CODE（站点编码，企业内唯一）、F_NAME（站点名称）
- F_LOCATION（地理位置）、F_DESCRIPTION（站点描述）

#### T_AREAS（区域表）
**核心字段**：
- F_ID（主键，GUID）、F_SITE_ID（站点ID，外键GUID）
- F_CODE（区域编码，站点内唯一）、F_NAME（区域名称）
- F_DESCRIPTION（区域描述）、F_AREA_TYPE（区域类型）

#### T_PRODUCTION_LINES（生产线表）
**核心字段**：
- F_ID（主键，GUID）、F_AREA_ID（区域ID，外键GUID）
- F_CODE（生产线编码，区域内唯一）、F_NAME（生产线名称）
- F_CAPACITY（生产能力）、F_STATUS（运行状态）

#### T_WORK_CELLS（工作单元表）
**核心字段**：
- F_ID（主键，GUID）、F_PRODUCTION_LINE_ID（生产线ID，外键GUID）
- F_CODE（工作单元编码，生产线内唯一）、F_NAME（工作单元名称）
- F_CELL_TYPE（单元类型）、F_DESCRIPTION（单元描述）

### 设备管理表群
**设计原则**：设备与数据点分离，支持多协议设备管理

#### T_DEVICES（设备表）
**核心字段**：
- F_ID（主键，GUID）、F_DEVICE_CODE（设备编码，全局唯一）
- F_DEVICE_NAME（设备名称）、F_WORK_CELL_ID（工作单元ID，外键GUID）
- F_PROTOCOL_TYPE（协议类型）、F_IP_ADDRESS（IP地址）
- F_PORT（端口号）、F_CONNECTION_PARAMS（连接参数，JSON格式）
- F_STATUS（设备状态）、F_LAST_COMMUNICATION_TIME（最后通讯时间）

#### T_DATA_POINTS（数据点表）
**核心字段**：
- F_ID（主键，GUID）、F_DEVICE_ID（设备ID，外键GUID）
- F_POINT_NAME（数据点名称）、F_ADDRESS（寄存器地址）
- F_DATA_TYPE（数据类型）、F_UNIT（单位）
- F_SCALE_FACTOR（缩放因子）、F_MIN_VALUE（最小值）
- F_MAX_VALUE（最大值）、F_IS_ENABLED（是否启用）

### 系统监控表群
**设计原则**：支持系统资源监控和告警配置管理

#### T_MONITORING_CONFIGS（监控配置表）
**核心字段**：
- F_ID（主键，GUID）、F_DEVICE_CODE（设备编码，外键）
- F_COLLECTION_INTERVAL（采集间隔，默认30秒）
- F_CPU_THRESHOLD（CPU告警阈值，默认80%）
- F_MEMORY_THRESHOLD（内存告警阈值，默认85%）
- F_DISK_THRESHOLD（磁盘告警阈值，默认90%）
- F_NETWORK_THRESHOLD（网络流量阈值）、F_IS_ENABLED（是否启用）

#### T_DEBUG_SESSIONS（调试会话表）
**核心字段**：
- F_ID（主键，GUID）、F_SESSION_ID（会话ID，唯一）
- F_DEVICE_CODE（设备编码）、F_USER_ID（用户ID，外键GUID）
- F_STATUS（会话状态）、F_START_TIME（开始时间）
- F_END_TIME（结束时间）、F_OPERATION_COUNT（操作次数）

#### T_DEBUG_OPERATIONS（调试操作表）
**核心字段**：
- F_ID（主键，GUID）、F_SESSION_ID（会话ID，外键GUID）
- F_OPERATION_TYPE（操作类型：connection/read/write/mysql_query/influxdb_query）
- F_TARGET（操作目标）、F_REQUEST_DATA（请求数据，JSON）
- F_RESPONSE_DATA（响应数据，JSON）、F_EXECUTION_TIME（执行时间）
- F_STATUS（执行状态）、F_ERROR_MESSAGE（错误信息）

### 实时查询表群
**设计原则**：支持用户自定义查询配置和查询历史管理

#### T_REALTIME_QUERY_CONFIGS（实时查询配置表）
**核心字段**：
- F_ID（主键，GUID）、F_USER_ID（用户ID，外键GUID）
- F_QUERY_NAME（查询名称）、F_DEVICE_CODES（设备编码列表，JSON）
- F_DATA_POINTS（数据点列表，JSON）、F_REFRESH_INTERVAL（刷新间隔）
- F_MAX_RESULTS（最大结果数）、F_IS_DEFAULT（是否默认查询）

### 规则引擎表群
**设计原则**：支持JavaScript规则的版本控制和热更新

#### T_JAVASCRIPT_RULES（JavaScript规则表）
**核心字段**：
- F_ID（主键，GUID）、F_RULE_ID（规则ID，唯一）
- F_RULE_NAME（规则名称）、F_DEVICE_CODE（设备编码，外键）
- F_RULE_CODE（规则代码）、F_VERSION（当前版本）
- F_IS_ACTIVE（是否激活）、F_LAST_DEPLOYED（最后部署时间）

#### T_RULE_VERSIONS（规则版本表）
**核心字段**：
- F_ID（主键，GUID）、F_RULE_ID（规则ID，外键GUID）
- F_VERSION（版本号）、F_RULE_CODE（规则代码）
- F_CODE_HASH（代码哈希）、F_STATUS（版本状态）
- F_PERFORMANCE_METRICS（性能指标，JSON）、F_DEPLOYED_AT（部署时间）

### 用户权限表群
**设计原则**：完整的RBAC权限管理体系

#### T_USERS（用户表）
**核心字段**：
- F_ID（主键，GUID）、F_USERNAME（用户名，唯一）
- F_PASSWORD_HASH（密码哈希）、F_EMAIL（邮箱）
- F_REAL_NAME（真实姓名）、F_PHONE（手机号）
- F_IS_ENABLED（是否启用）、F_LAST_LOGIN_TIME（最后登录时间）

#### T_ROLES（角色表）
**核心字段**：
- F_ID（主键，GUID）、F_ROLE_CODE（角色编码，唯一）
- F_ROLE_NAME（角色名称）、F_DESCRIPTION（角色描述）
- F_IS_ENABLED（是否启用）、F_DATA_SCOPE（数据权限范围）

#### T_PERMISSIONS（权限表）
**核心字段**：
- F_ID（主键，GUID）、F_PERMISSION_CODE（权限编码，唯一）
- F_PERMISSION_NAME（权限名称）、F_MODULE（所属模块）
- F_ACTION（操作类型）、F_DESCRIPTION（权限描述）

#### T_USER_ROLES（用户角色关联表）
**核心字段**：
- F_ID（主键，GUID）、F_USER_ID（用户ID，外键GUID）
- F_ROLE_ID（角色ID，外键GUID）、F_ASSIGNED_AT（分配时间）

#### T_ROLE_PERMISSIONS（角色权限关联表）
**核心字段**：
- F_ID（主键，GUID）、F_ROLE_ID（角色ID，外键GUID）
- F_PERMISSION_ID（权限ID，外键GUID）、F_GRANTED_AT（授权时间）

### 协议管理表群
**设计原则**：支持协议插件化的完整生命周期管理和分发控制

#### T_PROTOCOL_PACKAGES（协议包表）
**核心字段**：
- F_ID（主键，GUID）、F_PROTOCOL_NAME（协议名称，全局唯一）
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
- F_UPLOAD_USER_ID（上传用户ID，外键GUID）、F_STATUS（版本状态）

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

## 📈 InfluxDB时序数据库设计

### 数据保留策略
**三级数据保留策略**：
- **实时数据**：原始精度，保留24小时
- **监控数据**：30秒精度，保留3个月  
- **历史数据**：1小时精度，保留1年

### Measurement设计

#### device_data（设备数据）
**Tags**：device_code, data_point, quality
**Fields**：value, timestamp, sequence_number
**用途**：存储所有设备的实时采集数据

#### system_metrics（系统监控）
**Tags**：device_code, metric_type
**Fields**：cpu_usage, memory_usage, disk_usage, network_speed
**用途**：存储采集端系统资源使用情况

#### device_status（设备状态）
**Tags**：device_code, status_type
**Fields**：status_value, connection_quality, response_time
**用途**：存储设备连接状态和通信质量

#### system_alerts（系统告警）
**Tags**：device_code, alert_type, alert_level
**Fields**：threshold_value, actual_value, resolved
**用途**：存储系统告警记录和处理历史

#### debug_operations（调试操作）
**Tags**：device_code, operation_type, user_id, result
**Fields**：response_time, error_message, operation_detail
**用途**：存储调试操作记录和性能指标

#### protocol_performance（协议性能）
**Tags**：protocol_name, version, device_code
**Fields**：connection_time, data_transfer_rate, error_count, success_rate
**用途**：监控协议运行性能和稳定性

#### protocol_distribution_events（协议分发事件）
**Tags**：distribution_id, device_code, event_type
**Fields**：event_timestamp, status, error_code, retry_count
**用途**：记录协议分发过程的详细事件

## 🔗 数据关联策略

### 跨数据库关联
**关联键设计**：
- **device_code**：作为MySQL和InfluxDB的主要关联键（业务编码）
- **F_ID (GUID)**：MySQL表间的主要关联键
- **user_id**：用户相关数据的关联键（GUID格式）
- **session_id**：调试会话数据的关联键（GUID格式）
- **rule_id**：规则相关数据的关联键（GUID格式）

**GUID关联优势**：
- 全局唯一性，避免分布式环境下的ID冲突
- 支持数据库分片和迁移
- 便于跨系统数据同步和集成
- 提高数据安全性，避免ID猜测攻击

### 数据一致性保证
**策略选择**：
- **最终一致性**：跨数据库的数据同步采用最终一致性
- **事务一致性**：单数据库内的关联操作保证事务一致性
- **补偿机制**：数据同步失败时的补偿和重试机制

## 📊 索引设计策略

### MySQL索引设计
**主要索引**：
- **唯一索引**：所有编码字段（企业编码、设备编码等）
- **复合索引**：层级关系查询（企业-站点-区域）
- **外键索引**：所有外键字段自动建立索引
- **查询索引**：根据常用查询条件建立复合索引

### InfluxDB索引优化
**Tag索引**：
- 所有Tag字段自动建立索引
- 控制Tag的基数，避免高基数Tag
- 优化Tag顺序，高选择性Tag在前

## 🔒 数据安全设计

### 敏感数据保护
**加密策略**：
- **密码哈希**：使用BCrypt进行密码哈希
- **敏感字段**：联系方式等敏感信息进行AES加密
- **传输加密**：数据库连接使用SSL/TLS加密

### 数据备份策略
**备份机制**：
- **MySQL**：定期全量备份 + 增量备份
- **InfluxDB**：快照备份 + 连续备份
- **配置文件**：版本控制管理
- **异地备份**：重要数据异地备份保障

### 数据清理策略
**自动清理**：
- **调试记录**：1年后自动清理
- **监控数据**：3个月后自动清理
- **日志数据**：根据配置定期清理
- **临时数据**：会话数据定期清理

这个数据库设计方案为IoT工业数据采集平台提供了完整的数据存储架构，确保数据的完整性、一致性和高性能访问。 
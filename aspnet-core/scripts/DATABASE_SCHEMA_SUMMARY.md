# IoT工业数据采集平台 - 数据库表结构设计总结

## 📋 数据库命名规范

### ✅ 命名约定
- **表名前缀**: `T_` - 所有业务表名以T_开头
- **字段名前缀**: `F_` - 所有业务字段名以F_开头  
- **视图名前缀**: `V_` - 所有视图名以V_开头
- **存储过程前缀**: `G_` - 所有存储过程名以G_开头
- **扩展字段**: 每个表保留10个扩展字段 `F_EXP_01` 到 `F_EXP_10`

### ✅ ABP审计字段（自动包含）
- `Id` - 主键
- `CreationTime` - 创建时间
- `CreatorId` - 创建者ID
- `LastModificationTime` - 最后修改时间
- `LastModifierId` - 最后修改者ID
- `IsDeleted` - 软删除标记
- `DeleterId` - 删除者ID
- `DeletionTime` - 删除时间
- `ExtraProperties` - 额外属性（JSON）
- `ConcurrencyStamp` - 并发戳

## 📊 核心业务表结构

### 1. 组织管理相关表

#### 🏢 T_ENTERPRISES (企业表)
```sql
CREATE TABLE T_ENTERPRISES (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    F_ENTERPRISE_CODE       VARCHAR(64)     NOT NULL UNIQUE,    -- 企业编码
    F_ENTERPRISE_NAME       VARCHAR(256)    NOT NULL,           -- 企业名称
    F_SHORT_NAME           VARCHAR(128),                       -- 企业简称
    F_ENTERPRISE_TYPE      VARCHAR(64),                        -- 企业类型
    F_CONTACT_PERSON       VARCHAR(128),                       -- 联系人
    F_CONTACT_PHONE        VARCHAR(32),                        -- 联系电话
    F_ADDRESS              VARCHAR(512),                       -- 企业地址
    F_DESCRIPTION          VARCHAR(1000),                      -- 企业描述
    F_STATUS               INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER           INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10   VARCHAR(500),
    
    -- ABP审计字段...
);
```

#### 🏭 T_SITES (站点表)
```sql
CREATE TABLE T_SITES (
    Id                      CHAR(36)        NOT NULL PRIMARY KEY,
    F_ENTERPRISE_ID         CHAR(36)        NOT NULL,           -- 企业ID (外键)
    F_SITE_CODE            VARCHAR(64)     NOT NULL,           -- 站点编码
    F_SITE_NAME            VARCHAR(256)    NOT NULL,           -- 站点名称
    F_SHORT_NAME           VARCHAR(128),                       -- 站点简称
    F_SITE_TYPE            VARCHAR(64),                        -- 站点类型
    F_ADDRESS              VARCHAR(512),                       -- 站点地址
    F_MANAGER              VARCHAR(128),                       -- 负责人
    F_CONTACT_PHONE        VARCHAR(32),                        -- 联系电话
    F_DESCRIPTION          VARCHAR(1000),                      -- 站点描述
    F_STATUS               INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER           INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10   VARCHAR(500),
    
    UNIQUE KEY (F_ENTERPRISE_ID, F_SITE_CODE)
);
```

### 2. 设备管理相关表

#### 🔧 T_DEVICES (设备表)
```sql
CREATE TABLE T_DEVICES (
    Id                          CHAR(36)        NOT NULL PRIMARY KEY,
    F_SITE_ID                   CHAR(36)        NOT NULL,           -- 站点ID (外键)
    F_DEVICE_CODE              VARCHAR(64)     NOT NULL,           -- 设备编码
    F_DEVICE_NAME              VARCHAR(256)    NOT NULL,           -- 设备名称
    F_DEVICE_TYPE              VARCHAR(64),                        -- 设备类型
    F_DEVICE_MODEL             VARCHAR(128),                       -- 设备型号
    F_MANUFACTURER             VARCHAR(128),                       -- 设备制造商
    F_COMMUNICATION_PROTOCOL   VARCHAR(64),                        -- 通信协议
    F_IP_ADDRESS               VARCHAR(45),                        -- IP地址
    F_PORT                     INT,                                -- 端口号
    F_SLAVE_ADDRESS            INT,                                -- 从站地址
    F_COLLECTION_INTERVAL      INT             DEFAULT 1000,       -- 采集间隔(ms)
    F_TIMEOUT                  INT             DEFAULT 5000,       -- 超时时间(ms)
    F_RETRY_COUNT              INT             DEFAULT 3,          -- 重试次数
    F_LOCATION                 VARCHAR(512),                       -- 设备位置
    F_DESCRIPTION              VARCHAR(1000),                      -- 设备描述
    F_CONNECTION_STATUS        INT             DEFAULT 0,          -- 连接状态
    F_LAST_COMMUNICATION_TIME  DATETIME,                           -- 最后通信时间
    F_IS_COLLECTION_ENABLED    BOOLEAN         DEFAULT TRUE,       -- 是否启用采集
    F_STATUS                   INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER               INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10       VARCHAR(500),
    
    UNIQUE KEY (F_SITE_ID, F_DEVICE_CODE)
);
```

#### 📊 T_DATA_POINTS (数据点表)
```sql
CREATE TABLE T_DATA_POINTS (
    Id                         CHAR(36)        NOT NULL PRIMARY KEY,
    F_DEVICE_ID                CHAR(36)        NOT NULL,           -- 设备ID (外键)
    F_POINT_CODE              VARCHAR(64)     NOT NULL,           -- 数据点编码
    F_POINT_NAME              VARCHAR(256)    NOT NULL,           -- 数据点名称
    F_DATA_TYPE               VARCHAR(32)     NOT NULL,           -- 数据类型
    F_REGISTER_ADDRESS        INT,                                -- 寄存器地址
    F_REGISTER_TYPE           VARCHAR(32),                        -- 寄存器类型
    F_DATA_LENGTH             INT,                                -- 数据长度
    F_IS_SIGNED               BOOLEAN         DEFAULT FALSE,      -- 是否有符号
    F_BYTE_ORDER              VARCHAR(16),                        -- 字节序
    F_SCALE_FACTOR            DOUBLE          DEFAULT 1.0,        -- 缩放比例
    F_OFFSET                  DOUBLE          DEFAULT 0.0,        -- 偏移量
    F_UNIT                    VARCHAR(32),                        -- 单位
    F_DESCRIPTION             VARCHAR(1000),                      -- 数据点描述
    F_MIN_VALUE               DOUBLE,                             -- 最小值
    F_MAX_VALUE               DOUBLE,                             -- 最大值
    F_DEFAULT_VALUE           VARCHAR(100),                       -- 默认值
    F_IS_READ_ONLY            BOOLEAN         DEFAULT TRUE,       -- 是否只读
    F_IS_COLLECTION_ENABLED   BOOLEAN         DEFAULT TRUE,       -- 是否启用采集
    F_COLLECTION_PRIORITY     INT             DEFAULT 2,          -- 采集优先级
    F_LAST_COLLECTION_TIME    DATETIME,                           -- 最后采集时间
    F_LAST_VALUE              VARCHAR(500),                       -- 最后采集值
    F_STATUS                  INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER              INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10      VARCHAR(500),
    
    UNIQUE KEY (F_DEVICE_ID, F_POINT_CODE)
);
```

### 3. 规则引擎相关表

#### 🧠 T_JAVASCRIPT_RULES (JavaScript规则表)
```sql
CREATE TABLE T_JAVASCRIPT_RULES (
    Id                         CHAR(36)        NOT NULL PRIMARY KEY,
    F_DEVICE_ID               CHAR(36),                           -- 设备ID (外键，可空)
    F_RULE_CODE               VARCHAR(64)     NOT NULL UNIQUE,    -- 规则编码
    F_RULE_NAME               VARCHAR(256)    NOT NULL,           -- 规则名称
    F_RULE_TYPE               VARCHAR(64),                        -- 规则类型
    F_TRIGGER_CONDITION       VARCHAR(128),                       -- 触发条件
    F_SCRIPT_CODE             LONGTEXT        NOT NULL,           -- JavaScript脚本
    F_VERSION                 VARCHAR(32)     NOT NULL,           -- 版本号
    F_EXECUTION_PRIORITY      INT             DEFAULT 2,          -- 执行优先级
    F_TIMEOUT                 INT             DEFAULT 5000,       -- 超时时间(ms)
    F_MEMORY_LIMIT            BIGINT          DEFAULT 10485760,   -- 内存限制(bytes)
    F_IS_HOT_UPDATE_ENABLED   BOOLEAN         DEFAULT TRUE,       -- 是否启用热更新
    F_LAST_EXECUTION_TIME     DATETIME,                           -- 最后执行时间
    F_EXECUTION_COUNT         BIGINT          DEFAULT 0,          -- 执行次数
    F_LAST_EXECUTION_RESULT   VARCHAR(32),                        -- 最后执行结果
    F_LAST_ERROR_MESSAGE      VARCHAR(2000),                      -- 最后错误信息
    F_DESCRIPTION             VARCHAR(1000),                      -- 规则描述
    F_IS_ENABLED              BOOLEAN         DEFAULT TRUE,       -- 是否启用
    F_STATUS                  INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER              INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10      VARCHAR(500)
);
```

#### 🌊 T_NODERED_FLOWS (Node-RED流程表)
```sql
CREATE TABLE T_NODERED_FLOWS (
    Id                           CHAR(36)        NOT NULL PRIMARY KEY,
    F_FLOW_CODE                 VARCHAR(64)     NOT NULL UNIQUE,    -- 流程编码
    F_FLOW_NAME                 VARCHAR(256)    NOT NULL,           -- 流程名称
    F_FLOW_TYPE                 VARCHAR(64),                        -- 流程类型
    F_FLOW_CATEGORY             VARCHAR(64),                        -- 流程分类
    F_FLOW_CONFIG               LONGTEXT        NOT NULL,           -- Node-RED流程JSON
    F_VERSION                   VARCHAR(32)     NOT NULL,           -- 版本号
    F_TAB_ID                    VARCHAR(64),                        -- Node-RED标签页ID
    F_EXECUTION_PRIORITY        INT             DEFAULT 2,          -- 执行优先级
    F_MAX_PROCESSING_RATE       INT             DEFAULT 1000,       -- 最大处理速率
    F_IS_HOT_DEPLOY_ENABLED     BOOLEAN         DEFAULT TRUE,       -- 是否启用热部署
    F_LAST_DEPLOY_TIME          DATETIME,                           -- 最后部署时间
    F_LAST_EXECUTION_TIME       DATETIME,                           -- 最后执行时间
    F_EXECUTION_COUNT           BIGINT          DEFAULT 0,          -- 执行次数
    F_PROCESSED_MESSAGE_COUNT   BIGINT          DEFAULT 0,          -- 处理消息数
    F_ERROR_MESSAGE_COUNT       BIGINT          DEFAULT 0,          -- 错误消息数
    F_LAST_DEPLOY_STATUS        VARCHAR(32),                        -- 最后部署状态
    F_LAST_ERROR_MESSAGE        VARCHAR(2000),                      -- 最后错误信息
    F_DESCRIPTION               VARCHAR(1000),                      -- 流程描述
    F_AUTHOR                    VARCHAR(128),                       -- 流程作者
    F_RELATED_DEVICES           TEXT,                               -- 关联设备(JSON)
    F_INPUT_TOPICS              TEXT,                               -- 输入主题(JSON)
    F_OUTPUT_TOPICS             TEXT,                               -- 输出主题(JSON)
    F_IS_ENABLED                BOOLEAN         DEFAULT TRUE,       -- 是否启用
    F_STATUS                    INT             DEFAULT 1,          -- 状态
    F_SORT_ORDER                INT             DEFAULT 0,          -- 排序号
    
    -- 扩展字段
    F_EXP_01 TO F_EXP_10        VARCHAR(500)
);
```

## 🎯 索引设计

### 🔍 性能优化索引
```sql
-- 企业表索引
CREATE UNIQUE INDEX IX_T_ENTERPRISES_F_ENTERPRISE_CODE ON T_ENTERPRISES(F_ENTERPRISE_CODE);
CREATE INDEX IX_T_ENTERPRISES_F_ENTERPRISE_NAME ON T_ENTERPRISES(F_ENTERPRISE_NAME);
CREATE INDEX IX_T_ENTERPRISES_F_STATUS ON T_ENTERPRISES(F_STATUS);

-- 站点表索引
CREATE UNIQUE INDEX IX_T_SITES_F_ENTERPRISE_ID_F_SITE_CODE ON T_SITES(F_ENTERPRISE_ID, F_SITE_CODE);
CREATE INDEX IX_T_SITES_F_SITE_NAME ON T_SITES(F_SITE_NAME);
CREATE INDEX IX_T_SITES_F_STATUS ON T_SITES(F_STATUS);

-- 设备表索引
CREATE UNIQUE INDEX IX_T_DEVICES_F_SITE_ID_F_DEVICE_CODE ON T_DEVICES(F_SITE_ID, F_DEVICE_CODE);
CREATE INDEX IX_T_DEVICES_F_DEVICE_NAME ON T_DEVICES(F_DEVICE_NAME);
CREATE INDEX IX_T_DEVICES_F_CONNECTION_STATUS ON T_DEVICES(F_CONNECTION_STATUS);
CREATE INDEX IX_T_DEVICES_F_IS_COLLECTION_ENABLED ON T_DEVICES(F_IS_COLLECTION_ENABLED);
CREATE INDEX IX_T_DEVICES_F_STATUS ON T_DEVICES(F_STATUS);

-- 数据点表索引
CREATE UNIQUE INDEX IX_T_DATA_POINTS_F_DEVICE_ID_F_POINT_CODE ON T_DATA_POINTS(F_DEVICE_ID, F_POINT_CODE);
CREATE INDEX IX_T_DATA_POINTS_F_POINT_NAME ON T_DATA_POINTS(F_POINT_NAME);
CREATE INDEX IX_T_DATA_POINTS_F_IS_COLLECTION_ENABLED ON T_DATA_POINTS(F_IS_COLLECTION_ENABLED);
CREATE INDEX IX_T_DATA_POINTS_F_COLLECTION_PRIORITY ON T_DATA_POINTS(F_COLLECTION_PRIORITY);
CREATE INDEX IX_T_DATA_POINTS_F_STATUS ON T_DATA_POINTS(F_STATUS);

-- JavaScript规则表索引
CREATE UNIQUE INDEX IX_T_JAVASCRIPT_RULES_F_RULE_CODE ON T_JAVASCRIPT_RULES(F_RULE_CODE);
CREATE INDEX IX_T_JAVASCRIPT_RULES_F_RULE_NAME ON T_JAVASCRIPT_RULES(F_RULE_NAME);
CREATE INDEX IX_T_JAVASCRIPT_RULES_F_DEVICE_ID ON T_JAVASCRIPT_RULES(F_DEVICE_ID);
CREATE INDEX IX_T_JAVASCRIPT_RULES_F_IS_ENABLED ON T_JAVASCRIPT_RULES(F_IS_ENABLED);
CREATE INDEX IX_T_JAVASCRIPT_RULES_F_STATUS ON T_JAVASCRIPT_RULES(F_STATUS);

-- Node-RED流程表索引
CREATE UNIQUE INDEX IX_T_NODERED_FLOWS_F_FLOW_CODE ON T_NODERED_FLOWS(F_FLOW_CODE);
CREATE INDEX IX_T_NODERED_FLOWS_F_FLOW_NAME ON T_NODERED_FLOWS(F_FLOW_NAME);
CREATE INDEX IX_T_NODERED_FLOWS_F_FLOW_TYPE ON T_NODERED_FLOWS(F_FLOW_TYPE);
CREATE INDEX IX_T_NODERED_FLOWS_F_IS_ENABLED ON T_NODERED_FLOWS(F_IS_ENABLED);
CREATE INDEX IX_T_NODERED_FLOWS_F_STATUS ON T_NODERED_FLOWS(F_STATUS);
```

## 📈 数据库设计特点

### ✅ 企业级特性
1. **符合命名规范**: 严格遵循T_/F_命名约定
2. **完整审计功能**: ABP框架自动提供创建、修改、删除审计
3. **软删除支持**: 逻辑删除，数据安全
4. **扩展性设计**: 每表预留10个扩展字段
5. **高性能索引**: 针对查询场景优化的索引设计
6. **数据完整性**: 外键约束和唯一性约束
7. **UTF8MB4字符集**: 完整支持Unicode字符

### ✅ IoT专业特性
1. **设备层次化管理**: 企业→站点→设备→数据点
2. **通信协议适配**: 支持Modbus、OPC UA等主流协议
3. **实时数据采集**: 完整的采集配置和状态管理
4. **双层规则引擎**: 边缘JavaScript + 服务端Node-RED
5. **状态监控**: 连接状态、采集状态实时跟踪
6. **性能优化**: 优先级管理、批量处理支持

## 🚀 后续扩展预留

已为以下功能预留扩展字段和设计空间：
- 监控告警系统 (T_ALERTS, T_MONITORING_THRESHOLDS)
- 设备调试系统 (T_DEBUG_SESSIONS, T_DEBUG_LOGS)  
- 实时查询系统 (T_QUERY_SESSIONS)
- 协议管理系统 (T_PROTOCOL_PACKAGES, T_PROTOCOL_DISTRIBUTIONS)
- 采集节点管理 (T_COLLECTOR_NODES)

---
**生成时间**: 2025-01-14  
**项目**: IoT工业数据采集平台  
**框架**: ABP + .NET 8 + MySQL  
**设计原则**: DDD + Clean Architecture 
# IoT工业数据采集平台 - 项目概览

## 📋 项目基本信息
- **项目名称**: IoT工业数据采集平台
- **技术架构**: DDD + Clean Architecture
- **技术栈**: .NET 8 + NanoMQ + MySQL + InfluxDB + Vue 3.0
- **开发模式**: RIPER-5协议
- **创建时间**: 2025-01-14
- **项目状态**: 规划阶段

## 🎯 核心需求
- **数据采集**: PLC、工控机、SOCKET等设备数据采集
- **系统监控**: 采集端CPU、内存、磁盘、网络监控
- **实时查询**: 设备数据实时查询（1-5秒刷新）
- **设备调试**: 设备连接测试、数据点读写、数据库查询
- **权限管理**: RBAC权限体系
- **数据存储**: MySQL配置数据 + InfluxDB时序数据

## 🏗️ DDD领域架构
1. **组织管理领域** (Organization Domain)
2. **设备管理领域** (Device Domain)  
3. **数据采集领域** (Data Collection Domain)
4. **系统监控领域** (System Monitoring Domain)
5. **实时查询领域** (Real-time Query Domain)
6. **设备调试领域** (Device Debug Domain) - 包含数据库查询
7. **规则引擎领域** (Rule Engine Domain)
8. **协议管理领域** (Protocol Management Domain) - 协议插件化管理
9. **身份权限领域** (Identity & Authorization Domain)

## 📊 技术栈确认
- **监控频率**: 30秒采集间隔
- **告警阈值**: CPU 80% / 内存 85% / 磁盘 90%
- **实时数据刷新**: 1-5秒（可配置），无分页限制
- **调试权限**: 所有用户均可调试，包含数据库查询功能
- **数据保留策略**: 监控数据3个月，调试记录1年
- **告警方式**: 界面显示

## 📁 文档结构
- `00_project_overview.md` - 项目概览（本文档）
- `01_domain_design.md` - DDD领域设计（9个领域）
- `02_technical_architecture.md` - 技术架构设计
- `03_database_design.md` - 数据库设计
- `04_implementation_plan.md` - 实施计划（46项任务）
- `05_monitoring_requirements.md` - 监控需求规范
- `06_debug_requirements.md` - 调试需求规范
- `07_protocol_management_requirements.md` - 协议管理需求规范

## 🎯 当前状态
- **RESEARCH阶段**: ✅ 完成
- **INNOVATE阶段**: ✅ 完成  
- **PLAN阶段**: 🚧 进行中 - 文档重构和需求优化
- **EXECUTE阶段**: ⏳ 待开始 
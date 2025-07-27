# IoT数据采集平台 - Node-RED规则引擎

## 概述

Node-RED作为IoT数据采集平台的服务端规则引擎，用于处理设备数据汇聚、规则执行和告警管理。

## 服务配置

### 访问信息
- **Web界面**: http://localhost:1880
- **用户名**: admin  
- **密码**: iot123456

### 网络架构
```
设备端采集器 → HTTP/MQTT → Node-RED规则引擎 → MySQL数据库
                    ↓
               实时监控面板
```

## 功能特性

### 1. 数据接收与验证
- HTTP接口: `/api/device/data` 
- MQTT订阅: `iot/devices/+/data`
- 数据格式验证和转换
- 错误处理和响应

### 2. 规则引擎
- 动态规则加载（来自数据库）
- JavaScript规则执行
- 条件触发和告警
- 多设备数据汇聚

### 3. 数据存储
- MySQL连接池管理
- 批量数据插入
- 历史数据记录
- 性能优化

## 预装流程

### 主流程: IoT数据采集流程
- **接收设备数据**: HTTP POST接口
- **数据验证与转换**: 格式检查和数据清洗  
- **存储到MySQL**: 批量插入优化
- **响应处理**: 成功/错误状态返回

### 规则流程: 设备规则引擎  
- **MQTT设备数据**: 实时数据订阅
- **规则处理器**: 动态规则执行
- **数据存储**: 处理后数据持久化
- **告警处理**: 异常情况通知

## 开发指南

### 全局变量配置
```javascript
// MySQL连接配置
global.get('iotConfig').mysql = {
    host: 'iot-mysql',
    port: 3306, 
    user: 'root',
    password: 'iot123456',
    database: 'IoTDataCollection_Main'
};

// API配置
global.get('iotConfig').api = {
    baseUrl: 'http://host.docker.internal:44330',
    timeout: 30000
};
```

### 常用节点
- **HTTP In/Response**: REST API接口
- **Function**: JavaScript业务逻辑
- **MySQL**: 数据库操作
- **Debug**: 调试和日志输出
- **Inject**: 测试数据注入

### 数据库表映射
- `T_ENTERPRISES`: 企业信息
- `T_SITES`: 站点信息  
- `T_DEVICES`: 设备信息
- `T_DATA_POINTS`: 数据点配置
- `T_JAVASCRIPT_RULES`: JavaScript规则
- `T_NODERED_FLOWS`: Node-RED流程

## 部署和维护

### Docker容器管理
```bash
# 启动服务
docker-compose up -d

# 查看日志
docker logs iot-nodered -f

# 重启服务
docker-compose restart node-red

# 停止服务
docker-compose down
```

### 数据备份
```bash
# 备份Node-RED数据卷
docker run --rm -v rulesdemoproject_node_red_data:/data -v $(pwd):/backup busybox tar czf /backup/node_red_backup.tar.gz /data

# 恢复Node-RED数据卷  
docker run --rm -v rulesdemoproject_node_red_data:/data -v $(pwd):/backup busybox tar xzf /backup/node_red_backup.tar.gz
```

### 性能监控
- CPU使用率监控
- 内存使用情况
- 网络连接状态
- 消息处理延迟

## 集成说明

### 与ABP后端集成
- RESTful API调用
- 认证令牌传递
- 数据同步机制
- 错误处理统一

### 与前端集成
- WebSocket实时推送
- 状态数据API
- 告警信息广播
- 配置参数管理

## 安全考虑

- 用户认证和授权
- 数据传输加密
- 输入验证和过滤
- 日志记录和审计
- 网络隔离和防护

## 故障排除

### 常见问题
1. **连接失败**: 检查网络配置和防火墙设置
2. **数据库错误**: 验证MySQL连接参数和权限
3. **内存溢出**: 调整Node-RED内存限制
4. **性能问题**: 优化流程设计和数据库查询

### 日志分析
```bash
# 查看错误日志
docker logs iot-nodered --since 1h | grep ERROR

# 查看连接日志
docker logs iot-nodered --since 1h | grep mysql
```

---

**更新时间**: 2025-01-14  
**版本**: v1.0.0  
**维护团队**: IoT开发组 
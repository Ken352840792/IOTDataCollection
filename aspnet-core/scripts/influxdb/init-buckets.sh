#!/bin/bash

# InfluxDB初始化脚本 - 创建监控数据bucket
# 业务数据bucket (iot_data_business) 已由Docker Compose创建

echo "开始初始化InfluxDB监控数据bucket..."

# 等待InfluxDB服务启动
sleep 10

# 使用influx CLI创建监控数据bucket
echo "创建监控数据bucket: iot_data_monitoring"
influx bucket create \
  --name iot_data_monitoring \
  --org IOTDataCollection \
  --retention 7d \
  --description "IoT监控数据存储"

if [ $? -eq 0 ]; then
    echo "监控数据bucket创建成功"
else
    echo "监控数据bucket可能已存在或创建失败"
fi

echo "InfluxDB bucket初始化完成"
echo "- iot_data_business: 业务数据存储 (30天保留) - 已由Docker Compose创建"
echo "- iot_data_monitoring: 监控数据存储 (7天保留) - 已创建" 
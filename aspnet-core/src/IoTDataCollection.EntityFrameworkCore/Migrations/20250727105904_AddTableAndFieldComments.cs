using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDataCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddTableAndFieldComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "T_SITES",
                comment: "站点信息表 - 存储企业下属站点/工厂信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_NODERED_FLOWS",
                comment: "Node-RED流程表 - 存储服务端可视化数据处理流程")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_JAVASCRIPT_RULES",
                comment: "JavaScript规则表 - 存储采集端边缘计算规则脚本")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_ENTERPRISES",
                comment: "企业信息表 - 存储企业基本信息和联系方式")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_DEVICES",
                comment: "设备信息表 - 存储工业设备基本信息和连接参数")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_DATA_POINTS",
                comment: "数据点信息表 - 存储设备数据点定义和采集配置")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_COLLECTOR_NODES",
                comment: "采集节点表 - 存储数据采集端节点信息和状态")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_SITES",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_SITES",
                type: "int",
                nullable: false,
                comment: "排序号 - 用于显示排序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_TYPE",
                table: "T_SITES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "站点类型 - 工厂、车间、仓库等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_NAME",
                table: "T_SITES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "站点名称 - 站点全称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_CODE",
                table: "T_SITES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "站点编码 - 企业内唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SHORT_NAME",
                table: "T_SITES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "站点简称 - 站点简短名称",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_MANAGER",
                table: "T_SITES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "负责人 - 站点负责人姓名",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_ENTERPRISE_ID",
                table: "T_SITES",
                type: "char(36)",
                nullable: false,
                comment: "企业ID - 关联企业表外键",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_SITES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "站点描述 - 站点简介和说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PHONE",
                table: "T_SITES",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                comment: "联系电话 - 站点联系电话",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ADDRESS",
                table: "T_SITES",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "站点地址 - 站点详细地址",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_VERSION",
                table: "T_NODERED_FLOWS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                comment: "版本号 - 流程版本标识",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_TAB_ID",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "标签页ID - Node-RED标签页标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "F_MAX_PROCESSING_RATE",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                comment: "最大处理速率 - 每秒处理条数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_EXECUTION_TIME",
                table: "T_NODERED_FLOWS",
                type: "datetime(6)",
                nullable: true,
                comment: "最后执行时间 - 流程最后执行时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_DEPLOY_TIME",
                table: "T_NODERED_FLOWS",
                type: "datetime(6)",
                nullable: true,
                comment: "最后部署时间 - 流程最后部署时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_HOT_DEPLOY_ENABLED",
                table: "T_NODERED_FLOWS",
                type: "tinyint(1)",
                nullable: false,
                comment: "启用热部署 - 是否支持热部署",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_ENABLED",
                table: "T_NODERED_FLOWS",
                type: "tinyint(1)",
                nullable: false,
                comment: "是否启用 - 流程启用状态",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_TYPE",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "流程类型 - 数据汇集、数据转换、告警处理等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_NAME",
                table: "T_NODERED_FLOWS",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "流程名称 - 流程显示名称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CONFIG",
                table: "T_NODERED_FLOWS",
                type: "LONGTEXT",
                nullable: false,
                comment: "流程配置 - Node-RED流程JSON配置",
                oldClrType: typeof(string),
                oldType: "LONGTEXT")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CODE",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "流程编码 - 全局唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CATEGORY",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "流程分类 - 实时处理、批量处理、定时任务等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_EXECUTION_PRIORITY",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                comment: "执行优先级 - 1:高 2:中 3:低",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "F_EXECUTION_COUNT",
                table: "T_NODERED_FLOWS",
                type: "bigint",
                nullable: false,
                comment: "执行次数 - 流程执行统计",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "F_TRIGGER_CONDITION",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "触发条件 - 时间触发、数据变化触发等",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_JAVASCRIPT_RULES",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_SCRIPT_CODE",
                table: "T_JAVASCRIPT_RULES",
                type: "LONGTEXT",
                nullable: false,
                comment: "脚本代码 - JavaScript代码内容",
                oldClrType: typeof(string),
                oldType: "LONGTEXT")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_TYPE",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "规则类型 - 数据处理、告警、控制等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_NAME",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "规则名称 - 规则显示名称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_CODE",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "规则编码 - 全局唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_EXECUTION_TIME",
                table: "T_JAVASCRIPT_RULES",
                type: "datetime(6)",
                nullable: true,
                comment: "最后执行时间 - 规则最后执行时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_HOT_UPDATE_ENABLED",
                table: "T_JAVASCRIPT_RULES",
                type: "tinyint(1)",
                nullable: false,
                comment: "启用热更新 - 是否支持热更新",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<int>(
                name: "F_EXECUTION_PRIORITY",
                table: "T_JAVASCRIPT_RULES",
                type: "int",
                nullable: false,
                comment: "执行优先级 - 1:高 2:中 3:低",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "F_EXECUTION_COUNT",
                table: "T_JAVASCRIPT_RULES",
                type: "bigint",
                nullable: false,
                comment: "执行次数 - 规则执行统计",
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_DEVICE_ID",
                table: "T_JAVASCRIPT_RULES",
                type: "char(36)",
                nullable: true,
                comment: "设备ID - 关联设备表外键，为空表示全局规则",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "规则描述 - 规则详细说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_ENTERPRISES",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_ENTERPRISES",
                type: "int",
                nullable: false,
                comment: "排序号 - 用于显示排序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_SHORT_NAME",
                table: "T_ENTERPRISES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "企业简称 - 企业简短名称",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_ENABLED",
                table: "T_ENTERPRISES",
                type: "tinyint(1)",
                nullable: false,
                comment: "是否启用 - 业务启用标识",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_TYPE",
                table: "T_ENTERPRISES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "企业类型 - 制造业、服务业等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_NAME",
                table: "T_ENTERPRISES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "企业名称 - 企业全称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_CODE",
                table: "T_ENTERPRISES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "企业编码 - 全局唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_ENTERPRISES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "企业描述 - 企业简介和说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PHONE",
                table: "T_ENTERPRISES",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                comment: "联系电话 - 企业联系电话",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PERSON",
                table: "T_ENTERPRISES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                comment: "联系人 - 企业主要联系人姓名",
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ADDRESS",
                table: "T_ENTERPRISES",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "企业地址 - 企业详细地址",
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                comment: "排序号 - 用于显示排序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_SITE_ID",
                table: "T_DEVICES",
                type: "char(36)",
                nullable: false,
                comment: "站点ID - 关联站点表外键",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "F_PORT",
                table: "T_DEVICES",
                type: "int",
                nullable: true,
                comment: "端口号 - 设备通信端口",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_COLLECTION_ENABLED",
                table: "T_DEVICES",
                type: "tinyint(1)",
                nullable: false,
                comment: "启用采集 - 是否启用数据采集",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "F_IP_ADDRESS",
                table: "T_DEVICES",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                comment: "IP地址 - 设备网络地址",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_TYPE",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "设备类型 - PLC、Modbus、OPC-UA等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_NAME",
                table: "T_DEVICES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "设备名称 - 设备显示名称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_CODE",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "设备编码 - 站点内唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_DEVICES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "设备描述 - 设备详细说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_CONNECTION_STATUS",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                comment: "连接状态 - 在线、离线、异常",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_COMMUNICATION_PROTOCOL",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "通信协议 - Modbus、OPC-UA、TCP等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_COLLECTION_INTERVAL",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                comment: "采集间隔 - 数据采集间隔秒数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_UNIT",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                comment: "单位 - 数据值单位",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                comment: "状态 - 0:禁用 1:启用",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                comment: "排序号 - 用于显示排序",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "F_SCALE_FACTOR",
                table: "T_DATA_POINTS",
                type: "double",
                nullable: false,
                comment: "缩放比例 - 数据值缩放因子",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<string>(
                name: "F_REGISTER_TYPE",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                comment: "寄存器类型 - 保持寄存器、输入寄存器等",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_REGISTER_ADDRESS",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: true,
                comment: "寄存器地址 - Modbus寄存器地址",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "F_POINT_NAME",
                table: "T_DATA_POINTS",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "数据点名称 - 数据点显示名称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_POINT_CODE",
                table: "T_DATA_POINTS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "数据点编码 - 设备内唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "F_OFFSET",
                table: "T_DATA_POINTS",
                type: "double",
                nullable: false,
                comment: "偏移量 - 数据值偏移量",
                oldClrType: typeof(double),
                oldType: "double");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_COLLECTION_ENABLED",
                table: "T_DATA_POINTS",
                type: "tinyint(1)",
                nullable: false,
                comment: "启用采集 - 是否启用此数据点采集",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_DEVICE_ID",
                table: "T_DATA_POINTS",
                type: "char(36)",
                nullable: false,
                comment: "设备ID - 关联设备表外键",
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_DATA_POINTS",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "数据点描述 - 数据点详细说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DATA_TYPE",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                comment: "数据类型 - Int16、Int32、Float、Bool、String等",
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_DATA_LENGTH",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: true,
                comment: "数据长度 - 字节数或位数",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "F_COLLECTION_PRIORITY",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                comment: "采集优先级 - 1:高 2:中 3:低",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_TYPE",
                table: "T_COLLECTOR_NODES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                comment: "节点类型 - 采集节点、监控节点、边缘节点等",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_NODE_STATUS",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                comment: "节点状态 - 0:离线 1:在线 2:故障 3:维护中",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_NAME",
                table: "T_COLLECTOR_NODES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                comment: "节点名称 - 节点显示名称",
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_CODE",
                table: "T_COLLECTOR_NODES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                comment: "节点编码 - 全局唯一标识",
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_MEMORY_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                comment: "内存使用率 - 当前内存使用百分比",
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_HEARTBEAT_TIME",
                table: "T_COLLECTOR_NODES",
                type: "datetime(6)",
                nullable: true,
                comment: "最后心跳时间 - 节点最后心跳时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_MONITORING_ENABLED",
                table: "T_COLLECTOR_NODES",
                type: "tinyint(1)",
                nullable: false,
                comment: "启用监控 - 是否启用系统监控",
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<string>(
                name: "F_IP_ADDRESS",
                table: "T_COLLECTOR_NODES",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                comment: "IP地址 - 节点网络地址",
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_HEARTBEAT_INTERVAL",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                comment: "心跳间隔 - 心跳发送间隔秒数",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_DISK_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                comment: "磁盘使用率 - 当前磁盘使用百分比",
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_COLLECTOR_NODES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "节点描述 - 节点详细说明",
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_DATA_PORT",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                comment: "数据端口 - 节点数据通信端口",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_CPU_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                comment: "CPU使用率 - 当前CPU使用百分比",
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "F_CONNECTION_STATUS",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                comment: "连接状态 - 0:断开 1:已连接 2:连接中",
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterTable(
                name: "T_SITES",
                oldComment: "站点信息表 - 存储企业下属站点/工厂信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_NODERED_FLOWS",
                oldComment: "Node-RED流程表 - 存储服务端可视化数据处理流程")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_JAVASCRIPT_RULES",
                oldComment: "JavaScript规则表 - 存储采集端边缘计算规则脚本")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_ENTERPRISES",
                oldComment: "企业信息表 - 存储企业基本信息和联系方式")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_DEVICES",
                oldComment: "设备信息表 - 存储工业设备基本信息和连接参数")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_DATA_POINTS",
                oldComment: "数据点信息表 - 存储设备数据点定义和采集配置")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterTable(
                name: "T_COLLECTOR_NODES",
                oldComment: "采集节点表 - 存储数据采集端节点信息和状态")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_SITES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_SITES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序号 - 用于显示排序");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_TYPE",
                table: "T_SITES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "站点类型 - 工厂、车间、仓库等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_NAME",
                table: "T_SITES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "站点名称 - 站点全称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SITE_CODE",
                table: "T_SITES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "站点编码 - 企业内唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_SHORT_NAME",
                table: "T_SITES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "站点简称 - 站点简短名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_MANAGER",
                table: "T_SITES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "负责人 - 站点负责人姓名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_ENTERPRISE_ID",
                table: "T_SITES",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "企业ID - 关联企业表外键")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_SITES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "站点描述 - 站点简介和说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PHONE",
                table: "T_SITES",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldComment: "联系电话 - 站点联系电话")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ADDRESS",
                table: "T_SITES",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "站点地址 - 站点详细地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_VERSION",
                table: "T_NODERED_FLOWS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldComment: "版本号 - 流程版本标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_TAB_ID",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "标签页ID - Node-RED标签页标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<int>(
                name: "F_MAX_PROCESSING_RATE",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "最大处理速率 - 每秒处理条数");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_EXECUTION_TIME",
                table: "T_NODERED_FLOWS",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "最后执行时间 - 流程最后执行时间");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_DEPLOY_TIME",
                table: "T_NODERED_FLOWS",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "最后部署时间 - 流程最后部署时间");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_HOT_DEPLOY_ENABLED",
                table: "T_NODERED_FLOWS",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "启用热部署 - 是否支持热部署");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_ENABLED",
                table: "T_NODERED_FLOWS",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "是否启用 - 流程启用状态");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_TYPE",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "流程类型 - 数据汇集、数据转换、告警处理等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_NAME",
                table: "T_NODERED_FLOWS",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "流程名称 - 流程显示名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CONFIG",
                table: "T_NODERED_FLOWS",
                type: "LONGTEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldComment: "流程配置 - Node-RED流程JSON配置")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CODE",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "流程编码 - 全局唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_FLOW_CATEGORY",
                table: "T_NODERED_FLOWS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "流程分类 - 实时处理、批量处理、定时任务等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_EXECUTION_PRIORITY",
                table: "T_NODERED_FLOWS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "执行优先级 - 1:高 2:中 3:低");

            migrationBuilder.AlterColumn<long>(
                name: "F_EXECUTION_COUNT",
                table: "T_NODERED_FLOWS",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "执行次数 - 流程执行统计");

            migrationBuilder.AlterColumn<string>(
                name: "F_TRIGGER_CONDITION",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "触发条件 - 时间触发、数据变化触发等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_JAVASCRIPT_RULES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<string>(
                name: "F_SCRIPT_CODE",
                table: "T_JAVASCRIPT_RULES",
                type: "LONGTEXT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "LONGTEXT",
                oldComment: "脚本代码 - JavaScript代码内容")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_TYPE",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "规则类型 - 数据处理、告警、控制等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_NAME",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "规则名称 - 规则显示名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_RULE_CODE",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "规则编码 - 全局唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_EXECUTION_TIME",
                table: "T_JAVASCRIPT_RULES",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "最后执行时间 - 规则最后执行时间");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_HOT_UPDATE_ENABLED",
                table: "T_JAVASCRIPT_RULES",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "启用热更新 - 是否支持热更新");

            migrationBuilder.AlterColumn<int>(
                name: "F_EXECUTION_PRIORITY",
                table: "T_JAVASCRIPT_RULES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "执行优先级 - 1:高 2:中 3:低");

            migrationBuilder.AlterColumn<long>(
                name: "F_EXECUTION_COUNT",
                table: "T_JAVASCRIPT_RULES",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldComment: "执行次数 - 规则执行统计");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_DEVICE_ID",
                table: "T_JAVASCRIPT_RULES",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true,
                oldComment: "设备ID - 关联设备表外键，为空表示全局规则")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_JAVASCRIPT_RULES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "规则描述 - 规则详细说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_ENTERPRISES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_ENTERPRISES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序号 - 用于显示排序");

            migrationBuilder.AlterColumn<string>(
                name: "F_SHORT_NAME",
                table: "T_ENTERPRISES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "企业简称 - 企业简短名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_ENABLED",
                table: "T_ENTERPRISES",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "是否启用 - 业务启用标识");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_TYPE",
                table: "T_ENTERPRISES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "企业类型 - 制造业、服务业等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_NAME",
                table: "T_ENTERPRISES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "企业名称 - 企业全称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ENTERPRISE_CODE",
                table: "T_ENTERPRISES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "企业编码 - 全局唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_ENTERPRISES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "企业描述 - 企业简介和说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PHONE",
                table: "T_ENTERPRISES",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldComment: "联系电话 - 企业联系电话")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_CONTACT_PERSON",
                table: "T_ENTERPRISES",
                type: "varchar(128)",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(128)",
                oldMaxLength: 128,
                oldNullable: true,
                oldComment: "联系人 - 企业主要联系人姓名")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_ADDRESS",
                table: "T_ENTERPRISES",
                type: "varchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512,
                oldNullable: true,
                oldComment: "企业地址 - 企业详细地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序号 - 用于显示排序");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_SITE_ID",
                table: "T_DEVICES",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "站点ID - 关联站点表外键")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<int>(
                name: "F_PORT",
                table: "T_DEVICES",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "端口号 - 设备通信端口");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_COLLECTION_ENABLED",
                table: "T_DEVICES",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "启用采集 - 是否启用数据采集");

            migrationBuilder.AlterColumn<string>(
                name: "F_IP_ADDRESS",
                table: "T_DEVICES",
                type: "varchar(45)",
                maxLength: 45,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldNullable: true,
                oldComment: "IP地址 - 设备网络地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_TYPE",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "设备类型 - PLC、Modbus、OPC-UA等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_NAME",
                table: "T_DEVICES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "设备名称 - 设备显示名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DEVICE_CODE",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "设备编码 - 站点内唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_DEVICES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "设备描述 - 设备详细说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_CONNECTION_STATUS",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "连接状态 - 在线、离线、异常");

            migrationBuilder.AlterColumn<string>(
                name: "F_COMMUNICATION_PROTOCOL",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "通信协议 - Modbus、OPC-UA、TCP等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_COLLECTION_INTERVAL",
                table: "T_DEVICES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "采集间隔 - 数据采集间隔秒数");

            migrationBuilder.AlterColumn<string>(
                name: "F_UNIT",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldComment: "单位 - 数据值单位")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_STATUS",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "状态 - 0:禁用 1:启用");

            migrationBuilder.AlterColumn<int>(
                name: "F_SORT_ORDER",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "排序号 - 用于显示排序");

            migrationBuilder.AlterColumn<double>(
                name: "F_SCALE_FACTOR",
                table: "T_DATA_POINTS",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "缩放比例 - 数据值缩放因子");

            migrationBuilder.AlterColumn<string>(
                name: "F_REGISTER_TYPE",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldNullable: true,
                oldComment: "寄存器类型 - 保持寄存器、输入寄存器等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_REGISTER_ADDRESS",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "寄存器地址 - Modbus寄存器地址");

            migrationBuilder.AlterColumn<string>(
                name: "F_POINT_NAME",
                table: "T_DATA_POINTS",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "数据点名称 - 数据点显示名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_POINT_CODE",
                table: "T_DATA_POINTS",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "数据点编码 - 设备内唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<double>(
                name: "F_OFFSET",
                table: "T_DATA_POINTS",
                type: "double",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double",
                oldComment: "偏移量 - 数据值偏移量");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_COLLECTION_ENABLED",
                table: "T_DATA_POINTS",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "启用采集 - 是否启用此数据点采集");

            migrationBuilder.AlterColumn<Guid>(
                name: "F_DEVICE_ID",
                table: "T_DATA_POINTS",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldComment: "设备ID - 关联设备表外键")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_DATA_POINTS",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "数据点描述 - 数据点详细说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_DATA_TYPE",
                table: "T_DATA_POINTS",
                type: "varchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(32)",
                oldMaxLength: 32,
                oldComment: "数据类型 - Int16、Int32、Float、Bool、String等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_DATA_LENGTH",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "数据长度 - 字节数或位数");

            migrationBuilder.AlterColumn<int>(
                name: "F_COLLECTION_PRIORITY",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "采集优先级 - 1:高 2:中 3:低");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_TYPE",
                table: "T_COLLECTOR_NODES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldNullable: true,
                oldComment: "节点类型 - 采集节点、监控节点、边缘节点等")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_NODE_STATUS",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "节点状态 - 0:离线 1:在线 2:故障 3:维护中");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_NAME",
                table: "T_COLLECTOR_NODES",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256,
                oldComment: "节点名称 - 节点显示名称")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "F_NODE_CODE",
                table: "T_COLLECTOR_NODES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64,
                oldComment: "节点编码 - 全局唯一标识")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_MEMORY_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true,
                oldComment: "内存使用率 - 当前内存使用百分比");

            migrationBuilder.AlterColumn<DateTime>(
                name: "F_LAST_HEARTBEAT_TIME",
                table: "T_COLLECTOR_NODES",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true,
                oldComment: "最后心跳时间 - 节点最后心跳时间");

            migrationBuilder.AlterColumn<bool>(
                name: "F_IS_MONITORING_ENABLED",
                table: "T_COLLECTOR_NODES",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldComment: "启用监控 - 是否启用系统监控");

            migrationBuilder.AlterColumn<string>(
                name: "F_IP_ADDRESS",
                table: "T_COLLECTOR_NODES",
                type: "varchar(45)",
                maxLength: 45,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(45)",
                oldMaxLength: 45,
                oldComment: "IP地址 - 节点网络地址")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_HEARTBEAT_INTERVAL",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "心跳间隔 - 心跳发送间隔秒数");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_DISK_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true,
                oldComment: "磁盘使用率 - 当前磁盘使用百分比");

            migrationBuilder.AlterColumn<string>(
                name: "F_DESCRIPTION",
                table: "T_COLLECTOR_NODES",
                type: "varchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "节点描述 - 节点详细说明")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "F_DATA_PORT",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "数据端口 - 节点数据通信端口");

            migrationBuilder.AlterColumn<decimal>(
                name: "F_CPU_USAGE",
                table: "T_COLLECTOR_NODES",
                type: "decimal(65,30)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(65,30)",
                oldNullable: true,
                oldComment: "CPU使用率 - 当前CPU使用百分比");

            migrationBuilder.AlterColumn<int>(
                name: "F_CONNECTION_STATUS",
                table: "T_COLLECTOR_NODES",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "连接状态 - 0:断开 1:已连接 2:连接中");
        }
    }
}

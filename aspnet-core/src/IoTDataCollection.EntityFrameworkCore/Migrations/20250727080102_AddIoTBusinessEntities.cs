using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDataCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddIoTBusinessEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "T_DATA_POINTS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_DEVICE_ID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_POINT_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_POINT_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DATA_TYPE = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_REGISTER_ADDRESS = table.Column<int>(type: "int", nullable: true),
                    F_REGISTER_TYPE = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DATA_LENGTH = table.Column<int>(type: "int", nullable: true),
                    F_IS_SIGNED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_BYTE_ORDER = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SCALE_FACTOR = table.Column<double>(type: "double", nullable: false),
                    F_OFFSET = table.Column<double>(type: "double", nullable: false),
                    F_UNIT = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_MIN_VALUE = table.Column<double>(type: "double", nullable: true),
                    F_MAX_VALUE = table.Column<double>(type: "double", nullable: true),
                    F_DEFAULT_VALUE = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IS_READ_ONLY = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_IS_COLLECTION_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_COLLECTION_PRIORITY = table.Column<int>(type: "int", nullable: false),
                    F_LAST_COLLECTION_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_LAST_VALUE = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DATA_POINTS", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_DEVICES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_SITE_ID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_DEVICE_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DEVICE_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DEVICE_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DEVICE_MODEL = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_MANUFACTURER = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_COMMUNICATION_PROTOCOL = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IP_ADDRESS = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_PORT = table.Column<int>(type: "int", nullable: true),
                    F_SLAVE_ADDRESS = table.Column<int>(type: "int", nullable: true),
                    F_COLLECTION_INTERVAL = table.Column<int>(type: "int", nullable: false),
                    F_TIMEOUT = table.Column<int>(type: "int", nullable: false),
                    F_RETRY_COUNT = table.Column<int>(type: "int", nullable: false),
                    F_LOCATION = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_CONNECTION_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_LAST_COMMUNICATION_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_IS_COLLECTION_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_DEVICES", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_ENTERPRISES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_ENTERPRISE_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_ENTERPRISE_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SHORT_NAME = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_ENTERPRISE_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_CONTACT_PERSON = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_CONTACT_PHONE = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_ADDRESS = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_ENTERPRISES", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_JAVASCRIPT_RULES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_DEVICE_ID = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    F_RULE_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_RULE_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_RULE_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_TRIGGER_CONDITION = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SCRIPT_CODE = table.Column<string>(type: "LONGTEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_VERSION = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXECUTION_PRIORITY = table.Column<int>(type: "int", nullable: false),
                    F_TIMEOUT = table.Column<int>(type: "int", nullable: false),
                    F_MEMORY_LIMIT = table.Column<long>(type: "bigint", nullable: false),
                    F_IS_HOT_UPDATE_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_LAST_EXECUTION_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_EXECUTION_COUNT = table.Column<long>(type: "bigint", nullable: false),
                    F_LAST_EXECUTION_RESULT = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_LAST_ERROR_MESSAGE = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IS_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_JAVASCRIPT_RULES", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_NODERED_FLOWS",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_FLOW_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_FLOW_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_FLOW_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_FLOW_CATEGORY = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_FLOW_CONFIG = table.Column<string>(type: "LONGTEXT", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_VERSION = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_TAB_ID = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXECUTION_PRIORITY = table.Column<int>(type: "int", nullable: false),
                    F_MAX_PROCESSING_RATE = table.Column<int>(type: "int", nullable: false),
                    F_IS_HOT_DEPLOY_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_LAST_DEPLOY_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_LAST_EXECUTION_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_EXECUTION_COUNT = table.Column<long>(type: "bigint", nullable: false),
                    F_PROCESSED_MESSAGE_COUNT = table.Column<long>(type: "bigint", nullable: false),
                    F_ERROR_MESSAGE_COUNT = table.Column<long>(type: "bigint", nullable: false),
                    F_LAST_DEPLOY_STATUS = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_LAST_ERROR_MESSAGE = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_AUTHOR = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_RELATED_DEVICES = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_INPUT_TOPICS = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_OUTPUT_TOPICS = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IS_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_NODERED_FLOWS", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "T_SITES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_ENTERPRISE_ID = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_SITE_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SITE_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SHORT_NAME = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SITE_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_ADDRESS = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_MANAGER = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_CONTACT_PHONE = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_SORT_ORDER = table.Column<int>(type: "int", nullable: false),
                    F_EXP_01 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_02 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_03 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_04 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_05 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_06 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_07 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_08 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_09 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_EXP_10 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExtraProperties = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConcurrencyStamp = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatorId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    LastModificationTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci"),
                    DeletionTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_T_SITES", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_T_DATA_POINTS_F_COLLECTION_PRIORITY",
                table: "T_DATA_POINTS",
                column: "F_COLLECTION_PRIORITY");

            migrationBuilder.CreateIndex(
                name: "IX_T_DATA_POINTS_F_DEVICE_ID_F_POINT_CODE",
                table: "T_DATA_POINTS",
                columns: new[] { "F_DEVICE_ID", "F_POINT_CODE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_DATA_POINTS_F_IS_COLLECTION_ENABLED",
                table: "T_DATA_POINTS",
                column: "F_IS_COLLECTION_ENABLED");

            migrationBuilder.CreateIndex(
                name: "IX_T_DATA_POINTS_F_POINT_NAME",
                table: "T_DATA_POINTS",
                column: "F_POINT_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_DATA_POINTS_F_STATUS",
                table: "T_DATA_POINTS",
                column: "F_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_DEVICES_F_CONNECTION_STATUS",
                table: "T_DEVICES",
                column: "F_CONNECTION_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_DEVICES_F_DEVICE_NAME",
                table: "T_DEVICES",
                column: "F_DEVICE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_DEVICES_F_IS_COLLECTION_ENABLED",
                table: "T_DEVICES",
                column: "F_IS_COLLECTION_ENABLED");

            migrationBuilder.CreateIndex(
                name: "IX_T_DEVICES_F_SITE_ID_F_DEVICE_CODE",
                table: "T_DEVICES",
                columns: new[] { "F_SITE_ID", "F_DEVICE_CODE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_DEVICES_F_STATUS",
                table: "T_DEVICES",
                column: "F_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_ENTERPRISES_F_ENTERPRISE_CODE",
                table: "T_ENTERPRISES",
                column: "F_ENTERPRISE_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_ENTERPRISES_F_ENTERPRISE_NAME",
                table: "T_ENTERPRISES",
                column: "F_ENTERPRISE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_ENTERPRISES_F_STATUS",
                table: "T_ENTERPRISES",
                column: "F_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_JAVASCRIPT_RULES_F_DEVICE_ID",
                table: "T_JAVASCRIPT_RULES",
                column: "F_DEVICE_ID");

            migrationBuilder.CreateIndex(
                name: "IX_T_JAVASCRIPT_RULES_F_IS_ENABLED",
                table: "T_JAVASCRIPT_RULES",
                column: "F_IS_ENABLED");

            migrationBuilder.CreateIndex(
                name: "IX_T_JAVASCRIPT_RULES_F_RULE_CODE",
                table: "T_JAVASCRIPT_RULES",
                column: "F_RULE_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_JAVASCRIPT_RULES_F_RULE_NAME",
                table: "T_JAVASCRIPT_RULES",
                column: "F_RULE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_JAVASCRIPT_RULES_F_STATUS",
                table: "T_JAVASCRIPT_RULES",
                column: "F_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_NODERED_FLOWS_F_FLOW_CODE",
                table: "T_NODERED_FLOWS",
                column: "F_FLOW_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_NODERED_FLOWS_F_FLOW_NAME",
                table: "T_NODERED_FLOWS",
                column: "F_FLOW_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_NODERED_FLOWS_F_FLOW_TYPE",
                table: "T_NODERED_FLOWS",
                column: "F_FLOW_TYPE");

            migrationBuilder.CreateIndex(
                name: "IX_T_NODERED_FLOWS_F_IS_ENABLED",
                table: "T_NODERED_FLOWS",
                column: "F_IS_ENABLED");

            migrationBuilder.CreateIndex(
                name: "IX_T_NODERED_FLOWS_F_STATUS",
                table: "T_NODERED_FLOWS",
                column: "F_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_SITES_F_ENTERPRISE_ID_F_SITE_CODE",
                table: "T_SITES",
                columns: new[] { "F_ENTERPRISE_ID", "F_SITE_CODE" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_SITES_F_SITE_NAME",
                table: "T_SITES",
                column: "F_SITE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_SITES_F_STATUS",
                table: "T_SITES",
                column: "F_STATUS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_DATA_POINTS");

            migrationBuilder.DropTable(
                name: "T_DEVICES");

            migrationBuilder.DropTable(
                name: "T_ENTERPRISES");

            migrationBuilder.DropTable(
                name: "T_JAVASCRIPT_RULES");

            migrationBuilder.DropTable(
                name: "T_NODERED_FLOWS");

            migrationBuilder.DropTable(
                name: "T_SITES");
        }
    }
}

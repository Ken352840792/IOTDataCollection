using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDataCollection.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityFieldsStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "F_IS_ENABLED",
                table: "T_ENTERPRISES",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "T_COLLECTOR_NODES",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    F_NODE_CODE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_NODE_NAME = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_DESCRIPTION = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_NODE_TYPE = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IP_ADDRESS = table.Column<string>(type: "varchar(45)", maxLength: 45, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_MANAGEMENT_PORT = table.Column<int>(type: "int", nullable: false),
                    F_DATA_PORT = table.Column<int>(type: "int", nullable: false),
                    F_OPERATING_SYSTEM = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_SOFTWARE_VERSION = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_HARDWARE_INFO = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_NODE_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_CONNECTION_STATUS = table.Column<int>(type: "int", nullable: false),
                    F_LAST_HEARTBEAT_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_LAST_OFFLINE_TIME = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    F_HEARTBEAT_INTERVAL = table.Column<int>(type: "int", nullable: false),
                    F_TIMEOUT_THRESHOLD = table.Column<int>(type: "int", nullable: false),
                    F_CPU_USAGE = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    F_MEMORY_USAGE = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    F_DISK_USAGE = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    F_NETWORK_TRAFFIC = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    F_DEVICE_COUNT = table.Column<int>(type: "int", nullable: false),
                    F_ONLINE_DEVICE_COUNT = table.Column<int>(type: "int", nullable: false),
                    F_DATA_COLLECTION_RATE = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    F_ERROR_DATA_COUNT = table.Column<int>(type: "int", nullable: false),
                    F_LOCATION = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_RESPONSIBLE_PERSON = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_CONTACT_PHONE = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    F_IS_MONITORING_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    F_IS_ALERT_ENABLED = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_T_COLLECTOR_NODES", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_CONNECTION_STATUS",
                table: "T_COLLECTOR_NODES",
                column: "F_CONNECTION_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_IP_ADDRESS",
                table: "T_COLLECTOR_NODES",
                column: "F_IP_ADDRESS",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_IS_MONITORING_ENABLED",
                table: "T_COLLECTOR_NODES",
                column: "F_IS_MONITORING_ENABLED");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_LAST_HEARTBEAT_TIME",
                table: "T_COLLECTOR_NODES",
                column: "F_LAST_HEARTBEAT_TIME");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_NODE_CODE",
                table: "T_COLLECTOR_NODES",
                column: "F_NODE_CODE",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_NODE_NAME",
                table: "T_COLLECTOR_NODES",
                column: "F_NODE_NAME");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_NODE_STATUS",
                table: "T_COLLECTOR_NODES",
                column: "F_NODE_STATUS");

            migrationBuilder.CreateIndex(
                name: "IX_T_COLLECTOR_NODES_F_NODE_TYPE",
                table: "T_COLLECTOR_NODES",
                column: "F_NODE_TYPE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "T_COLLECTOR_NODES");

            migrationBuilder.DropColumn(
                name: "F_IS_ENABLED",
                table: "T_ENTERPRISES");
        }
    }
}

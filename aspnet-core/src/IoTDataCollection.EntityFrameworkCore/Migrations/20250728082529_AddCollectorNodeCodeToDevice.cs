using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDataCollection.Migrations
{
    /// <inheritdoc />
    public partial class AddCollectorNodeCodeToDevice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "F_COLLECTOR_NODE_CODE",
                table: "T_DEVICES",
                type: "varchar(64)",
                maxLength: 64,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "F_COLLECTOR_NODE_CODE",
                table: "T_DEVICES");
        }
    }
}

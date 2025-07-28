using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IoTDataCollection.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRegisterAddressToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "F_REGISTER_ADDRESS",
                table: "T_DATA_POINTS",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "寄存器地址 - 支持各种协议的地址格式，如Modbus的\"40001\"、Siemens S7的\"DB1.DBD0\"等",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "寄存器地址 - Modbus寄存器地址")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "F_REGISTER_ADDRESS",
                table: "T_DATA_POINTS",
                type: "int",
                nullable: true,
                comment: "寄存器地址 - Modbus寄存器地址",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "寄存器地址 - 支持各种协议的地址格式，如Modbus的\"40001\"、Siemens S7的\"DB1.DBD0\"等")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}

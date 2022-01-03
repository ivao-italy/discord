using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class NullableOtp : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Otp",
            table: "Users",
            type: "varchar(6)",
            maxLength: 6,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(6)",
            oldMaxLength: 6)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Users",
            keyColumn: "Otp",
            keyValue: null,
            column: "Otp",
            value: "");

        migrationBuilder.AlterColumn<string>(
            name: "Otp",
            table: "Users",
            type: "varchar(6)",
            maxLength: 6,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "varchar(6)",
            oldMaxLength: 6,
            oldNullable: true)
            .Annotation("MySql:CharSet", "utf8mb4")
            .OldAnnotation("MySql:CharSet", "utf8mb4");
    }
}

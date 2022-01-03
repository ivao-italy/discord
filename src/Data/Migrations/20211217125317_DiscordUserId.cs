using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class DiscordUserId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Otp",
            table: "Users");

        migrationBuilder.AddColumn<ulong>(
            name: "DiscordUserId",
            table: "Users",
            type: "bigint unsigned",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "DiscordUserId",
            table: "Users");

        migrationBuilder.AddColumn<string>(
            name: "Otp",
            table: "Users",
            type: "varchar(6)",
            maxLength: 6,
            nullable: true)
            .Annotation("MySql:CharSet", "utf8mb4");
    }
}

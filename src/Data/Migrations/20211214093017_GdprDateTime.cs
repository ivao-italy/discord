using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class GdprDateTime : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AcceptsGdpr",
            table: "Users");

        migrationBuilder.AddColumn<DateTime>(
            name: "GdprAcceptDate",
            table: "Users",
            type: "datetime(6)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GdprAcceptDate",
            table: "Users");

        migrationBuilder.AddColumn<bool>(
            name: "AcceptsGdpr",
            table: "Users",
            type: "tinyint(1)",
            nullable: false,
            defaultValue: false);
    }
}

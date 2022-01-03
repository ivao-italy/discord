using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class RenameVidColumn : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "IvaoVid",
            table: "Users",
            newName: "Vid");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Vid",
            table: "Users",
            newName: "IvaoVid");
    }
}

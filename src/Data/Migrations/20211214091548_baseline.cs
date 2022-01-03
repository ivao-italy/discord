using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class baseline : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new {
                IvaoVid = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                FirstName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                LastName = table.Column<string>(type: "varchar(70)", maxLength: 70, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4"),
                AcceptsGdpr = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                Otp = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false)
                    .Annotation("MySql:CharSet", "utf8mb4")
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.IvaoVid);
            })
            .Annotation("MySql:CharSet", "utf8mb4");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Users");
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations
{
    public partial class EventTask_UniqueIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventTasks_EventId_TaskTypeId",
                table: "EventTasks",
                columns: new[] { "EventId", "TaskTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_EventTasks_EventId_TaskTypeId",
                table: "EventTasks");
        }
    }
}

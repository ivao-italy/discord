using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations
{
    public partial class TrainingView_FixWhere : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER ALGORITHM = UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `trainingslist` AS " +
                "SELECT id, planned_date, planned_hour, airport, facility, rating_training AS rating " +
                "FROM trainingNEW.trainings " +
                "WHERE status = 3 AND approved = 1 " +
                "AND planned_date >= CAST(NOW() AS DATE)"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "ALTER ALGORITHM = UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `trainingslist` AS " +
                "SELECT id, planned_date, planned_hour, airport, facility, rating_training AS rating " +
                "FROM trainingNEW.trainings " +
                "WHERE `type` = 1 " +
                "AND planned_date >= CAST(NOW() AS DATE)"
            );
        }
    }
}

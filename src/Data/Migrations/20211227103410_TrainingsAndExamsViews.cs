using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations;

public partial class TrainingsAndExamsViews : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "CREATE ALGORITHM = UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `examlist` AS " +
            "SELECT id, planned_date, planned_hour, airport, facility, rating " +
            "FROM exam.examlist " +
            "WHERE `type` = 1 " +
            "AND planned_date >= CAST(NOW() AS DATE)"
            );

        migrationBuilder.Sql(
            "CREATE ALGORITHM = UNDEFINED DEFINER=`root`@`localhost` SQL SECURITY DEFINER VIEW `trainingslist` AS " +
            "SELECT id, planned_date, planned_hour, airport, facility, rating_training AS rating " +
            "FROM trainingNEW.trainings " +
            "WHERE `type` = 1 " +
            "AND planned_date >= CAST(NOW() AS DATE)"
            );
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP VIEW `examlist`;");

        migrationBuilder.Sql("DROP VIEW `trainingslist`;");
    }
}

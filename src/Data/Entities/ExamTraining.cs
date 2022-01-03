namespace Ivao.It.DiscordBot.Data.Entities;

public abstract class BaseExamTraining
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public int Id { get; set; }
    public string Airport { get; set; }
    public string Facility { get; set; }
    public int Rating { get; }
    public DateOnly PlannedDate { get; set; }
    public TimeOnly PlannedTime { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}    

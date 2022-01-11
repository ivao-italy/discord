namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal interface IScheduledTask
{
    TimeSpan StartDelay { get; init; }
    TimeSpan Period { get; init; }
    public bool IsRunning { get; }

    void Run();
    Task Stop();
}

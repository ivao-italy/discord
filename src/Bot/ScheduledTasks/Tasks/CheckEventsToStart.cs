namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal class CheckEventsToStart : IScheduledTask, IDisposable
{
    private Timer _timer;

    public TimeSpan StartDelay { get; init; }
    public TimeSpan Period { get; init; }

    public bool IsRunning  => _timer != null;

    public void Run(IvaoItBot bot) 
        =>  _timer = new Timer(async _ => await bot.CheckEventsToStart(), null, StartDelay, Period);

    public async Task Stop() => await _timer.DisposeAsync();
    public void Dispose() => _timer.Dispose();
}
using Ivao.It.DiscordBot.ScheduledTasks.Tasks;

namespace Ivao.It.DiscordBot.ScheduledTasks;

public class IvaoItBotTasks
{
    private static IvaoItBot? _bot;
    private static List<IScheduledTask>? _tasks;
    private static bool _areTaskDisposed;

    public int RunningTasks = _tasks?.Count ?? 0;

    internal IvaoItBotTasks(IvaoItBot botInstance)
    {
        _bot = botInstance;
        Init();
    }

    public void Init()
    {
        if (_bot is null) throw new InvalidOperationException("Bot instance is null");

        _tasks = new List<IScheduledTask>
        {
#if DEBUG
            //new CheckEventsToStart(_bot) { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromSeconds(30) },
            //new DeletePastEventsPost(_bot) { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromSeconds(30) }4
            new CheckCancelledEvents(_bot) { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromHours(1) }
#else
            new CheckEventsToStart(_bot) { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromMinutes(5) }, //TODO Controlla quando lanciarlo per schedularo sui 15'
            new DeletePastEventsPost(_bot) { StartDelay = TimeSpan.FromSeconds(30), Period = TimeSpan.FromHours(1) }
#endif
        };
    }

    public void Run()
    {
        if (_areTaskDisposed) throw new InvalidOperationException("Scheduled Tasks disposed. Rerun Init()");
        if (_tasks is null) throw new NullReferenceException("Tasks collection null");

        foreach (var item in _tasks)
        {
            if (item.IsRunning) continue;
            item.Run();
        }
    }

    public void Stop()
    {
        if (_tasks is null) return;

        foreach (var item in _tasks)
        {
            item.Stop();
        }
        _areTaskDisposed = true;
    }
}

using Ivao.It.DiscordBot.ScheduledTasks.Tasks;

namespace Ivao.It.DiscordBot.ScheduledTasks;

public class IvaoItBotTasks
{
    private static IvaoItBot _bot;
    private static List<IScheduledTask> _tasks;
    private static bool _areTaskDisposed;

    public int RunningTasks = _tasks?.Count ?? 0;

    internal IvaoItBotTasks(IvaoItBot botInstance)
    {
        _bot = botInstance;
        Init();
    }

    public void Init()
    {
        _tasks = new List<IScheduledTask>
        {
#if DEBUG
            new CheckEventsToStart { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromSeconds(30) },
            new DeletePastEventsPost { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromSeconds(30) }
#else
            new CheckEventsToStart { StartDelay = TimeSpan.FromSeconds(5), Period = TimeSpan.FromMinutes(5) }, //TODO Controlla quando lanciarlo per schedularo sui 15'
            new DeletePastEventsPost { StartDelay = TimeSpan.FromSeconds(30), Period = TimeSpan.FromHours(1) }
#endif
        };
    }

    public void Run()
    {
        if (_areTaskDisposed) throw new InvalidOperationException("Scheduled Tasks disposed. Rerun Init()");

        foreach (var item in _tasks)
        {
            if (item.IsRunning) continue;
            item.Run(_bot);
        }
    }

    public void Stop()
    {
        foreach (var item in _tasks)
        {
            item.Stop();
        }
        _areTaskDisposed = true;
    }
}

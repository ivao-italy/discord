using DSharpPlus;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal abstract class BaseScheduledTask : IScheduledTask, IDisposable
{
    protected Timer? Timer;

    protected IvaoItBot Bot { get; }
    public TimeSpan StartDelay { get; init; }
    public TimeSpan Period { get; init; }
    public bool IsRunning => this.Timer != null;

    protected BaseScheduledTask(IvaoItBot bot)
    {
        Bot = bot;
    }


    public virtual void Run() 
        => this.Timer = new Timer(async _ => await this.DoTaskAsync(), null, this.StartDelay, this.Period);

    public async Task Stop() => await (this.IsRunning ? this.Timer!.DisposeAsync() : ValueTask.CompletedTask);

    public void Dispose() => this.Timer?.Dispose();

    /// <summary>
    /// Encapsulates task login to run on the Client instance
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    protected abstract Task DoTaskAsync();
}

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal abstract class BaseScheduledTask : IScheduledTask, IDisposable
{
    protected Timer? Timer;

    public TimeSpan StartDelay { get; init; }
    public TimeSpan Period { get; init; }
    public bool IsRunning => this.Timer != null;

    public abstract void Run(IvaoItBot bot);

    public async Task Stop() => await (this.IsRunning ? this.Timer!.DisposeAsync() : ValueTask.CompletedTask);

    public void Dispose() => this.Timer?.Dispose();
}

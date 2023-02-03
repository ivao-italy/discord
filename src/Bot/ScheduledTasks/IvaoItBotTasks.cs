using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.Impl;

namespace Ivao.It.DiscordBot.ScheduledTasks;

/// <summary>
/// Quartz wrapper for IVAO IT Bot scheduled jobs
/// </summary>
internal class IvaoItBotTasks
{
    private readonly IvaoItBot? _bot;
    private readonly IHostEnvironment _environment;

    private IScheduler? _scheduler;

    internal IvaoItBotTasks(IvaoItBot botInstance, IHostEnvironment environment)
    {
        _bot = botInstance;
        _environment = environment;
    }

    /// <summary>
    /// Runs the Jobs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        if (_bot is null) throw new InvalidOperationException("Bot instance is null");
        if (_environment is null) throw new InvalidOperationException("Environment instance is null");

        //Configuring the scheduler (with the Bot injected in the context)
        _scheduler = await (new StdSchedulerFactory()).GetScheduler();
        await _scheduler.StartDelayed(TimeSpan.FromSeconds(5), cancellationToken);
        _scheduler.Context.Add("Bot", _bot);

        //Adding the Jobs schedules
#if DEBUG
        //await _scheduler.AddCheckEventsToStartJobAsync(_environment);
        await _scheduler.AddDeleteOlderPostsJobAsync(_environment);
        //await _scheduler.AddCheckCancelledEventsJobAsync(_environment);
        //await _scheduler.AddEventsTasksReminderJobAsync(_environment);
#else
        await _scheduler.AddCheckEventsToStartJobAsync(_environment);
        await _scheduler.AddDeleteOlderPostsJobAsync(_environment);
        await _scheduler.AddCheckCancelledEventsJobAsync(_environment);
        await _scheduler.AddEventsTasksReminderJobAsync(_environment);
#endif

    }

    /// <summary>
    /// Stops the jobs
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public async Task Stop(CancellationToken cancellationToken = default)
    {
        if (_scheduler is null) throw new NullReferenceException("Instance not initialized");
        await _scheduler.Shutdown(cancellationToken);
    }
}

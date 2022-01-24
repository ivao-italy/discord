using Ivao.It.DiscordBot.ScheduledTasks.Tasks;
using Microsoft.Extensions.Hosting;
using Quartz;

namespace Ivao.It.DiscordBot.ScheduledTasks;

internal static class JobSchedulerExtensions
{

    /// <summary>
    /// Adds to the scheduler the <see cref="DeletePastEventsPost"/> Job
    /// <para>Prod: every 24hrs at 00:00UTC</para>
    /// <para>Debug: every 30s</para>
    /// </summary>
    /// <param name="scheduler"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    internal static async Task AddDeletePastEventsJobAsync(this IScheduler scheduler, IHostEnvironment environment)
    {
        var init = CreateJobAndTriggerBuilder<DeletePastEventsPost>();

        if (environment.IsDevelopment())
        {
            init.builder.WithSimpleSchedule(s => s.WithIntervalInSeconds(30).RepeatForever());
        }
        else
        {
            init.builder.WithDailyTimeIntervalSchedule(opt =>
                opt.InTimeZone(TimeZoneInfo.Utc)
                    .WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
            );
        }

        await scheduler.ScheduleJob(init.job, init.builder.Build());
    }

    /// <summary>
    /// Adds to the scheduler the <see cref="CheckEventsToStart"/> Job
    /// <para>Prod: every 30mins</para>
    /// <para>Debug: every 15s</para>
    /// </summary>
    /// <param name="scheduler"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    internal static async Task AddCheckEventsToStartJobAsync(this IScheduler scheduler, IHostEnvironment environment)
    {
        var init = CreateJobAndTriggerBuilder<CheckEventsToStart>();

        if (environment.IsDevelopment())
        {
            init.builder.WithSimpleSchedule(s => s.WithIntervalInSeconds(15).RepeatForever());
        }
        else
        {
            init.builder.WithSimpleSchedule(s => s.WithIntervalInMinutes(30).RepeatForever());
        }

        await scheduler.ScheduleJob(init.job, init.builder.Build());
    }

    /// <summary>
    /// Adds to the scheduler the <see cref="CheckCancelledEvents"/> Job
    /// <para>Prod: every 15mins from 1600UTC to 2100UTC</para>
    /// <para>Debug: every 45s</para>
    /// </summary>
    /// <param name="scheduler"></param>
    /// <param name="environment"></param>
    /// <returns></returns>
    internal static async Task AddCheckCancelledEventsJobAsync(this IScheduler scheduler, IHostEnvironment environment)
    {
        var init = CreateJobAndTriggerBuilder<CheckCancelledEvents>();

        if (environment.IsDevelopment())
        {
            init.builder.WithSimpleSchedule(s => s.WithIntervalInSeconds(45).RepeatForever());
        }
        else
        {
            init.builder.WithDailyTimeIntervalSchedule(opt =>
                opt.InTimeZone(TimeZoneInfo.Utc)
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(16, 0))
                    .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 0))
                    .WithIntervalInMinutes(15)
                    .OnEveryDay()
            );
        }

        await scheduler.ScheduleJob(init.job, init.builder.Build());
    }
    
    private static (IJobDetail job, TriggerBuilder builder) CreateJobAndTriggerBuilder<T>() where T : IJob
    {
        IJobDetail job = JobBuilder.Create<T>()
            .WithIdentity($"{typeof(T).Name}_Job", "ExamsTraining")
            .Build();

        TriggerBuilder triggerBuilder = TriggerBuilder.Create()
            .WithIdentity($"{typeof(T).Name}_Trigger", "ExamsTraining")
            .StartNow();

        return new(job, triggerBuilder);
    }
}
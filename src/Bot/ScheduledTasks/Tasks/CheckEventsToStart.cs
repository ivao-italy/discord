using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

/// <summary>
/// Checks if Guild has events ready to be started
/// </summary>
/// <returns></returns>
public sealed class CheckEventsToStart : IJob
{
    /// <summary>
    /// Checks if Guild has events ready to be started
    /// </summary>
    /// <returns></returns>
    public async Task Execute(IJobExecutionContext context)
    {
        var client = ((IvaoItBot)context.Scheduler.Context.Get("Bot")).Client;
        if (client == null) return;


        var guild = client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            client.Logger.LogWarning("CheckEventsToStart - Guild not found on the client");
            return;
        }

        int started = 0;
        foreach (var evt in await guild.GetEventsAsync())
        {
            if (evt.Status == ScheduledGuildEventStatus.Scheduled && (evt.StartTime - DateTime.Now) <= TimeSpan.Zero)
            {
                try
                {
                    await guild.StartEventAsync(evt);
                    started++;
                    client.Logger.LogDebug("Started event {eventId} - {eventName}", evt.Id, evt.Name);
                }
                catch (InvalidOperationException ex)
                {
                    client.Logger.LogError(ex, "Error startig event {eventId} - {eventName}", evt.Id, evt.Name);
                }
            }
        }
        client.Logger.LogInformation("CheckEventsToStart Invoked. Items affected: {items}", started);
    }
}
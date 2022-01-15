using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

/// <summary>
/// Checks if Guild has events ready to be started
/// </summary>
/// <returns></returns>
internal sealed class CheckEventsToStart : BaseScheduledTask
{
    public CheckEventsToStart(IvaoItBot bot)
        : base(bot)
    { }

    /// <summary>
    /// Checks if Guild has events ready to be started
    /// </summary>
    /// <returns></returns>
    protected async override Task DoTaskAsync()
    {
        var client = this.Bot.Client;
        if (client == null) return;

        client.Logger.LogInformation("CheckEventsToStart Invoked");

        var guild = client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            client.Logger.LogWarning("CheckEventsToStart - Guild not found on the client");
            return;
        }

        foreach (var evt in await guild.GetEventsAsync())
        {
            if ((evt.StartTime - DateTime.Now) <= TimeSpan.Zero)
            {
                try
                {
                    await guild.StartEventAsync(evt);
                    client.Logger.LogDebug("Started event {eventId} - {eventName}", evt.Id, evt.Name);
                }
                catch (InvalidOperationException ex)
                {
                    client.Logger.LogError(ex, "Error startig event {eventId} - {eventName}", evt.Id, evt.Name);
                }
            }
        }
    }
}
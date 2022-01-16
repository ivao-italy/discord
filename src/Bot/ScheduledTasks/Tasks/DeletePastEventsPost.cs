using Microsoft.Extensions.Logging;
using Quartz;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

/// <summary>
/// Deletes past events posts from the announcement channel
/// </summary>
internal sealed class DeletePastEventsPost : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var client = ((IvaoItBot)context.Scheduler.Context.Get("Bot")).Client;
        if (client == null) return;

        var guild = client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            client.Logger.LogWarning("DeletePastEventsPost - Guild not found on the client");
            return;
        }

        try
        {
            var channel = guild.GetChannel(IvaoItBot.Config!.AnnouncementsChannelId);
            var pastMessages = (await channel.GetMessagesAsync()).Where(m => m.Timestamp <= DateTime.Now.Date).ToList();
            if (pastMessages.Any())
            {
                await channel.DeleteMessagesAsync(pastMessages);
            }
            client.Logger.LogInformation("DeletePastEventsPost Invoked. Items affected: {items}", pastMessages.Count);
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "DeletePastEventsPost error");
        }
    }
}
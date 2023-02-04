using Microsoft.Extensions.Logging;
using Quartz;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal sealed class DeleteOlderPosts : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var client = ((IvaoItBot)context.Scheduler.Context.Get("Bot")).Client;
        if (client == null) return;

        var guild = client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            client.Logger.LogWarning("{name} - Guild not found on the client", nameof(DeleteOlderPosts));
            return;
        }

        try
        {
            foreach (var channelId in IvaoItBot.Config!.CannelsToBeCleanedUp)
            {
                var channel = guild.GetChannel(channelId);
                var pastMessages = (await channel.GetMessagesAsync())
#if DEBUG
                    .Where(m => m.Timestamp <= DateTime.Now && !m.Pinned)
#else
                    .Where(m => m.Timestamp.Date <= DateTime.Now.Date && !m.Pinned)
#endif
                    .ToList();
                if (pastMessages.Any())
                {
                    await channel.DeleteMessagesAsync(pastMessages);
                }
                client.Logger.LogInformation("{name} - Channel {channel} - Items affected: {items}", nameof(DeleteOlderPosts), channel.Name, pastMessages.Count);
            }
            
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "{name} error", nameof(DeleteOlderPosts));
        }
    }
}

using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal sealed class DeletePastEventsPost : BaseScheduledTask
{
    public DeletePastEventsPost(IvaoItBot bot) : base(bot)
    {
    }

    protected async override Task DoTaskAsync()
    {
        var client = this.Bot.Client;
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
            var pastMessages = (await channel.GetMessagesAsync()).Where(m => m.Timestamp <= DateTime.Now.Date);
            await channel.DeleteMessagesAsync(pastMessages);
            client.Logger.LogInformation("DeletePastEventsPost Invoked");
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "DeletePastEventsPost error");
        }
    }
}
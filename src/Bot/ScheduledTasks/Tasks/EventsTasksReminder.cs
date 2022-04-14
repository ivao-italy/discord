using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Ivao.It.DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;


namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;
internal class EventsTasksReminder : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var bot = (IvaoItBot)context.Scheduler.Context.Get("Bot");
        if (bot.Client == null) return;

        var guild = bot.Client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            bot.Client.Logger.LogWarning("{item} - Guild not found on the client", nameof(EventsTasksReminder));
            return;
        }

        var scopeSp = bot.ServiceScopeFactory.CreateScope().ServiceProvider;
        var service = scopeSp.GetRequiredService<EventsService>();

        var eventsToRemind = await service.GetUpcomingWithToDosAsync();
        
        foreach (var @event in eventsToRemind)
        {
            var toMention = @event.Tasks.Where(t => !t.CompletedAt.HasValue).Select(tt => tt.TaskType.StaffGroupToNofify).ToHashSet();
            var mentions = toMention.Select(r => guild.Roles[r].Mention);

            var mb = new DiscordMessageBuilder()
                .WithContent($"Hey {string.Join(", ", mentions)}! :warning: IT-ED is waiting for you!")
                .WithEmbed(await @event.ToEmbedAsync(guild));
            
            await guild.GetChannel(IvaoItBot.Config!.EventsStaffChannelId).SendMessageAsync(mb);
        }
    }
}

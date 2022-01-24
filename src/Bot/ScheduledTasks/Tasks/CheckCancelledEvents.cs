using DSharpPlus.Entities;
using Ivao.It.Discord.Shared.Models;
using Ivao.It.Discord.Shared.Services;
using Ivao.It.DiscordBot.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

/// <summary>
/// Checks if any of the posted exams/trainings for today have been cancelled. If so, announces the cancellation and deletes the related Guild Scheduled Event
/// </summary>
internal class CheckCancelledEvents : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var bot = (IvaoItBot)context.Scheduler.Context.Get("Bot");
        if (bot.Client == null) return;


        var guild = bot.Client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            bot.Client.Logger.LogWarning("CheckCancelledEvents - Guild not found on the client");
            return;
        }

        try
        {
            //All the events posted in the channel
            var channel = guild.GetChannel(IvaoItBot.Config!.AnnouncementsChannelId);
            Dictionary<string, EventType> postedEvents = new();
            List<string> announcedCancellations = new();
            foreach (var post in (await channel.GetMessagesAsync()).Where(m => m.Timestamp.Date == DateTime.Now.Date))
            {
                foreach (var emb in post.Embeds)
                {
                    if (Consts.FacilityRegex.IsMatch(emb.Description))
                    {
                        var facility = Consts.FacilityRegex.Match(emb.Description).Value;
                        if (emb.Color.HasValue && emb.Color.Value.Value != DiscordEmbedHelper.Orange.Value)
                            postedEvents.Add(facility, this.GetTypeFromTitle(emb.Title));
                        else
                            announcedCancellations.Add(facility);
                    }
                }
            }

            int itemsCancelled = 0;
            if (postedEvents.Count != 0)
            {
                //All the events in the DB
                var scopeSp = bot.ServiceScopeFactory.CreateScope().ServiceProvider;
                var db = scopeSp.GetRequiredService<DiscordDbContext>();
                var examTrainings = (await scopeSp.GetRequiredService<ITrainingAndExamsService>()
                        .GetPlannedAsync(db, DateOnly.FromDateTime(DateTime.Today)))
                    .Select(e => e.Facility);

                //Calculate what Training/Exams have been deleted/postponed and their cancellation hasn't been already posted (not happening today)
                var cancellationsToBeAnnounced = postedEvents
                    .Where(pe => examTrainings.All(x => x != pe.Key) && announcedCancellations.All(x => x != pe.Key))
                    .ToDictionary(k => k.Key, v => v.Value);
                itemsCancelled = cancellationsToBeAnnounced.Count;

                //Annuncement of the cancellation and cancellation of the Guild scheduled event
                var events = await guild.GetEventsAsync();
                var builder = new DiscordMessageBuilder();
                foreach (var toBeCancelled in cancellationsToBeAnnounced)
                {
                    builder.AddEmbed(await DiscordEmbedHelper.GetEmbedWarning(
                        guild,
                        $"{toBeCancelled.Value} rimandato!",
                        $"Attenzione! L'evento previsto oggi a **{toBeCancelled.Key}** non avrà luogo! Ci scusiamo per il disagio!"));

                    var evt = events.SingleOrDefault(e => e.Name.Contains(toBeCancelled.Key));
                    if (evt != null) await guild.CancelEventAsync(evt);
                }

                //Auto crossposted by the message handler listening on that channel
                if (cancellationsToBeAnnounced.Count > 0)
                    await builder.SendAsync(channel);
            }

            bot.Client.Logger.LogInformation("CheckCancelledEvents Invoked. Items affected: {items}", itemsCancelled);
        }
        catch (Exception ex)
        {
            bot.Client.Logger.LogError(ex, "CheckCancelledEvents error");
        }
    }


    private EventType GetTypeFromTitle(string title)
    {
        if (title.StartsWith(Consts.ExamTitle))
            return EventType.Exam;
        if (title.StartsWith(Consts.TrainingTitle))
            return EventType.Training;
        throw new ArgumentOutOfRangeException(nameof(title), "Title not in correct format");
    }

}

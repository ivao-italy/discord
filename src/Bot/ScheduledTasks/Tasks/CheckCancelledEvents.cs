using DSharpPlus.Entities;
using Ivao.It.Discord.Shared.Models;
using Ivao.It.Discord.Shared.Services;
using Ivao.It.DiscordBot.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal class CheckCancelledEvents : BaseScheduledTask
{
    public CheckCancelledEvents(IvaoItBot bot) : base(bot)
    {
    }

    protected override async Task DoTaskAsync()
    {
        var client = this.Bot.Client;
        if (client == null) return;

        var guild = client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            client.Logger.LogWarning("CheckCancelledEvents - Guild not found on the client");
            return;
        }

        try
        {
            //All the events posted in the channel
            var channel = guild.GetChannel(IvaoItBot.Config!.AnnouncementsChannelId);
            var postedEvents = new Dictionary<string, EventType>();
            foreach (var post in (await channel.GetMessagesAsync()).Where(m => m.Timestamp.Date == DateTime.Now.Date))
            {
                var embedsNotOrangeDescriptions = post.Embeds
                    .Where(e => e.Color.HasValue && e.Color.Value.Value != DiscordColor.Orange.Value);

                foreach (var emb in embedsNotOrangeDescriptions)
                {
                    if (Consts.FacilityRegex.IsMatch(emb.Description))
                    {
                        postedEvents.Add(Consts.FacilityRegex.Match(emb.Description).Value, this.GetTypeFromTitle(emb.Title));
                    }
                }
            }
            if (postedEvents.Count == 0) return;


            //All the events in the DB
            var scopeSp = this.Bot.ServiceScopeFactory.CreateScope().ServiceProvider;
            var db = scopeSp.GetRequiredService<DiscordDbContext>();
            var examTrainings = (await scopeSp.GetRequiredService<ITrainingAndExamsService>()
                .GetPlannedAsync(db, DateOnly.FromDateTime(DateTime.Today)))
                .Select(e => e.Facility);

            //Calculate what Training/Exams have been deleted/postponed (not happening today)
            var cancellationsToBeAnnounced = postedEvents
                .Where(pe => examTrainings.All(x => x != pe.Key))
                .ToDictionary(k => k.Key, v => v.Value);

            //Annuncement of the cancellation and cancellation of the Guild scheduled event
            var events = await guild.GetEventsAsync();
            var builder = new DiscordMessageBuilder();
            foreach (var toBeCancelled in cancellationsToBeAnnounced)
            {
                builder.AddEmbed(await DiscordEmbedHelper.GetEmbedWarning(
                    guild,
                    $"{toBeCancelled.Value} @ {toBeCancelled.Key} rimandato/cancellato!", 
                    "Attenzione! L'evento in oggetto, questa sera, non avrà luogo! Ci scusiamo per il disagio!"));

                var evt = events.SingleOrDefault(e => e.Name.Contains(toBeCancelled.Key));
                if (evt != null) await guild.CancelEventAsync(evt);
            }

            //Auto crossposted by the message handler listening on that channel
            await builder.SendAsync(channel);

            client.Logger.LogInformation("CheckCancelledEvents Invoked");
        }
        catch (Exception ex)
        {
            client.Logger.LogError(ex, "CheckCancelledEvents error");
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

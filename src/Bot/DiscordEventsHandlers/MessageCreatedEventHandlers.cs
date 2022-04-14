using DSharpPlus;
using DSharpPlus.EventArgs;
using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Ivao.It.DiscordBot.DiscordEventsHandlers;

/// <summary>
/// Contenitore di handlers per gli eventi del client Discord
/// </summary>
public class MessageCreatedEventHandlers
{
    private readonly IDbContextFactory<DiscordDbContext> _dbFactory;

    public MessageCreatedEventHandlers(
        IDbContextFactory<DiscordDbContext> db)
    {
        this._dbFactory = db;
    }

    /// <summary>
    /// Reagisce al post di un nuovo evento tramite il webhook impostato sul canale annunci, e trasforma quanto postato in un evento sul server Discord
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal async Task EventPosted_MakeEvent(DiscordClient sender, MessageCreateEventArgs e)
    {
        //Webhook pubblicazione eventi
        if (e.Channel.Id == IvaoItBot.Config!.AnnouncementsChannelId && e.Author.Id != IvaoItBot.Config.BotUserId)
        {
            var guild = sender.Guilds.SingleOrDefault().Value;

            foreach (var embed in e.Message.Embeds)
            {
                var startStr = RegExpressions.StartTime.Match(embed.Description).Value;
                var start = DateTime.Now.Date; //Viene lanciato sempre per gli eventi del giorno stesso
                start = new DateTime(start.Year, start.Month, start.Day, int.Parse(startStr[..2]), int.Parse(startStr[2..]), 0, DateTimeKind.Utc);

                var evt = new ScheduledEvent
                {
                    Title = embed.Title.Split('\n')[0].Replace("!", string.Empty),
                    Facility = RegExpressions.Facility.Match(embed.Description).Value,
                    Description = string.Join("", RegExpressions.Stars.Matches(embed.Title).Select(m => m.Value)),
                    StartDateTime = start,
                };

                var ev = await evt.AddToGuildAsync(guild);
            }
        }
    }


    /// <summary>
    /// Crossposta ogni messaggio che viene postato nel canale annunci
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal async Task EventPosted_Crosspost(DiscordClient sender, MessageCreateEventArgs e)
    {
        //Webhook pubblicazione eventi
        if (e.Channel.Id == IvaoItBot.Config!.AnnouncementsChannelId)
        {
            await e.Message.Channel.CrosspostMessageAsync(e.Message);
        }
    }

    /// <summary>
    /// Gestione dell'attivazione di un utente nell'apposito canale
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <returns></returns>
    internal async Task UserActivation(DiscordClient sender, MessageCreateEventArgs e)
    {
        if (e.Channel.Id == IvaoItBot.Config!.ActivationChannelId && e.Author.Id != IvaoItBot.Config.BotUserId)
        {
            using var db = await _dbFactory.CreateDbContextAsync();
            var user = await db.Users.SingleOrDefaultAsync(u => u.DiscordUserId == e.Message.Author.Id);
            if (user == null)
            {
                var embed = await DiscordEmbedHelper.GetWarningAsync(e.Guild, "User not recognized", $"We're unable to detect who you are. Please go to {IvaoItBot.Config.DiscordAccessWizardHomepage}");
                await e.Channel.SendMessageAsync(embed);
                return;
            }
            //Recupero e valido l'utente
            var member = await e.Guild.GetMemberAsync(e.Author.Id);
            var chk = await UserVerificationManager.ForceUserVerificationAsync(sender, member, e.Guild, user);
        }
    }



    private class RegExpressions
    {
        public static Regex Stars => new(":star:", RegexOptions.Compiled);
        public static Regex Facility => new(@"((?<=\*\*)\w{4}_\w{0,3}_\w{3})|((?<=\*\*)\w{4}_\w{3})", RegexOptions.Compiled);
        public static Regex StartTime => new(@"\d{4}", RegexOptions.Compiled);
    }
}

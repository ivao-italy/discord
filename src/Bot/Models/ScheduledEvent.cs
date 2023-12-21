using DSharpPlus;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Extensions;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Models;

/// <summary>
/// Model di appoggio per un event discord -> il model dell'SDK non ha costruttore pubblico ed è sealed
/// </summary>
internal class ScheduledEvent
{
    private DateTime _startDateTime;

    public string? Title { get; set; }
    public string? Facility { get; set; }
    public string? Description { get; set; }
    public ulong? ChannelId { get; private set; }
    public DateTime StartDateTime {
        get => _startDateTime;
        set {
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("it-IT");
            _startDateTime = value.ToLocalTime();
        }
    }

    public string Name => $"{Title} {Facility}";
    public ScheduledGuildEventType EventType => this.ChannelId != null ? ScheduledGuildEventType.VoiceChannel : ScheduledGuildEventType.External;
    public ScheduledGuildEventPrivacyLevel PrivacyLevel => ScheduledGuildEventPrivacyLevel.GuildOnly;
    public DateTimeOffset Start => new(StartDateTime);
    public DateTimeOffset End => new(this.Start.DateTime.AddHours(2));

    /// <summary>
    /// Aggiunge l'evento alla Guild
    /// </summary>
    /// <param name="guild"></param>
    /// <returns></returns>
    public async Task<DiscordScheduledGuildEvent> AddToGuildAsync(DiscordGuild guild, ILogger<BaseDiscordClient> logger)
    {
        //Channel name calculations (Discord channel naming by convention)
        var airport = this.Facility![..4];
        var channelName = airport.GetAccIcao();

        //channel not found => evento esterno (gestito dal getter dell'event type)
        var overlappingExistingEvents =
            (await guild.GetEventsAsync())
            .Where(e => e.StartTime <= this.End || this.Start <= e.EndTime)
            .ToList();
        logger.LogWarning("Found overlapping events in channels: {events}", string.Join(", ", overlappingExistingEvents.Select(e => e.Name)));

        var channels = (await guild.GetChannelsAsync())
            .OrderBy(c => c.Position)
            .Where(c => c.Name.StartsWith(channelName))
            .ToList();
        logger.LogWarning("Found channels for ACC section: {channels}", string.Join(", ", channels.Select(c => c.Name)));


        foreach (var channel in channels)
        {
            //Event already existing for the first channel in ACC section
            if (overlappingExistingEvents.Any(e => e.ChannelId == channel.Id))
            {
                logger.LogWarning("Found overlapping event, skipping channel: {event} - {channel}", this.Name, channel.Name);
                continue;
            }

            //No event already planned on this channel, picking channel and stopping the loop
            this.ChannelId = channel.Id;
            logger.LogWarning("Matching event and channel: {event} on {channel}", this.Name, channel.Name);
            break;
        }

        //create event
        return await guild.CreateEventAsync(
                       this.Name,
                       this.Description,
                       this.ChannelId,
                       this.EventType,
                       this.PrivacyLevel,
                       this.Start,
                       this.End,
                       location: this.EventType == ScheduledGuildEventType.External ? "IVAO Italy" : null);
    }
}

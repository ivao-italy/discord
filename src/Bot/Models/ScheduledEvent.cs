using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Extensions;

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
    public async Task<DiscordScheduledGuildEvent> AddToGuildAsync(DiscordGuild guild)
    {
        //Channel name calculations (Discord channel naming by convention)
        var airport = this.Facility![..4];
        string channelName = airport;
        if (airport.IsAccIcao())
        {
            if (this.Facility.IsAccFacility()) channelName = $"{airport} ACC Room";
            if (this.Facility.IsAppFacility()) channelName = $"{airport} ARR/DEP";
        }

        //channel not found => evento esterno (gestito dal getter dell'event type)
        var channel = (await guild.GetChannelsAsync()).FirstOrDefault(c => c.Name.StartsWith(channelName));
        this.ChannelId = channel?.Id;

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

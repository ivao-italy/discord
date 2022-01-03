using System.Diagnostics;

namespace Ivao.It.DiscordLogin.Models;

[DebuggerDisplay("{" + nameof(Facility) + "} {" + nameof(Planned) + "}")]
public class IvaoScheduledEvent
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Airport { get; init; }
    public string Facility { get; init; }
    public AtcRating Rating { get; init; }
    public DateTime Planned { get; init; }
    public EventType Type { get; init; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}

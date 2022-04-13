namespace Ivao.It.DiscordBot.Models.Events;

internal static class EnumExtensions
{
    internal static EventsTasks ToEventsTasks(this string str) => str switch {
        "rfe" => EventsTasks.WebBookingTool,
        "banner" => EventsTasks.Graphics,
        "routes" => EventsTasks.Routes,
        _ => throw new ArgumentOutOfRangeException("Event task string"),
    };
}
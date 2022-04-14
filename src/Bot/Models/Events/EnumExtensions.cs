namespace Ivao.It.DiscordBot.Models.Events;

internal static class EnumExtensions
{
    internal static EventsTasks ToEventsTasks(this string str) => str switch {
        "rfe" => EventsTasks.WebBookingTool,
        "forum" => EventsTasks.ForumTopic,
        "announcement" => EventsTasks.Announcement,
        "social" => EventsTasks.AnnouncementSocial,
        "atcs" => EventsTasks.Atcs,
        "banner" => EventsTasks.Graphics,
        "routes" => EventsTasks.Routes,
        _ => throw new ArgumentOutOfRangeException("Event task string"),
    };
}
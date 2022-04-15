using DSharpPlus;
using DSharpPlus.Entities;

namespace Ivao.It.DiscordBot.Models.Events;

internal static class EnumExtensions
{
    internal static EventsTasks ToEventsTasks(this string str) => str switch
    {
        "rfe" => EventsTasks.WebBookingTool,
        "forum" => EventsTasks.ForumTopic,
        "announcement" => EventsTasks.Announcement,
        "social" => EventsTasks.AnnouncementSocial,
        "atcs" => EventsTasks.Atcs,
        "banner" => EventsTasks.Graphics,
        "routes" => EventsTasks.Routes,
        _ => throw new ArgumentOutOfRangeException("Event task string"),
    };

    internal static string ToEmojiName(this EventsTasks value) => value switch
    {
        EventsTasks.Atcs => ":satellite:",
        EventsTasks.AnnouncementSocial => ":loudspeaker:",
        EventsTasks.Announcement => ":mega:",
        EventsTasks.Routes => ":ticket:",
        EventsTasks.Graphics => ":camera_with_flash:",
        EventsTasks.ForumTopic => ":paperclips:",
        EventsTasks.WebBookingTool => ":computer:",
        _ => throw new ArgumentOutOfRangeException("Task not associated with Emoji"),
    };
    internal static DiscordEmoji ToEmoji(this EventsTasks value, DiscordClient client) 
        => DiscordEmoji.FromName(client, value.ToEmojiName());

    internal static EventsTasks ToEnum(this DiscordEmoji value) => value.GetDiscordName() switch
    {
        ":satellite:" => EventsTasks.Atcs,
        ":loudspeaker:" => EventsTasks.AnnouncementSocial,
        ":mega:" => EventsTasks.Announcement,
        ":ticket:" => EventsTasks.Routes,
        ":camera_with_flash:" => EventsTasks.Graphics,
        ":paperclips:" => EventsTasks.ForumTopic,
        ":computer:" => EventsTasks.WebBookingTool,
        _ => throw new ArgumentOutOfRangeException("Task not associated with Emoji"),
    };
}
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Data.Entities;

namespace Ivao.It.DiscordBot.Models.Events;
internal static class EntitiesExtensions
{
    /// <summary>
    /// Adds an event to the EmbedBuilder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="evt"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public static async Task<DiscordEmbedBuilder> CreateEventAsync(this DiscordEmbedBuilder builder, Event evt, DiscordGuild guild)
    {
        builder
            .WithColor(DiscordEmbedHelper.Green)
            .WithTitle(evt.Name)
            .WithTimestamp(DateTime.Now)
            .WithAuthor("IVAO Italia", @"https://www.ivao.it", guild.IconUrl ?? (await guild.GetMemberAsync(IvaoItBot.Config!.BotUserId)).AvatarUrl);

        var mentionAuthor = (await guild.GetMemberAsync(evt.CreatedByUserId)).Mention;
        evt.Tasks = evt.Tasks.OrderBy(t => evt.Date.AddDays(-t.TaskType.DaysBefore)).ToList();

        var markdown = $"__**A new event has been inserted**{Environment.NewLine}{Environment.NewLine}";
        markdown += $"**#{evt.Id} {evt.Name}**{Environment.NewLine}";
        markdown += $"Starting at {evt.Date:yyyy MMMM dd} - Created by {mentionAuthor}{Environment.NewLine}";
        if (!string.IsNullOrEmpty(evt.Link))
        {
            markdown += $"{evt.Link}{Environment.NewLine}";
        }
        markdown += $"{Environment.NewLine}Tasks to be completed:";
        builder.WithDescription(markdown);

        foreach (var task in evt.Tasks)
        {
            await builder.TaskToFieldAsync(guild, task, evt.Date);
        }

        return builder;
    }

    /// <summary>
    /// Adds event tasks to the builder as fields
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="guild"></param>
    /// <param name="task"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    public static async Task TaskToFieldAsync(this DiscordEmbedBuilder builder, DiscordGuild guild, EventTask task, DateTime date)
    {
        if (task.CompletedAt is null)
        {
            builder.AddField(
                $":pushpin: {task.TaskType.Description}",
                $"Due {date.AddDays(-task.TaskType.DaysBefore):yyyy MMMM dd}",
                true);
        }
        else
        {
            var mention = (await guild.GetMemberAsync(task.CompletedBy!.Value)).Mention;
            builder.AddField(
                $":white_check_mark: {task.TaskType.Description}",
                $"Completed at {task.CompletedAt:yyyy MMMM dd} by {mention}",
                true);
        }
    }
}

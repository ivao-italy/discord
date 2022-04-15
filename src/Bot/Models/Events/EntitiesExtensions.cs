using DSharpPlus;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Data.Entities;

namespace Ivao.It.DiscordBot.Models.Events;
internal static class EntitiesExtensions
{
    public static async Task<DiscordMessage> SendEvent(
        this Event evt,
        DiscordClient client,
        DiscordGuild guild,
        DiscordChannel channel,
        bool insertConfirmation = false)
    {
        var embed = await evt!.ToEmbedAsync(guild, insertConfirmation: true);
        var message = await channel.SendMessageAsync(embed);
        await message.AppendTasksReactions(client, evt.Tasks);
        return message;
    }

    public static async Task<DiscordMessage> ReplyWithEvent(
        this Event evt,
        DiscordClient client,
        DiscordGuild guild,
        DiscordMessage replyTo,
        bool insertConfirmation = false)
    {
        var embed = await evt!.ToEmbedAsync(guild, insertConfirmation: true);
        var message = await replyTo.RespondAsync(embed);
        await message.AppendTasksReactions(client, evt.Tasks);
        return message;
    }

    /// <summary>
    /// Adds an event to the EmbedBuilder
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="evt"></param>
    /// <param name="guild"></param>
    /// <returns></returns>
    public static async Task<DiscordEmbedBuilder> ToEmbedAsync(this Event evt, DiscordGuild guild, bool insertConfirmation = false)
    {
        var builder = await DiscordEmbedHelper.GetAsync(guild, $"Event Code:{evt.Id} - {evt.Name}");
        if (insertConfirmation)
        {
            builder.WithDescription("A new event has been planned!");
            builder.WithColor(DiscordEmbedHelper.Green);
        }


        var mentionAuthor = (await guild.GetMemberAsync(evt.CreatedByUserId)).Mention;
        evt.Tasks = evt.Tasks.OrderBy(t => evt.Date.AddDays(-t.TaskType.DaysBefore)).ToList();
        var daysBefore = (evt.Date - DateTime.UtcNow).Days;

        var markdown = $"Starting in {daysBefore} days ({evt.Date:yyyy MMMM dd}){Environment.NewLine}Created by {mentionAuthor}{Environment.NewLine}";
        if (!string.IsNullOrEmpty(evt.Link))
        {
            builder.WithUrl(evt.Link);
        }
        markdown += $"{Environment.NewLine}Tasks status:";
        builder.WithDescription(markdown);

        foreach (var task in evt.Tasks)
        {
            await builder.TaskToFieldAsync(guild, task, evt.Date);
        }

        return builder;
    }


    /// <summary>
    /// Appends the reactions for the tasks to be done (to help users to react correctly)
    /// </summary>
    /// <param name="message"></param>
    /// <param name="client"></param>
    /// <param name="tasks"></param>
    /// <returns></returns>
    internal static async Task AppendTasksReactions(this DiscordMessage message, DiscordClient client, IEnumerable<EventTask> tasks)
    {
        var reactions = tasks.Where(t => !t.CompletedBy.HasValue)
            .Select(t => (EventsTasks)t.TaskTypeId)
            .Select(async t =>
            {
                await message.CreateReactionAsync(t.ToEmoji(client));
            });

        await Task.WhenAll(reactions);
    }


    /// <summary>
    /// Adds event tasks to the builder as fields
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="guild"></param>
    /// <param name="task"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    private static async Task TaskToFieldAsync(this DiscordEmbedBuilder builder, DiscordGuild guild, EventTask task, DateTime date)
    {
        if (task.CompletedAt is null)
        {
            var targetGroup = guild.GetRole(task.TaskType.StaffGroupToNofify).Mention;
            var emoji = ((EventsTasks)task.TaskTypeId).ToEmojiName();
            builder.AddField(
                $":pushpin: {task.TaskType.Description}",
                $"Due {date.AddDays(-task.TaskType.DaysBefore):yyyy MMMM dd}{Environment.NewLine}{targetGroup}!{Environment.NewLine}react with {emoji} to complete!",
                true);
            return;
        }

        if (task.TaskTypeId == (short)EventsTasks.Graphics)
        {
            builder.WithImageUrl(task.Content);
        }
        var mention = (await guild.GetMemberAsync(task.CompletedBy!.Value)).Mention;
        builder.AddField(
            $":white_check_mark: {task.TaskType.Description}",
            $"{task.CompletedAt:yyyy MMMM dd} by {mention}",
            true);
    }

}

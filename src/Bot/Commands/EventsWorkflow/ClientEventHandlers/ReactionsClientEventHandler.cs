using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using Ivao.It.DiscordBot.Extensions;
using Ivao.It.DiscordBot.Models.Events;
using Ivao.It.DiscordBot.Services;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow.ClientEventHandlers;
internal class ReactionsClientEventHandlers
{
    private readonly EventsService _service;

    private static Regex RgxEventPostTitle = new Regex(@"(?<=(Event Code:))\d", RegexOptions.Compiled);
    private static ICollection<EventsTasks> _NoContentTasks =>
        new List<EventsTasks> {
            EventsTasks.Routes,
            EventsTasks.AnnouncementSocial,
            EventsTasks.Announcement,
            EventsTasks.Atcs
        };

    public ReactionsClientEventHandlers(EventsService service)
    {
        _service = service;
    }



    public Task OnEventPostOnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            //Validation: is a reaction to be processed?
            e.Handled = true;
            var targetMessage = e.Message;
            if (e.Message.Author is null || e.Message.Content is null || e.Message.Embeds is null)
                targetMessage = await e.Channel.GetMessageAsync(e.Message!.Id);

            //No reaction collection for non-bot messages
            //No bot reactions collection
            //Check & find event id?
            int eventId = default;
            if (
                !IsTheBot(targetMessage.Author) ||
                IsTheBot(e.User) ||
                !TryGetEventId(targetMessage, out eventId)
                )
                return;

            var task = e.Emoji.ToEnum();
            if (!await _service.IsTaskCompleted(task, eventId))
                return;

            await e.Channel.TriggerTypingAsync();
            string? content = null;

            //Needs the content... collect it
            if (!_NoContentTasks.Contains(task))
                content = await this.PromptUserForContentAsync(e.Channel, e.User);

            //Store the task completion
            try
            {
                await _service.CompleteTaskAsync(task, eventId, e.User.Id, content);
            }
            catch (Exception ex)
            {
                s.Logger.LogError(ex, "Event recap reaction: Error updating the task - {event} - {taskType} - {user}",
                    eventId,
                    task.ToString(),
                    e.User.Username);
                throw;
            }

            //Confirm to the user
            var replyTo = await e.Message.RespondAsync($"Ok {e.User.Mention}! You made a good job :beers:");
            //Event status recap
            var evt = await _service.GetAsync(eventId);
            await evt!.ReplyWithEvent(s, e.Guild, replyTo, true);

        });
        return Task.CompletedTask;
    }



    private bool IsTheBot(DiscordUser user)
        => user.IsBot && user.Id == IvaoItBot.Config!.BotUserId;

    private bool TryGetEventId(DiscordMessage message, out int id)
    {
        var match = RgxEventPostTitle.Match(message.Embeds.Single().Title);
        return int.TryParse(match.Value, out id);
    }

    private async Task<string?> PromptUserForContentAsync(DiscordChannel channel, DiscordUser user)
    {
        var message = await channel
            .SendMessageAsync(new DiscordMessageBuilder()
                .WithContent($"Please write me down the link expected for this task."));

        var result = await channel.GetNextMessageAsync(user, TimeSpan.FromSeconds(15));

        if (result.TimedOut || !result.Result.Content.IsValidUri())
        {
            throw new IvaoItBotBusinessException(
                "You waited too long to reply or maybe this is not a link in the correct format like (https://www.ivao.it/test/page). Reply again to the previous message with a correct Url");
        }

        return result.Result.Content;
    }
}

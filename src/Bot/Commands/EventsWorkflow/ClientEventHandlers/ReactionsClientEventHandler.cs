using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using Ivao.It.DiscordBot.Extensions;
using Ivao.It.DiscordBot.Models.Events;
using Ivao.It.DiscordBot.Services;
using System.Text.RegularExpressions;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow.ClientEventHandlers;
internal class ReactionsClientEventHandlers
{
    private readonly EventsService _service;

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

    private static Regex RgxEventPostTitle = new Regex(@"(?<=(Event Code:))\d", RegexOptions.Compiled);

    public Task OnEventPostOnMessageReactionAdded(DiscordClient s, MessageReactionAddEventArgs e)
    {
        _ = Task.Run(async () =>
        {
            //Validation: is a reaction to be processed?
            e.Handled = true;
            var targetMessage = e.Message;
            if (e.Message.Author is null || e.Message.Content is null || e.Message.Embeds is null)
            {
                targetMessage = await e.Channel.GetMessageAsync(e.Message!.Id);
            }
            
            //No reaction collection for non-bot messages
            if (!(targetMessage.Author.IsBot && targetMessage.Author.Id == IvaoItBot.Config!.BotUserId))
                return;
            //No bot reactions collection
            if (e.User.IsBot && e.User.Id == IvaoItBot.Config!.BotUserId)
            {
                return;
            }

            var match = RgxEventPostTitle.Match(targetMessage.Embeds.Single().Title);
            if (!match.Success)
                return;

            await e.Channel.TriggerTypingAsync();

            //Which reaction has been added?
            var task = e.Emoji.ToEnum();
            string? content = null;

            //Needs the content... collect it
            if (!_NoContentTasks.Contains(task))
            {
                var message = await e.Channel
                    .SendMessageAsync(new DiscordMessageBuilder()
                        .WithContent($"Please write me down the link expected for this task."));
                
                var result = await e.Channel.GetNextMessageAsync(e.User, TimeSpan.FromSeconds(15));

                if (result.TimedOut || !result.Result.Content.IsValidUri())
                {
                    await message.Channel.SendMessageAsync(
                        new DiscordMessageBuilder()
                            .WithContent("Nope... you waited too long to reply or maybe this is not a link in the correct format like (https://www.ivao.it/test/page). Reply again to the previous message with a correct Url"));
                    return;
                }

                content = message.Content;
            }

            //Store the task completion if not yet completed
            var eventId = int.Parse(match.Value);
            if (!await _service.IsTaskCompleted(task, eventId))
            {
                await e.Message.RespondAsync($"Nope... this task is already marked as completed.");
                return;
            }

            await _service.CompleteTaskAsync(task, eventId, e.User.Id, content);
            //Confirm to the user
            var replyTo = await e.Message.RespondAsync($"Ok {e.User.Mention}! You made a good job :beers:");
            //Event status recap
            var evt = await _service.GetAsync(eventId);
            var embed = await evt!.ToEmbedAsync(e.Guild, insertConfirmation: true);
            await replyTo.RespondAsync(embed);

            //TODO logging + error handling
        });
        return Task.CompletedTask;
    }
}

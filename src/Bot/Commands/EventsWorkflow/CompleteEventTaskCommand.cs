using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;
internal partial class EventsCommands
{
    [Command("complete")]
    [Description("Declares as completed a Task related to the event.")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [CheckCallChannel]
    public async Task Completed(
        CommandContext ctx,
        [Description("Event code (the number before # in the event post)")] int eventId,
        [Description("Task completed")] EventsTasks task,
        [Description("Task content (link to forum, rfe tool, or the image to post the event on Discord)")] string? content = null
        )
    {
        var evt = await _service.GetAsync(eventId);
        if (evt is null)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetErrorAsync(
                ctx.Guild, 
                "Ops!",
                $"Sorry, I don't know anything 'bout an event with code '{eventId}'. Maybe you can tell me more about planning a new event?"));
            ctx.Client.Logger.LogWarning("Event task completed command: Event not found - {event} - {user}",
                eventId,
                ctx.Member?.Nickname ?? ctx.User.Username);
        }
    }
}

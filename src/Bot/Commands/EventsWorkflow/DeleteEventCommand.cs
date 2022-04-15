using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Commands.EventsWorkflow.Attributes;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;
internal partial class EventsCommands
{
    [Command("delete")]
    [Description("Deletes the event")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [InEventsChannelOnly]
    public async Task Delete(
        CommandContext ctx,
        [Description("Event code (the number before # in the event post)")] int eventId
        )
    {
        try
        {
            await this._service.DeleteEventAsync(eventId);
            await ctx.RespondAsync(await DiscordEmbedHelper.GetSuccessAsync(
                ctx.Guild,
                "Well done, mate!",
                "Event and its tasks has been deleted."));
        }
        catch (Exception e)
        {
            await ctx.RespondAsync(await DiscordEmbedHelper.GetErrorAsync(
                ctx.Guild,
                "Ops!",
                $"Something went wrong deleting the Event...{Environment.NewLine}{e.Message}"));
            ctx.Client.Logger.LogError(e, "Event delete error: {event} - {user}",
                eventId,
                ctx.Member?.Nickname ?? ctx.User.Username);
        }
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Commands.EventsWorkflow.Attributes;
using Ivao.It.DiscordBot.Models.Events;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;

internal partial class EventsCommands
{
    [Command("planned")]
    [Description("Shows the upcoming events scheduled and their tasks status.")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [InEventsChannelOnly]
    public async Task Planned(CommandContext ctx)
    {
        var events = await this._service.GetAsync();

        foreach (var @event in events)
            await @event.ReplyWithEvent(ctx.Client, ctx.Guild, ctx.Message);
    }
}

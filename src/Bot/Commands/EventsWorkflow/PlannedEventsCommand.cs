using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Models.Events;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;

internal partial class EventsCommands
{
    [Command("planned")]
    [Description("Shows the upcoming events scheduled and their tasks status.")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [CheckCallChannel]
    public async Task Planned(CommandContext ctx)
    {
        var events = await this._service.GetAsync();

        var tasks = events.Select(async e => (DiscordEmbed)(await e.ToEmbedAsync(ctx.Guild)));
        var messageBuilder = new DiscordMessageBuilder();
        messageBuilder.AddEmbeds((await Task.WhenAll(tasks)).ToList());
        await ctx.RespondAsync(messageBuilder);
    }
}

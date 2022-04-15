using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
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

        foreach (var @event in events)
        {
            var embed = await @event.ToEmbedAsync(ctx.Guild);
            var message = await ctx.RespondAsync(embed);
            var reactionsTasks = @event.Tasks.
                Where(t => !t.CompletedBy.HasValue)
                .Select(t => (EventsTasks)t.TaskTypeId)
                .Select(async t =>
                {
                    await message.CreateReactionAsync(t.ToEmoji(ctx.Client));
                });

            await Task.WhenAll(reactionsTasks);
        }
        //var tasks = events.Select(async e => (DiscordEmbed)(await e.ToEmbedAsync(ctx.Guild)));
        //var messageBuilder = new DiscordMessageBuilder();
        //messageBuilder.AddEmbeds((await Task.WhenAll(tasks)).ToList());
    }
}

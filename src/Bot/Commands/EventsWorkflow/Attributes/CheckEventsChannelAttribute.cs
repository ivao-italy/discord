using DSharpPlus.CommandsNext;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow.Attributes;

internal class InEventsChannelOnlyAttribute : BaseCheckBotChannelAttribute
{
    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return await this.CheckIfInChannelAsync(ctx, IvaoItBot.Config!.EventsStaffChannelId);
    }
}
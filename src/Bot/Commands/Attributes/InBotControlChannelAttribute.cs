using DSharpPlus.CommandsNext;

namespace Ivao.It.DiscordBot.Commands;

internal class InBotControlChannelAttribute : BaseCheckBotChannelAttribute
{
    public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return await this.CheckIfInChannelAsync(ctx, IvaoItBot.Config!.BotControlChannelId);
    }
}
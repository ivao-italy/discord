using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands;

internal abstract class IvaoBaseCommandModule : BaseCommandModule
{
    protected bool IsChannelCorrect(CommandContext ctx)
    {
        if (ctx.Message.ChannelId != IvaoItBot.Config!.BotControlChannelId)
        {
            ctx.Client.Logger.LogWarning("'{commandName}' command invoked from wrong channel ({channel})", ctx.Command!.Name, ctx.Channel.Name);
            return false;
        }
        ctx.Client.Logger.LogDebug("'{commandName}' command invoked from correct channel", ctx.Command!.Name);
        return true;
    }
}

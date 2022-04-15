using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands;
internal abstract class BaseCheckBotChannelAttribute : CheckBaseAttribute
{
   protected async Task<bool> CheckIfInChannelAsync(CommandContext ctx, ulong channelId)
    {
        if (ctx.Message.ChannelId != channelId)
        {
            ctx.Client.Logger.LogWarning("'{commandName}' command invoked from wrong channel ({channel})", ctx.Command!.Name, ctx.Channel.Name);
            return false;
        }
        ctx.Client.Logger.LogDebug("'{commandName}' command invoked from correct channel", ctx.Command!.Name);
        await ctx.TriggerTypingAsync();
        return true;
    }
}
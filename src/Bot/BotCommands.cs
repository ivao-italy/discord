using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot;

internal class BotCommands : BaseCommandModule
{
    private static bool IsChannelCorrect(CommandContext ctx)
    {
        if (ctx.Message.ChannelId != IvaoItBot.Config!.BotControlChannelId)
        {
            ctx.Client.Logger.LogWarning("'{commandName}' command invoked from wrong channel ({channel})", ctx.Command.Name, ctx.Channel.Name);
            return false;
        }
        ctx.Client.Logger.LogDebug("'{commandName}' command invoked from correct channel", ctx.Command.Name);
        return true;
    }

    [Command("rule")]
    [Description("Generates a Rules post in the designed channel (by bot config).")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    public async Task Rule(
        CommandContext ctx,
        [Description("Title of the rule - Markdown formatting allowed")] string title,
        [Description("Text of the rule - Markdown formatting allowed")] string ruleText)
    {
        if (!IsChannelCorrect(ctx)) return;

        var welcomeChannel = ctx.Guild.GetChannel(IvaoItBot.Config!.WelcomeChannelId);
        await welcomeChannel.TriggerTypingAsync();
        await Task.Delay(3000);

        ;
        await welcomeChannel.SendMessageAsync(embed: await DiscordEmbedHelper.GetEmbed(ctx.Guild, title, ruleText));
        ctx.Client.Logger.LogInformation("Rule sent: {title}", title);
    }

    //[Obsolete("Usa il service")]
    //private async Task<DiscordEmbed> GetEmbed(CommandContext ctx, string title, string body) => new DiscordEmbedBuilder()
    //        .WithColor(new DiscordColor("#2a4982"))
    //        .WithTitle(title)
    //        .WithTimestamp(DateTime.Now)
    //        .WithDescription(body)
    //        .WithAuthor("IVAO Italia", @"https://www.ivao.it", ctx.Guild.IconUrl ?? (await ctx.Guild.GetMemberAsync(IvaoItBot.Config.BotUserId)).AvatarUrl);
}

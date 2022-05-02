using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands;

internal class RuleCommand : BaseCommandModule
{
    [Command("rule")]
    [Description("Generates a Rules post in the designed channel (by bot config).")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [InBotControlChannel]
    public async Task Rule(
        CommandContext ctx,
        [Description("Title of the rule - Markdown formatting allowed")] string title,
        [Description("Text of the rule - Markdown formatting allowed")] string ruleText)
    {
        var welcomeChannel = ctx.Guild.GetChannel(IvaoItBot.Config!.WelcomeChannelId);
        await welcomeChannel.TriggerTypingAsync();
        await Task.Delay(3000);

        ;
        await welcomeChannel.SendMessageAsync(embed: await DiscordEmbedHelper.GetAsync(ctx.Guild, title, ruleText));
        ctx.Client.Logger.LogInformation("Rule sent: {title}", title);
    }
}

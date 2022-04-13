using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands;

internal class BotCommands : IvaoBaseCommandModule
{
    [Command("rule")]
    [Description("Generates a Rules post in the designed channel (by bot config).")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    public async Task Rule(
        CommandContext ctx,
        [Description("Title of the rule - Markdown formatting allowed")] string title,
        [Description("Text of the rule - Markdown formatting allowed")] string ruleText)
    {
        if (!this.IsChannelCorrect(ctx)) return;

        var welcomeChannel = ctx.Guild.GetChannel(IvaoItBot.Config!.WelcomeChannelId);
        await welcomeChannel.TriggerTypingAsync();
        await Task.Delay(3000);

        ;
        await welcomeChannel.SendMessageAsync(embed: await DiscordEmbedHelper.GetEmbed(ctx.Guild, title, ruleText));
        ctx.Client.Logger.LogInformation("Rule sent: {title}", title);
    }
}

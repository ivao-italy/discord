using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Ivao.It.DiscordBot.SlashCommands;

public class IvaoSlashCommands : ApplicationCommandModule
{
    [SlashCommand("test", "A slash command made to test the DSharpPlusSlashCommands library!")]
#if DEBUG
    [RequireRoleId(771504168610955274)] //Admins
#else
    [RequireRoleId(426319218615517184)] //Staff
#endif
    public async Task TestCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource, 
            new DiscordInteractionResponseBuilder().WithContent($"Success!"));
    }
}

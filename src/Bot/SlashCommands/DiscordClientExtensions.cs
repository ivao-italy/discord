using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace Ivao.It.DiscordBot.SlashCommands;
internal static class DiscordClientExtensions
{
    internal static DiscordClient UseIvaoSlashCommands(this DiscordClient client)
    {
        var slash = client.UseSlashCommands();
        //slash.RegisterCommands<IvaoSlashCommands>();

        slash.SlashCommandErrored += async (s, e) =>
        {
            if (e.Exception is SlashExecutionChecksFailedException slex)
            {
                foreach (var check in slex.FailedChecks)
                    if (check is RequireRoleIdAttribute att)
                        await e.Context.CreateResponseAsync(
                            InteractionResponseType.ChannelMessageWithSource,
                            new DiscordInteractionResponseBuilder().WithContent(
                                $"You are not allowed to call this Slash Command. You must be a staff member!"));
            }
        };

        return client;
    }
}

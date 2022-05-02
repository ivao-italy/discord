using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.Commands.ClientEventHandlers;

internal class CommandsNextEventHandlers
{
    internal static async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        sender.Client.Logger.LogError(e.Exception, "CommandsNext error");

#if DEBUG
        var message = await DiscordEmbedHelper.GetErrorAsync(
            e.Context.Guild,
            "Ops!",
            $"There was an error...{Environment.NewLine}{e.Exception.Message}");
#else
        var message = await DiscordEmbedHelper.GetErrorAsync(
            e.Context.Guild,
            "Ops!",
            $"There was an error...{Environment.NewLine}{e.Exception.Message}");
#endif


        await e.Context.Channel.SendMessageAsync(message);
    }

    internal static async Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
    {
        sender.Client.Logger.LogInformation("Command '{Name}' executed.", e.Command.Name);
        await Task.CompletedTask;
    }
}
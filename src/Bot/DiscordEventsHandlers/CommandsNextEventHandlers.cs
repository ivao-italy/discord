using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Logging;

namespace Ivao.It.DiscordBot.DiscordEventsHandlers;

internal class CommandsNextEventHandlers
{
    internal async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        sender.Client.Logger.LogError(e.Exception, "CommandsNext error");
        await Task.CompletedTask;
    }

    internal async Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
    {
        sender.Client.Logger.LogInformation("Command '{Name}' executed.", e.Command.Name);
        await Task.CompletedTask;
    }
}
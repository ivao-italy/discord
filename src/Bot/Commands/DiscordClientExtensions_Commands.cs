using DSharpPlus;
using DSharpPlus.CommandsNext;
using Ivao.It.DiscordBot.Commands.ClientEventHandlers;
using Ivao.It.DiscordBot.Commands.EventsWorkflow;
using Ivao.It.DiscordBot.Commands.EventsWorkflow.ClientEventHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace Ivao.It.DiscordBot.Commands;

internal static class DiscordClientExtensions_Commands
{
    internal static DiscordClient UseIvaoCommands(this DiscordClient client, IServiceScopeFactory serviceScopeFactory)
    {
        var services = serviceScopeFactory.CreateScope().ServiceProvider;
        var commands = client.UseCommandsNext(new CommandsNextConfiguration
        {
            EnableMentionPrefix = true,
            Services = services
        });

        //Commands
        //commands.RegisterCommands(Assembly.GetExecutingAssembly()); //Events are not public.
        commands.RegisterCommands<RuleCommand>();
        commands.RegisterCommands<EventsCommands>();
        
        
        //Global Event Handlers
        var handlers = services.GetRequiredService<ReactionsClientEventHandlers>();
        commands.CommandExecuted += CommandsNextEventHandlers.Commands_CommandExecuted;
        commands.CommandErrored += CommandsNextEventHandlers.Commands_CommandErrored;
        client.MessageReactionAdded += handlers.OnEventPostOnMessageReactionAdded;

        return client;
    }
}
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Ivao.It.DiscordBot.ClientEventsHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Ivao.It.DiscordBot;

public class IvaoItBot
{
    public static DiscordConfig? Config { get; private set; }
    private CommandsNextExtension? Commands { get; set; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private DiscordClient? _client;


    public IvaoItBot(
        ILoggerFactory loggerFactory,
        IOptions<DiscordConfig> config,
        IServiceScopeFactory serviceScopeFactory)
    {
        _loggerFactory = loggerFactory;
        this._serviceScopeFactory = serviceScopeFactory;

        if (config == null) throw new ArgumentNullException(nameof(config));
        Config = config.Value;
    }

    public async Task RunAsync()
    {
        _client = new DiscordClient(new DiscordConfiguration
        {
            Token = Config!.DiscordToken,
            TokenType = TokenType.Bot,
            LoggerFactory = _loggerFactory,
            Intents = DiscordIntents.Guilds | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages | DiscordIntents.GuildMembers | DiscordIntents.ScheduledGuildEvents,
            AutoReconnect = true,
        });

        _client.Logger.LogInformation("Initializing IVAO IT Bot version {version}", Assembly.GetExecutingAssembly().GetName().Version?.ToString());

        //Commands
        this.Commands = _client.UseCommandsNext(new CommandsNextConfiguration
        {
            //StringPrefixes = new[] { "/" },
            EnableMentionPrefix = true,
        });
        this.Commands.RegisterCommands<BotCommands>();
        this.Commands.CommandExecuted += this.Commands_CommandExecuted;
        this.Commands.CommandErrored += this.Commands_CommandErrored;

        //Handlers
        _client.Ready += this.Client_Ready;
        _client.ClientErrored += this.Client_Errored;
        using var scope = _serviceScopeFactory.CreateScope();
        var handlers = scope.ServiceProvider.GetRequiredService<MessageCreatedEventHandlers>();
        _client.MessageCreated += handlers.UserActivation;
        _client.MessageCreated += handlers.EventPosted_MakeEvent;
        _client.MessageCreated += handlers.EventPosted_Crosspost;

        try
        {
            await _client.ConnectAsync();
            _client.Logger.LogWarning("Discord Client Connected");
        }
        catch (Exception ex) when (ex is UnauthorizedException || ex is BadRequestException || ex is ServerErrorException)
        {
            _client.Logger.LogError(ex, "Discord Client error");
        }
    }

    public async Task StopAsync()
    {
        await _client!.DisconnectAsync();
        _client.Logger.LogWarning("Discord Client Disconnected");
    }

    private async Task Client_Errored(DiscordClient sender, ClientErrorEventArgs e)
    {
        sender.Logger.LogError(e.Exception, "Discord Client Error");
        await Task.CompletedTask;
    }

    private async Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
    {
        sender.Logger.LogWarning("Bot started. Ready!");
#if DEBUG
        await sender.UpdateStatusAsync(new DiscordActivity($"IVAO Italy DEV {sender.VersionString}", ActivityType.Watching), UserStatus.Online);
#else
            await sender.UpdateStatusAsync(new DiscordActivity("IVAO Italy", ActivityType.Watching), UserStatus.Online);
#endif
        await Task.CompletedTask;
    }

    private async Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
    {
        sender.Client.Logger.LogError(e.Exception, "CommandsNext error");
        await Task.CompletedTask;
    }

    private async Task Commands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs e)
    {
        sender.Client.Logger.LogInformation("Command '{Name}' executed.", e.Command.Name);
        await Task.CompletedTask;
    }
}

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using Ivao.It.DiscordBot.DiscordEventsHandlers;
using Ivao.It.DiscordBot.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Ivao.It.DiscordBot;

/// <summary>
/// IVAO IT Bot object instance
/// </summary>
public class IvaoItBot
{
    /// <summary>
    /// Config read from json config data
    /// </summary>
    public static DiscordConfig? Config { get; private set; }
    private CommandsNextExtension? Commands { get; set; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IvaoItBotTasks _tasks;
    private DiscordClient? _client;

    /// <summary>
    /// Initialize a new bot instance
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="config"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <exception cref="ArgumentNullException"></exception>
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


    /// <summary>
    /// Runs the bot (with commands init and events handlers)
    /// </summary>
    /// <returns></returns>
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

        using var scope = _serviceScopeFactory.CreateScope();
        var commandsNextHandlers = scope.ServiceProvider.GetRequiredService<CommandsNextEventHandlers>();

        //Commands
        this.Commands = _client.UseCommandsNext(new CommandsNextConfiguration
        {
            //StringPrefixes = new[] { "/" },
            EnableMentionPrefix = true,
        });
        this.Commands.RegisterCommands<BotCommands>();
        this.Commands.CommandExecuted += commandsNextHandlers.Commands_CommandExecuted;
        this.Commands.CommandErrored += commandsNextHandlers.Commands_CommandErrored;

        //Handlers
        _client.Ready += this.Client_Ready;
        _client.ClientErrored += this.Client_Errored;
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

    /// <summary>
    /// Stops the bot execution
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        _tasks.Stop();
        _client!.Logger.LogWarning("Discord Schedule tasks stopped");

        await _client!.DisconnectAsync();
        _client.Logger.LogWarning("Discord Client Disconnected");
    }


    /// <summary>
    /// Checks if Guild has events ready to be started
    /// </summary>
    /// <returns></returns>
    internal async Task CheckEventsToStart()
    {
        if (_client == null) return;

        _client.Logger.LogInformation("CheckEventsToStart Invoked");

        var guild = _client.Guilds.Select(g => g.Value).SingleOrDefault();
        if (guild == null)
        {
            _client.Logger.LogWarning("CheckEventsToStart - Guild not found on the client");
            return;
        }

        foreach (var evt in await guild.GetEventsAsync())
        {
            if ((evt.StartTime - DateTime.Now) <= TimeSpan.Zero)
            {
                try
                {
                    await guild.StartEventAsync(evt);
                    _client.Logger.LogDebug("Started event {eventId} - {eventName}", evt.Id, evt.Name);
                }
                catch (InvalidOperationException ex)
                {
                    _client.Logger.LogError(ex, "Error startig event {eventId} - {eventName}", evt.Id, evt.Name);
                }
            }
        }
    }


    private async Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
    {
        sender.Logger.LogWarning("Bot started. Ready!");
#if DEBUG
        await sender.UpdateStatusAsync(new DiscordActivity($"IVAO Italy DEV {sender.VersionString}", ActivityType.Watching), UserStatus.Online);
#else
            await sender.UpdateStatusAsync(new DiscordActivity("IVAO Italy", ActivityType.Watching), UserStatus.Online);
#endif

        //Runs scheduled tasks
        _tasks = new IvaoItBotTasks(this);
        _tasks.Run();

        await Task.CompletedTask;
    }

    private async Task Client_Errored(DiscordClient sender, ClientErrorEventArgs e)
    {
        sender.Logger.LogError(e.Exception, "Discord Client Error");
        await Task.CompletedTask;
    }
}
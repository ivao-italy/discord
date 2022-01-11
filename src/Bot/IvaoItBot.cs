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
    internal DiscordClient? Client { get; private set; }

    private readonly ILoggerFactory _loggerFactory;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IvaoItBotTasks _tasks;

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
        Client = new DiscordClient(new DiscordConfiguration
        {
            Token = Config!.DiscordToken,
            TokenType = TokenType.Bot,
            LoggerFactory = _loggerFactory,
            Intents = DiscordIntents.Guilds | DiscordIntents.GuildMembers | DiscordIntents.GuildMessages | DiscordIntents.GuildMembers | DiscordIntents.ScheduledGuildEvents,
            AutoReconnect = true,
        });

        Client.Logger.LogInformation("Initializing IVAO IT Bot version {version}", Assembly.GetExecutingAssembly().GetName().Version?.ToString());

        using var scope = _serviceScopeFactory.CreateScope();
        var commandsNextHandlers = scope.ServiceProvider.GetRequiredService<CommandsNextEventHandlers>();

        //Commands
        this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration
        {
            //StringPrefixes = new[] { "/" },
            EnableMentionPrefix = true,
        });
        this.Commands.RegisterCommands<BotCommands>();
        this.Commands.CommandExecuted += commandsNextHandlers.Commands_CommandExecuted;
        this.Commands.CommandErrored += commandsNextHandlers.Commands_CommandErrored;

        //Handlers
        Client.Ready += this.Client_Ready;
        Client.ClientErrored += this.Client_Errored;
        var handlers = scope.ServiceProvider.GetRequiredService<MessageCreatedEventHandlers>();
        Client.MessageCreated += handlers.UserActivation;
        Client.MessageCreated += handlers.EventPosted_MakeEvent;
        Client.MessageCreated += handlers.EventPosted_Crosspost;

        try
        {
            await Client.ConnectAsync();
            Client.Logger.LogWarning("Discord Client Connected");
        }
        catch (Exception ex) when (ex is UnauthorizedException || ex is BadRequestException || ex is ServerErrorException)
        {
            Client.Logger.LogError(ex, "Discord Client error");
        }
    }

    /// <summary>
    /// Stops the bot execution
    /// </summary>
    /// <returns></returns>
    public async Task StopAsync()
    {
        _tasks.Stop();
        Client!.Logger.LogWarning("Discord Schedule tasks stopped");

        await Client!.DisconnectAsync();
        Client.Logger.LogWarning("Discord Client Disconnected");
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
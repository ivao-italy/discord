using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using Ivao.It.DiscordBot.Commands;
using Ivao.It.DiscordBot.DiscordEventsHandlers;
using Ivao.It.DiscordBot.ScheduledTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

    internal readonly IServiceScopeFactory ServiceScopeFactory;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IvaoItBotTasks _tasks;

    /// <summary>
    /// Initialize a new bot instance
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="config"></param>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="environment"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public IvaoItBot(
        ILoggerFactory loggerFactory,
        IOptions<DiscordConfig> config,
        IServiceScopeFactory serviceScopeFactory,
        IHostEnvironment environment)
    {
        _loggerFactory = loggerFactory;
        this.ServiceScopeFactory = serviceScopeFactory;

        if (config == null) throw new ArgumentNullException(nameof(config));
        Config = config.Value;

        _tasks = new IvaoItBotTasks(this, environment);
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
            Intents = DiscordIntents.Guilds | 
                        DiscordIntents.MessageContents | 
                        DiscordIntents.GuildMessages | 
                        DiscordIntents.GuildMembers | 
                        DiscordIntents.ScheduledGuildEvents | 
                        DiscordIntents.GuildMessageReactions,
            AutoReconnect = true
        });

        Client.Logger.LogInformation("Initializing IVAO IT Bot version {version}", Assembly.GetExecutingAssembly().GetName().Version?.ToString());

        using var scope = this.ServiceScopeFactory.CreateScope();
        
        //Commands
        Client.UseIvaoCommands(this.ServiceScopeFactory);

        //Interactivity
        Client.UseInteractivity(new InteractivityConfiguration {
            Timeout = TimeSpan.FromMinutes(2),
        });

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
        try
        {
            await _tasks.Stop();
            Client!.Logger.LogWarning("Discord Schedule tasks stopped");

            await Client!.DisconnectAsync();
            Client.Logger.LogWarning("Discord Client Disconnected");
        }
        catch (Exception e)
        {
            Client!.Logger.LogError(e, "Error stopping the bot");
            throw;
        }

    }


    private async Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
    {
        sender.Logger.LogWarning("Bot started. Ready!");
#if DEBUG
        var botVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString(3);
        await sender.UpdateStatusAsync(new DiscordActivity($"IVAO Italy DEV {botVersion}({sender.VersionString})", ActivityType.Watching), UserStatus.Online);
#else
        await sender.UpdateStatusAsync(new DiscordActivity("IVAO Italy", ActivityType.Watching), UserStatus.Online);
#endif

        //Runs scheduled tasks
        try
        {
#pragma warning disable CS4014
            _tasks.RunAsync();
#pragma warning restore CS4014
            sender.Logger.LogWarning("Bot scheduled tasks started.");
        }
        catch (Exception ex)
        {
            sender.Logger.LogError(ex, "Error starting Bot scheduled tasks.");
            throw;
        }
    }

    private async Task Client_Errored(DiscordClient sender, ClientErrorEventArgs e)
    {
        sender.Logger.LogError(e.Exception, "Discord Client Error");
        await Task.CompletedTask;
    }
}
namespace Ivao.It.DiscordBot.Service;

public class BotWorker : BackgroundService
{
    private readonly ILogger<BotWorker> _logger;
    private readonly IvaoItBot _bot;

    public BotWorker(ILogger<BotWorker> logger, IvaoItBot bot)
    {
        _logger = logger;
        _bot = bot;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting IVAO IT Discord Bot at: {time}", DateTimeOffset.Now);

        await _bot.RunAsync();
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping IVAO IT Discord Bot at: {time}", DateTimeOffset.Now);
        await _bot.StopAsync();

        await base.StopAsync(cancellationToken);
    }
}

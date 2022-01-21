using Ivao.It.DiscordBot;
using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordBot.Service;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;

IHost host = Host.CreateDefaultBuilder(args)
#if DEBUG
    .ConfigureAppConfiguration(conf =>
    {
        conf.AddUserSecrets(Assembly.GetExecutingAssembly());
    })
#endif
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<BotWorker>();
        services.AddIvaoItBot(hostContext.Configuration.GetSection("DiscordConfig"));
        services.AddDbContextFactory<DiscordDbContext>(opt =>
        {
            var connectionString = hostContext.Configuration.GetConnectionString("DiscordBot");
            opt.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });
    })
     .ConfigureLogging((hostContext, logBuilder) =>
     {
         var logger = new LoggerConfiguration()
                  .ReadFrom.Configuration(hostContext.Configuration)
                  .CreateLogger();
         logBuilder.AddSerilog(logger, dispose: true);
     })
    .Build();

await host.RunAsync();

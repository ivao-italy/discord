namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal class CheckEventsToStart : BaseScheduledTask
{
    public override void Run(IvaoItBot bot) 
        =>  this.Timer = new Timer(async _ => await bot.CheckEventsToStart(), null, StartDelay, Period);
    
}
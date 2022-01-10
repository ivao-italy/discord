namespace Ivao.It.DiscordBot.ScheduledTasks.Tasks;

internal class DeletePastEventsPost : BaseScheduledTask
{
    public override void Run(IvaoItBot bot) => 
        this.Timer = new Timer(async _ => await bot.DeletePastEventsPost(), null, StartDelay, Period);
}
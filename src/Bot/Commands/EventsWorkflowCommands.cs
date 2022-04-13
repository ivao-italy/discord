using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;


namespace Ivao.It.DiscordBot.Commands;

internal class EventsWorkflowCommands : IvaoBaseCommandModule
{
    [Command("addevent")]
    [Description("Schedules a new Event to build the appropriate checklist for all the staff members involved")]
    [RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    public async Task AddEvent(
        CommandContext ctx,
        [Description("Title of the event - Markdown formatting allowed")] string title,
        [Description("Date of the event start - In ICAO FPL DOF format: yyyymmdd")] string date,
        [Description("Tasks needed: rfe, banner, routes. Add all the needed taks in a comma separated value string. Eg: 'rfe,banner'")] string tasks,
        [Description("Forum link - the link of the FS topic. Optional.")] string? forumLink = null
        )
    {
        if (!this.IsChannelCorrect(ctx)) return;

        var dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        var tasksArray = tasks
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => s.ToEventsTasks());

        //Validations
        //Can be parametrized to the first deadline?
        if ((dt - DateTime.UtcNow).TotalDays < 30)
        {
            throw new IvaoItBotBusinessException(
                "Unable to schedule an event later than 30 days before the event itself.");
        }

        var dbTasks = tasksArray.Select(t => new EventTask { TaskTypeId = (short)t }).ToList();
        if(string.IsNullOrWhiteSpace(forumLink))
            dbTasks.Add(new EventTask {TaskTypeId = (short)EventsTasks.ForumTopic});
        dbTasks.Add(new EventTask { TaskTypeId = (short)EventsTasks.Announcement });
        dbTasks.Add(new EventTask { TaskTypeId = (short)EventsTasks.AnnouncementSocial });
        var evt = new Event {
            CreatedByUserId = ctx.User.Id,
            Date = dt,
            Name = title,
            Link = forumLink,
            Tasks = dbTasks.ToList()
        };

        var db = ctx.CommandsNext.Services.GetRequiredService<DiscordDbContext>();
        await db.Events.AddAsync(evt);
        await db.SaveChangesAsync();
    }
}

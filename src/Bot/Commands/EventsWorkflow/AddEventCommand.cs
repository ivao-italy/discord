using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Ivao.It.DiscordBot.Commands.EventsWorkflow.Attributes;
using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Ivao.It.DiscordBot.Commands.EventsWorkflow;

internal partial class EventsCommands
{
    [Command("add")]
    [Description("Schedules a new Event to build the appropriate checklist for all the staff members involved")]
    //[RequirePermissions(DSharpPlus.Permissions.ManageChannels)]
    [InEventsChannelOnly]
    public async Task Add(
        CommandContext ctx,
        [Description("Title of the event - Markdown formatting allowed")] string title,
        [Description("Date of the event start - In format: yyyymmdd")] string date,
        [Description("Tasks needed: rfe, banner, routes. Add all the needed taks in a comma separated value string. Eg: 'rfe,banner'")] string tasks,
        [Description("Forum link - the link of the FS topic. Optional.")] string? forumLink = null
        )
    {
        //Data parsing - removes duplicates using hashset
        var dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        var tasksArray = tasks
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => s.ToEventsTasks())
            .ToHashSet();

        //Tasks always present
        tasksArray.Add(EventsTasks.Announcement);
        tasksArray.Add(EventsTasks.AnnouncementSocial);
        tasksArray.Add(EventsTasks.Atcs);

        //var taskTypes = await this._service.GetAllTaskTypes();
        //taskTypes = taskTypes.Where(t => tasksArray.Contains((EventsTasks)t.Id)).ToList();

        //Validations
        //Can be parametrized to the first deadline?
        if ((dt - DateTime.UtcNow).TotalDays < 30)
        {
            ctx.Client.Logger.LogWarning(
                "Event creation requested before 30 days from the event: {event} - {date} - {user}",
                title,
                date,
                ctx.Member?.Nickname ?? ctx.User.Username);

            throw new IvaoItBotBusinessException(
                "Unable to schedule an event later than 30 days before the event itself.");
        }

        //Store the event
        try
        {
            var eventId = await _service.AddEventAsync(ctx.User.Id, title, dt, tasksArray, forumLink);

            //Success Message
            var evt = await _service.GetAsync(eventId);
            await evt!.ReplyWithEvent(ctx.Client, ctx.Guild, ctx.Message, true);
        }
        catch (Exception e)
        {
            ctx.Client.Logger.LogError(e, "Error creating new event: {event} - {date} - {user}",
                title,
                date,
                ctx.Member?.Nickname ?? ctx.User.Username);
            throw;
        }
    }
}

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Ivao.It.DiscordBot.Services;
using System.Globalization;
using System.Reflection.Metadata;


namespace Ivao.It.DiscordBot.Commands;

internal class EventsWorkflowCommands : IvaoBaseCommandModule
{
    private readonly EventsService _service;

    public EventsWorkflowCommands(EventsService service)
    {
        _service = service;
    }

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

        //Data parsing
        var dt = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
        var tasksArray = tasks
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(s => s.ToEventsTasks())
            .ToList();

        //Tasks always present
        tasksArray.Add(EventsTasks.Announcement);
        tasksArray.Add(EventsTasks.AnnouncementSocial);
        tasksArray.Add(EventsTasks.Atcs);

        //Validations
        //Can be parametrized to the first deadline?
        if ((dt - DateTime.UtcNow).TotalDays < 30)
        {
            throw new IvaoItBotBusinessException(
                "Unable to schedule an event later than 30 days before the event itself.");
        }

        //Store the event
        //TODO Error management + logging
        var eventId = await _service.AddEventAsync(ctx.User.Id, title, dt, tasksArray, forumLink);

        //Success Message
        var evt = await _service.GetEvent(eventId);
        var embed = await new DiscordEmbedBuilder().CreateEventAsync(evt!, ctx.Guild);
        await ctx.RespondAsync(embed);
    }
}

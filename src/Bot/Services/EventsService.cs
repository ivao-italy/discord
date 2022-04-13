using Ivao.It.DiscordBot.Data;
using Ivao.It.DiscordBot.Data.Entities;
using Ivao.It.DiscordBot.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace Ivao.It.DiscordBot.Services;

internal class EventsService
{
    private readonly DiscordDbContext _db;

    public EventsService(DiscordDbContext ctx)
    {
        _db = ctx;
    }

    public async Task<int> AddEventAsync(ulong userId, string title, DateTime date, IEnumerable<EventsTasks> tasks, string? forumLink = null)
    {
        var dbTasks = tasks.Select(t => new EventTask { TaskTypeId = (short)t }).ToList();
        if (string.IsNullOrWhiteSpace(forumLink))
            dbTasks.Add(new EventTask { TaskTypeId = (short)EventsTasks.ForumTopic });

        var evt = new Event
        {
            CreatedByUserId = userId,
            Date = date,
            Name = title,
            Link = forumLink,
            Tasks = dbTasks.ToList()
        };

        await _db.Events.AddAsync(evt);
        await _db.SaveChangesAsync();

        return evt.Id;
    }

    public async Task<Event?> GetEvent(int id) =>
        await _db.Events.AsNoTracking()
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TaskType)
            .SingleOrDefaultAsync(e=>e.Id == id);
}

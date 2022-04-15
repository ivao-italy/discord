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

    public async Task<Event?> GetAsync(int id) =>
        await _db.Events.AsNoTracking()
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TaskType)
            .SingleOrDefaultAsync(e => e.Id == id);

    public async Task<ICollection<Event>> GetAsync() =>
        await _db.Events.AsNoTracking()
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TaskType)
            .Where(e => e.Date >= DateTime.UtcNow)
            .ToListAsync();

    public async Task<ICollection<Event>> GetUpcomingWithToDosAsync() =>
        await _db.Events.AsNoTracking()
            .Include(e => e.Tasks)
            .ThenInclude(e => e.TaskType)
            .Where(e => e.Tasks.Any(t => !t.CompletedAt.HasValue))
            .ToListAsync();

    public async Task CompleteTaskAsync(EventsTasks task, int @event, ulong userId, string? content)
    {
        var taskTypeId = (short)task;
        var toUpdate = await _db.EventTasks.SingleAsync(t => t.EventId == @event && t.TaskTypeId == taskTypeId);
        toUpdate.CompletedBy = userId;
        toUpdate.CompletedAt = DateTime.UtcNow;
        toUpdate.Content = content;

        if (task == EventsTasks.ForumTopic)
        {
            toUpdate.Event.Link = content;
        }

        await _db.SaveChangesAsync();
    }

    public async Task<bool> IsTaskCompleted(EventsTasks task, int @event) 
        => _db.EventTasks.AsNoTracking().Any(t => t.TaskTypeId == (short)task && t.CompletedBy == null);

    public async Task DeleteEventAsync(int eventId)
    {
        _db.Events.Remove(new Event { Id = eventId });
        await _db.SaveChangesAsync();
    }
}

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

    /// <summary>
    /// Gets all the tasks types available
    /// </summary>
    /// <returns></returns>
    //TODO can be cached
    public async Task<IEnumerable<EventTaskType>> GetAllTaskTypes() 
        => await this._db.EventTaskTypes.AsNoTracking().ToListAsync();

    /// <summary>
    /// Adds a new event
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="title"></param>
    /// <param name="date"></param>
    /// <param name="tasks"></param>
    /// <param name="forumLink"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Gets an existing event
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<Event?> GetAsync(int id) =>
        await _db.Events.AsNoTracking()
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TaskType)
            .SingleOrDefaultAsync(e => e.Id == id);

    /// <summary>
    /// Gets all the upcoming events 
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<Event>> GetAsync() =>
        await _db.Events.AsNoTracking()
            .Include(p => p.Tasks)
            .ThenInclude(t => t.TaskType)
            .Where(e => e.Date >= DateTime.UtcNow)
            .ToListAsync();

    /// <summary>
    /// Gets upcoming events with tasks expired
    /// </summary>
    /// <returns></returns>
    public async Task<ICollection<Event>> GetUpcomingWithToDosExpiredAsync() =>
        await _db.Events.AsNoTracking()
            .Include(e => e.Tasks)
            .ThenInclude(e => e.TaskType)
            .Where(e => e.Date >= DateTime.UtcNow)
            .Where(e => e.Tasks.Any(t => !t.CompletedAt.HasValue))
            .Where(e => e.Tasks.Any(t => DateTime.UtcNow >= e.Date.AddDays(-t.TaskType.DaysBefore)))
            .ToListAsync();

    /// <summary>
    /// Completes a task
    /// </summary>
    /// <param name="task"></param>
    /// <param name="event"></param>
    /// <param name="userId"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    /// <exception cref="IvaoItBotBusinessException"></exception>
    public async Task CompleteTaskAsync(EventsTasks task, int @event, ulong userId, string? content)
    {
        var taskTypeId = (short)task;
        var toUpdate = await _db.EventTasks.SingleAsync(t => t.EventId == @event && t.TaskTypeId == taskTypeId);
        if (toUpdate.CompletedAt.HasValue)
        {
            throw new IvaoItBotBusinessException("Sorry! That task has already been completed.");
        }
        toUpdate.CompletedBy = userId;
        toUpdate.CompletedAt = DateTime.UtcNow;
        toUpdate.Content = content;

        if (task == EventsTasks.ForumTopic)
        {
            toUpdate.Event.Link = content;
        }

        await _db.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if task is completed
    /// </summary>
    /// <param name="task"></param>
    /// <param name="event"></param>
    /// <returns></returns>
    public async Task<bool> IsTaskCompleted(EventsTasks task, int @event) 
        => await _db.EventTasks.AsNoTracking().AnyAsync(t => t.TaskTypeId == (short)task && t.CompletedBy == null);

    /// <summary>
    /// Deletes an event and all the references
    /// </summary>
    /// <param name="eventId"></param>
    /// <returns></returns>
    public async Task DeleteEventAsync(int eventId)
    {
        var ev = await this._db.Events.SingleOrDefaultAsync(e => e.Id == eventId);
        if (ev == null) return;
        _db.Events.Remove(ev);
        await _db.SaveChangesAsync();
    }
}

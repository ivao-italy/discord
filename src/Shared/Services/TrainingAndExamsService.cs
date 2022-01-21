using Ivao.It.Discord.Shared.Models;
using Ivao.It.DiscordBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Ivao.It.Discord.Shared.Services;

public class TrainingAndExamsService : ITrainingAndExamsService
{
    private readonly ILogger<TrainingAndExamsService> _logger;

    public TrainingAndExamsService(ILogger<TrainingAndExamsService> logger)
    {
        _logger = logger;
    }
    public async Task<IEnumerable<IvaoScheduledEvent>> GetPlannedAsync(DiscordDbContext db, DateOnly day)
    {
        var results = new List<IvaoScheduledEvent>();

        results.AddRange(await db.Exams
            .Where(e => e.PlannedDate == day)
            .Select(e => new IvaoScheduledEvent
            {
                Airport = e.Airport,
                Facility = e.Facility,
                Planned = new DateTime(e.PlannedDate.Year, e.PlannedDate.Month, e.PlannedDate.Day, e.PlannedTime.Hour, e.PlannedTime.Minute, 0),
                Rating = (AtcRating)e.Rating,
                Type = EventType.Exam
            }).ToListAsync());

        results.AddRange(await db.Trainings
            .Where(e => e.PlannedDate == day)
            .Select(e => new IvaoScheduledEvent
            {
                Airport = e.Airport,
                Facility = e.Facility,
                Planned = new DateTime(e.PlannedDate.Year, e.PlannedDate.Month, e.PlannedDate.Day, e.PlannedTime.Hour, e.PlannedTime.Minute, 0),
                Rating = (AtcRating)e.Rating,
                Type = EventType.Training
            }).ToListAsync());

        return results;
    }
}

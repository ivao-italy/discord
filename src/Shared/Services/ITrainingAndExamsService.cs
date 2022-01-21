using Ivao.It.Discord.Shared.Models;
using Ivao.It.DiscordBot.Data;

namespace Ivao.It.Discord.Shared.Services;

public interface ITrainingAndExamsService
{
    Task<IEnumerable<IvaoScheduledEvent>> GetPlannedAsync(DiscordDbContext db, DateOnly day);
}

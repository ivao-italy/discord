using Ivao.It.DiscordBot.Data.EfExtension;
using Ivao.It.DiscordBot.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Ivao.It.DiscordBot.Data;

public class DiscordDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Training> Trainings { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventTask> EventTasks { get; set; }
    public DbSet<EventTaskType> EventTaskTypes { get; set; }



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DiscordDbContext(DbContextOptions<DiscordDbContext> options)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
            : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }


    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.EnsureReadonlyNotChanged();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }
    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        this.EnsureReadonlyNotChanged();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ivao.It.DiscordBot.Data.Entities;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public string? Link { get; set; }
    public ulong CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual ICollection<EventTask> Tasks { get; set; }
}


public class EventTypeConfig : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(255);

        builder.Property(p => p.Link)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .HasDefaultValueSql("NOW()");

        builder.HasMany(e => e.Tasks)
            .WithOne(t => t.Event)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

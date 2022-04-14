
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;

namespace Ivao.It.DiscordBot.Data.Entities;

[DebuggerDisplay("{" + nameof(TaskType) + "}")]
public class EventTask
{
    public int Id { get; set; }
    public short TaskTypeId { get; set; }
    public int EventId { get; set; }
    public ulong? CompletedBy { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? Content { get; set; }

    public virtual EventTaskType TaskType { get; set; }
    public virtual Event Event { get; set; }
}


public class EventTaskTypeConfig : IEntityTypeConfiguration<EventTask>
{
    public void Configure(EntityTypeBuilder<EventTask> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(p => p.Content)
               .HasMaxLength(255);

        builder.HasOne(p => p.TaskType)
               .WithMany()
               .HasForeignKey(p => p.TaskTypeId)
               .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(i => new { i.EventId, i.TaskTypeId })
            .IsUnique();
    }
}

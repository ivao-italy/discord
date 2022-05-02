using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ivao.It.DiscordBot.Data.Entities;

public class EventTaskType
{
    public short Id { get; set; }
    public string Description { get; set; }
    public short DaysBefore { get; set; }
    public ulong StaffGroupToNofify { get; set; }

    public override string ToString() => this.Description;
}


public class EventTaskType_TypeConfig : IEntityTypeConfiguration<EventTaskType>
{
    public void Configure(EntityTypeBuilder<EventTaskType> builder)
    {
        builder.HasKey(k => k.Id);
        
        builder.Property(k => k.Description)
            .IsUnicode(false)
            .HasMaxLength(25);

        builder.HasData(
            new EventTaskType { Id = 1, Description = "Web Booking Tool", DaysBefore = 30, StaffGroupToNofify = 621792089700696064 },
            new EventTaskType { Id = 2, Description = "Forum Topic", DaysBefore = 20, StaffGroupToNofify = 621788428832342036 },
            new EventTaskType { Id = 3, Description = "Routes", DaysBefore = 15, StaffGroupToNofify = 621792026375094283 },
            new EventTaskType { Id = 4, Description = "Graphics", DaysBefore = 15, StaffGroupToNofify = 963367786715947048 },
            new EventTaskType { Id = 5, Description = "Announcement", DaysBefore = 10, StaffGroupToNofify = 621788428832342036 },
            new EventTaskType { Id = 6, Description = "Announcement Social", DaysBefore = 10, StaffGroupToNofify = 541216766924423169 },
            new EventTaskType { Id = 7, Description = "ATC Bookings", DaysBefore = 2, StaffGroupToNofify = 541216766924423169 }
        );
    }
}

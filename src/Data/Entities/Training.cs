using Ivao.It.DiscordBot.Data.EfExtension;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ivao.It.DiscordBot.Data.Entities;

public class Training : BaseExamTraining
{
}

public class TrainingConfiguration : IEntityTypeConfiguration<Training>
{
    public void Configure(EntityTypeBuilder<Training> builder)
    {
        builder.IsReadOnly();
        builder.ToView("trainingslist");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Airport).HasMaxLength(4).IsRequired();
        builder.Property(x => x.Rating).HasColumnName("rating").IsRequired();
        builder.Property(x => x.PlannedDate).HasColumnName("planned_date").IsRequired();
        builder.Property(x => x.PlannedTime).HasColumnName("planned_hour").IsRequired();
    }
}

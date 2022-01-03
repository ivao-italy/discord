using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics;

namespace Ivao.It.DiscordBot.Data.Entities;

[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class User
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public string Vid { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public DateTime? GdprAcceptDate { get; set; }
    public ulong? DiscordUserId { get; set; }

    public override string ToString() => $"{Vid} {LastName}";

    private string GetDebuggerDisplay() => $"{this} {FirstName}";
}



public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Vid);
        builder.Property(x => x.Vid).HasMaxLength(6);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(70);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(70);
    }
}

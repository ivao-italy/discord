using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Primitives;

namespace Ivao.It.DiscordBot.Data.EfExtension;

internal static class EntitiesAnnotations
{
    public const string ReadOnly = "ReadOnly";
}


internal static class EntityTypeBuilderExtension
{
    public static EntityTypeBuilder<TEntity> IsReadOnly<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : class
    {
        builder.HasAnnotation(EntitiesAnnotations.ReadOnly, EntitiesAnnotations.ReadOnly);
        return builder;
    }
}


internal static class DbContextExtesions
{
    internal static void EnsureReadonlyNotChanged(this DbContext db)
    {
        var modifiedReadOnlyEntries = db.ChangeTracker.Entries()
            .Where(e => e.State != EntityState.Unchanged && e.Metadata.FindAnnotation(EntitiesAnnotations.ReadOnly) != null);

        if (modifiedReadOnlyEntries.Any())
        {
            StringValues errors = new(modifiedReadOnlyEntries.Select(s => $"{s.Entity} - State {s.State}").ToArray());
            throw new InvalidOperationException($"Entities state not compatible with an entity configured as ReadOnly.{Environment.NewLine}{errors}");
        }
    }
}

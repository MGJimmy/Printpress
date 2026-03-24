using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Printpress.Infrastructure
{
    public static class EntityTypeBuilderExtensions
    {
        public static void SetSchemaTable<TEntity>(
            this EntityTypeBuilder<TEntity> entity,
            string? schema) where TEntity : class
        {
            entity.ToTable(typeof(TEntity).Name + "s", schema);
        }
    }
}

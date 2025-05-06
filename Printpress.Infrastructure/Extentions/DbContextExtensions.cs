using Microsoft.EntityFrameworkCore;
using Printpress.Domain;
using Printpress.Domain.Enums;
using System.Data;
using System.Dynamic;

namespace Printpress.Infrastructure;

public static class DbContextExtensions
{
    public static void ChangeTrackedEntityStates(this DbContext db, ITrackedEntity entity)
    {
        var properties = entity.GetType().GetProperties();

        foreach (var propertyInfo in properties)
        {
            if (propertyInfo.GetValue(entity) is ITrackedEntity trackedEntity)
            {
                db.ChangeTrackedEntityStates(trackedEntity);
            }

            if (propertyInfo.GetValue(entity) is IEnumerable<ITrackedEntity> collection)
            {
                foreach (var item in collection)
                {
                    db.ChangeTrackedEntityStates(item);
                }
            }
        }

        var entry = db.Entry(entity);

        entry.State = entity.ObjectState switch
        {
            TrackingState.Added => EntityState.Added,
            TrackingState.Modified => EntityState.Modified,
            TrackingState.Deleted => EntityState.Deleted,
            TrackingState.Unchanged => EntityState.Unchanged,
            _ => entry.State
        };

    }

    public static IEnumerable<dynamic> DynamicListFromSql(this DbContext db, string sql)
    {
        using var cmd = db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = sql;
        if (cmd.Connection.State != ConnectionState.Open)
        {
            cmd.Connection.Open();
        }

        using var dataReader = cmd.ExecuteReader();
        while (dataReader.Read())
        {
            var row = new ExpandoObject() as IDictionary<string, object>;
            for (var fieldCount = 0; fieldCount < dataReader.FieldCount; fieldCount++)
            {
                row.Add(dataReader.GetName(fieldCount), dataReader[fieldCount]);
            }

            yield return row;
        }
    }
}

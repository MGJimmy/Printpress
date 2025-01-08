using Microsoft.EntityFrameworkCore;
using Printpress.Domain.Enums;
using System.Data;
using System.Dynamic;
using Printpress.Domain;

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

        if (entity.State == TrackingState.Added)
        {
            entry.State = EntityState.Added;
        }
        else if (entity.State == TrackingState.Modified)
        {
            entry.State = EntityState.Modified;
        }
        else if (entity.State == TrackingState.Unchanged)
        {
            entry.State = EntityState.Unchanged;
        }
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
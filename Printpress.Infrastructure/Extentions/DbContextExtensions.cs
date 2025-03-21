using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Printpress.Domain;
using Printpress.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
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

    
    
    public static void ChangeTrackedEntityStates_Jimmy(this DbContext db, IEntity entity)
    {
        var properties = entity.GetType().GetProperties();

        foreach (var propertyInfo in properties)
        {
            var propertyEntity = propertyInfo.GetValue(entity);

            if (propertyEntity is ITrackedEntity)
            {
                db.ChangeTrackedEntityStates((IEntity) propertyEntity);
            }

            if (propertyEntity is IEnumerable<ITrackedEntity>)
            {
                foreach (var item in (IEnumerable<IEntity>) propertyEntity)
                {
                    db.ChangeTrackedEntityStates(item);
                }
            }
        }

        // Handle sof delete
        if (entity is ISoftDelete_jimmy && entity.State == TrackingState.Deleted)
        {
            EntityEntry<ISoftDelete_jimmy> softDeleteEntry = db.Entry((ISoftDelete_jimmy)entity);
            softDeleteEntry.Entity.IsDeleted = true;
            softDeleteEntry.State = EntityState.Modified;
            return;
        }

        var entry = db.Entry(entity);
        entry.State = entity.State switch
        {
            TrackingState.Added => EntityState.Added,
            TrackingState.Modified => EntityState.Modified,
            TrackingState.Deleted => EntityState.Deleted,
            TrackingState.Unchanged => EntityState.Unchanged,
            _ => entry.State
        };

    }

}

public interface IEntity : ITrackedEntity
{
}

public interface ISoftDelete_jimmy : IEntity
{
    public bool IsDeleted { get; set; }
}
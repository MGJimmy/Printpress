namespace Printpress.Application;

public abstract class BaseMapper <TSource, TDestination>
{
    public List<TSource> MapFromDestinationToSource(List<TDestination> destinationEntities)
    {
        if (destinationEntities is null)
        {
            return new List<TSource>();       
        }


        List<TSource> list = new List<TSource>();
        {
            foreach (TDestination destinationEntity in destinationEntities)
            {
                TSource val = MapFromDestinationToSource(destinationEntity);
                if (val != null)
                {
                    list.Add(val);
                }
            }

            return list;
        }
        
    }

    public List<TDestination> MapFromSourceToDestination(List<TSource> sourceEntities)
    {
        if (sourceEntities is null)
        {
            return new List<TDestination>();
        }


        List<TDestination> list = new List<TDestination>();
        {
            foreach (TSource sourceEntity in sourceEntities)
            {
                TDestination val = MapFromSourceToDestination(sourceEntity);
                if (val != null)
                {
                    list.Add(val);
                }
            }

            return list;
        }
    }

    public abstract TSource MapFromDestinationToSource(TDestination destinationEntity);

    public abstract TDestination MapFromSourceToDestination(TSource sourceEntity);
}

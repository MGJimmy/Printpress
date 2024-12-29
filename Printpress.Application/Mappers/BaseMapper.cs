namespace Printpress.Application;

public abstract class BaseMapper <TSource, TDestination>
{
    public List<TSource> MapFromDestinationToSource(List<TDestination> destinationEntities)
    {
        if (destinationEntities != null)
        {
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

        return null;
    }

    public List<TDestination> MapFromSourceToDestination(List<TSource> sourceEntities)
    {
        if (sourceEntities != null)
        {
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

        return null;
    }

    public abstract TSource MapFromDestinationToSource(TDestination destinationEntity);

    public abstract TDestination MapFromSourceToDestination(TSource sourceEntity);
}

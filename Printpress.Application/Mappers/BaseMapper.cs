namespace Printpress.Application;

public abstract class BaseMapper<TSource, TDestination> where TDestination : class where TSource : class
{
    public PagedList<TDestination> MapFromSourceToDestination(PagedList<TSource> destinationEntities)
    {
        if (destinationEntities is null)
        {
            return new PagedList<TDestination>();
        }

        return new PagedList<TDestination>
        {
            PageNumber = destinationEntities.PageNumber,
            PageSize = destinationEntities.PageSize,
            TotalCount = destinationEntities.TotalCount,
            Items = MapFromSourceToDestination(destinationEntities.Items)
        }; ;


    }

    public List<TSource> MapFromDestinationToSource(IEnumerable<TDestination> destinationEntities)
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

    public List<TDestination> MapFromSourceToDestination(IEnumerable<TSource> sourceEntities)
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

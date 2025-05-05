using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Printpress.Domain;
using Printpress.Domain.Enums;

namespace Printpress.Application
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> NotDeleted<T>(this IEnumerable<T> source) where T : ITrackedEntity
        {
            return source.Where(x => x.State != TrackingState.Deleted);
        }
    }
}

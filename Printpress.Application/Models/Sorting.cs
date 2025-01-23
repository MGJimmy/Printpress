using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Application
{
    public class Sorting
    {
        public Sorting()
        {

        }
        public Sorting(string field, SortingDirection dir)
        {
            Field = field;
            Dir = dir;

        }
        public string Field { get; set; }

        public SortingDirection Dir { get; set; }
    }
}

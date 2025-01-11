using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printpress.Domain.Entities
{
    public class ItemDetails : Entity
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        public string Key { get; set; }
        public string Value { get; set; }

        public virtual Item Item { get; set; }
    }
}

using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class Service : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public ServiceCategoryEnum ServiceCategory { get; set; }
    }
}

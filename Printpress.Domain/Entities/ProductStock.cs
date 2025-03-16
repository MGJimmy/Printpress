using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class ProductStock : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public StockCategoryEnum StockCategory { get; set; }
    }
}

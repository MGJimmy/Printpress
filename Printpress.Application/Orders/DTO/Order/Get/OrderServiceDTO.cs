namespace Printpress.Application
{
    public class OrderServiceDTO
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }
    }
}

namespace Printpress.Application
{
    public class OrderServiceDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ServiceId { get; set; }
        public decimal? Price { get; set; }
    }
}

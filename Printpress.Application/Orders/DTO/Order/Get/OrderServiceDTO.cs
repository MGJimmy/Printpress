namespace Printpress.Application
{
    public class OrderServiceDTO
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal? Price { get; set; }
        public bool IsCover { get; set; }
    }
}

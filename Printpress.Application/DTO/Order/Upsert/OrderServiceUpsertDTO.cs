namespace Printpress.Application
{
    public class OrderServiceUpsertDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public decimal? Price { get; set; }
    }
}

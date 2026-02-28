namespace Printpress.Application
{
    public class OrderGroupServiceUpsertDTO : TrackedDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
    }
}

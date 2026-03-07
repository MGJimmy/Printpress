namespace Printpress.Application
{
    public class OrderGroupServiceUpsertDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
    }
}

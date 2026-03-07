namespace Printpress.Application
{
    public class OrderGroupServiceDTO : TrackedDTO
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Guid OrderGroupId { get; set; }
        public string ServiceName { get; set; }
    }
}

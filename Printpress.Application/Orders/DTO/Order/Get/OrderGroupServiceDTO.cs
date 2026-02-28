namespace Printpress.Application
{
    public class OrderGroupServiceDTO : TrackedDTO
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public int OrderGroupId { get; set; }
        public string ServiceName { get; set; }
    }
}

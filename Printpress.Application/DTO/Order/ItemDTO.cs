namespace Printpress.Application
{
    public class ItemDTO //:IObjectState
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int GroupId { get; set; }
        public string ObjectState { get; set; }

    }
}

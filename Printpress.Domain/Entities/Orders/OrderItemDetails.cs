
namespace Printpress.Domain
{
    public class OrderItemDetails : Entity , ISoftDelete
    {
        public Guid ItemId { get; set; }
        public bool IsDeleted { get; set; }

        public ItemDetailsKeyEnum ItemDetailsKey
        {
            get { return (ItemDetailsKeyEnum)ItemDetailsKeyId; }
            set { ItemDetailsKeyId = (int)value; }
        }

        public string Value { get; set; }
        public int ItemDetailsKeyId { get; set; }


        public virtual OrderItem Item { get; set; }
        public virtual ItemDetailsKey_LKP ItemDetailsKey_LKP { get; set; }
    }
}

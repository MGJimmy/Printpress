using Printpress.Domain.Enums;

namespace Printpress.Domain.Entities
{
    public class ItemDetails : Entity
    {
        public int Id { get; set; }
        public int ItemId { get; set; }

        public ItemDetailsKeyEnum ItemDetailsKey
        {
            get { return (ItemDetailsKeyEnum)ItemDetailsKeyId; }
            set { ItemDetailsKeyId = (int)value; }
        }

        public string Value { get; set; }
        public int ItemDetailsKeyId { get; set; }


        public virtual Item Item { get; set; }
        public virtual ItemDetailsKey_LKP ItemDetailsKey_LKP { get; set; }
    }
}

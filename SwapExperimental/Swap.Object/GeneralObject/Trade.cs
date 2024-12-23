using Swap.Object.Items;
using System.Runtime.Serialization;

namespace Swap.Object.GeneralObjects
{
    [DataContract]
    public class Trade
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int OfferedById { get; set; }
        [DataMember]
        public int OfferedToId { get; set; }
        [DataMember]
        public int ItemId { get; set; }
        [DataMember]
        public string ItemsToTrade { get; private set; }
        [DataMember]
        public int? Status { get; private set; }

        public User OfferedTo { get; set; }

        public User OfferedBy { get; set; }

        public Item OfferedItem { get; set; }


        public void SetForeignKeys(User offerTo, User offeredBy, Item offeredItem)
        {
            OfferedTo = offerTo;
            OfferedBy = offeredBy;
            OfferedItem = offeredItem;
        }

        public void WipeForeignKeys()
        {
            OfferedTo = null;
            OfferedBy = null;
            OfferedItem = null;
        }

        public void SetStatus(int status) => Status = status;
    }
}

using System.Runtime.Serialization;

namespace Swap.Object.Items
{
    [DataContract]
    public class Book : Item
    {
        [DataMember]
        public string Author { get; private set; }
        [DataMember]
        public short Pages { get; private set; }

        internal Book() { }

        public override void Update(Item book, string folderPath)
        {
            base.Update(book, folderPath);
            Author = ((Book)book).Author == null ? Author : ((Book)book).Author;
            Pages = ((Book)book).Pages == 0 ? Pages : ((Book)book).Pages;
        }
    }
}

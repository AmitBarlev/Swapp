using System.Runtime.Serialization;

namespace Swap.Objects.PushNotification
{
    [DataContract]
    public class Notification
    {
        [DataMember]
        public string title { get; set; }
        [DataMember]
        public string text { get; set; }

        internal Notification(string titleOfMessage, string bodyOfMessage)
        {
            title = titleOfMessage;
            text = bodyOfMessage;
        }
    }
}

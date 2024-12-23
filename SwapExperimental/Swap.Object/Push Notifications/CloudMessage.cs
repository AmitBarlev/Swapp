using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Swap.Objects.PushNotification
{
    [DataContract]
    public class CloudMessage
    {
        [DataMember]
        private Notification notification { get; set; }
        [DataMember]
        private string[] registration_ids { get; set; }
        [DataMember]
        public object data { get; set; }

        public CloudMessage(string[] sendToTokens, string title, string text, object sendToData = null)
        {
            registration_ids = sendToTokens;
            data = sendToData;
            notification = new Notification(title, text);
        }
    }
}

using Swap.Object.GeneralObjects;
using System;
using System.Runtime.Serialization;

namespace Swap.Object.ChatObjects
{
    [DataContract]
    public class InstantMessage
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public Guid ChatId { get; set; }
        [DataMember]
        public int UserId { get; set; }
        [DataMember]
        public string UserName { get; set; }
        [DataMember]
        public string Body { get; set; }
        [DataMember]
        public DateTime DateTime { get; set; } = DateTime.Now;

        public Chat Chat { get; set; }

        public User User { get; set; }

        public InstantMessage()
        {
            DateTime = DateTime.Now;
        }

        internal InstantMessage(string body, User user, Chat chat)
        {
            Body = body;
            Chat = chat;
            User = user;
            UserName = user.FirstName;
        }
    }
}

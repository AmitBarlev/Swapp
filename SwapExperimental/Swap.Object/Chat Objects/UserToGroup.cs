using Swap.Object.GeneralObjects;
using System;

namespace Swap.Object.ChatObjects
{
    public class UserToGroup
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public Chat Chat { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }

        internal UserToGroup() { }
        internal UserToGroup(User user) => User = user;
    }
}

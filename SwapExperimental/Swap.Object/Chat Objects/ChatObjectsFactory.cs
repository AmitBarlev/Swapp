using Swap.Object.GeneralObjects;
using System.Collections.Generic;

namespace Swap.Object.ChatObjects
{
    public static class ChatObjectsFactory
    {
        public static Chat GetChat(User fromUser, User toUser)
        {
            List<UserToGroup> participants = new List<UserToGroup>();
            participants.Add(GetUserToGroup(fromUser));
            participants.Add(GetUserToGroup(toUser));

            return new Chat(participants);
        }

        public static UserToGroup GetUserToGroup(User user) => new UserToGroup(user);

        public static InstantMessage GetInstantMessage(string body, User user, Chat chat)
        {
            return new InstantMessage(body, user, chat);
        }
    }
}

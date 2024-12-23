using System;
using System.Collections.Generic;
using System.Linq;
using Swap.Object.ChatObjects;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;

namespace Swap.WebApi.Repositories
{
    public class InstantMessageRepository : Repository<InstantMessage>, IInstantMessageRepository
    {
        private SwapContext _instantMessageContext => _context as SwapContext;

        public InstantMessageRepository(SwapContext context) : base(context) {}

        public IEnumerable<InstantMessage> GetLastMessages(string chatId, int wantedMessageCount)
        {
            IEnumerable<InstantMessage> messagesOfChat = GetAll(m => m.ChatId.ToString() == chatId);
            int howManyMessagesChatHave = messagesOfChat.Count();
            wantedMessageCount = wantedMessageCount > howManyMessagesChatHave ? howManyMessagesChatHave : wantedMessageCount;
            return messagesOfChat.Skip(howManyMessagesChatHave - wantedMessageCount);
        }

        public IEnumerable<InstantMessage> GetAll(Func<InstantMessage, bool> predicate) => GetAll().Where(m => predicate(m));

    }
}

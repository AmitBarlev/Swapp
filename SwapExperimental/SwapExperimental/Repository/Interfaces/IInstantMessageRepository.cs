using Swap.Object.ChatObjects;
using System;
using System.Collections.Generic;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface IInstantMessageRepository : IRepository<InstantMessage>
    {
        IEnumerable<InstantMessage> GetAll(Func<InstantMessage, bool> predicate);

        IEnumerable<InstantMessage> GetLastMessages(string chatId, int wantedNumberOfMessages);
    }
}

using Swap.Object.ChatObjects;
using System;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Chat Get(string guid);
    }
}

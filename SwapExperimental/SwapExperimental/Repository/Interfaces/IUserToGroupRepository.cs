using Swap.Object.ChatObjects;
using System;
using System.Collections.Generic;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface IUserToGroupRepository : IRepository<UserToGroup>
    {
        bool TryGetGroupName(int fromId, int toId, out string groupName);

         IEnumerable<UserToGroup> GetAll(Func<UserToGroup, bool> func);
    }
}

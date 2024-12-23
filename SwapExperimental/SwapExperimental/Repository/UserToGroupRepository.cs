using Microsoft.EntityFrameworkCore;
using Swap.Object.ChatObjects;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swap.WebApi.Repositories
{
    public class UserToGroupRepository : Repository<UserToGroup>, IUserToGroupRepository
    {
        private SwapContext _userToGroupContext => _context as SwapContext;

        public UserToGroupRepository(SwapContext context): base(context) {}

        public bool TryGetGroupName(int fromId, int toId, out string groupName)
        {
            bool doesGroupExist = false;
            const int areGuidsEqual = 0;
            groupName = null;
            foreach (UserToGroup element in _userToGroupContext.UserToGroup.Where(u => u.UserId == fromId))
            {
                if (_userToGroupContext.UserToGroup.Any(u => u.UserId == toId && u.Guid.CompareTo(element.Guid) == areGuidsEqual)) 
                {
                    doesGroupExist = true;
                    groupName = element.Guid.ToString();
                    break;
                }
            }
            return doesGroupExist;
        }

        public IEnumerable<UserToGroup> GetAll(Func<UserToGroup, bool> func)
        {
            return _userToGroupContext.UserToGroup.Include(u => u.Chat).Include(u => u.Chat.Messages).Where(u => func(u));
        }
    }
}

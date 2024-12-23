using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Swap.Object.ChatObjects;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;

namespace Swap.WebApi.Repositories
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        private SwapContext _chatContext => _context as SwapContext;

        public ChatRepository(SwapContext context) : base(context) {}

        public Chat Get(string guid)
        {
            return _chatContext.Chats.Include(c => c.UsersToGroup)
                .Where(c => c.Id.ToString() == guid).FirstOrDefault();
        }
    }
}

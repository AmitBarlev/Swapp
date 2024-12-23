using Microsoft.EntityFrameworkCore;
using Swap.Object.GeneralObjects;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swap.WebApi.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private SwapContext _userContext => _context as SwapContext;

        public UserRepository(SwapContext context) : base(context) {}

        public User Get(Func<User, bool> func) => GetAll().Where(func).FirstOrDefault();

        public new User Get(int id) => GetAll().Where(u => u.Id == id).FirstOrDefault();

        public new IEnumerable<User> GetAll() => _userContext.Users.Include(u => u.Images);
    }
}

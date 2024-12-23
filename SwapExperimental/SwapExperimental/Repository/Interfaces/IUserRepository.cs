using Swap.Object.GeneralObjects;
using System;
using System.Collections.Generic;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<User> 
    {
        User Get(Func<User, bool> func);

        new User Get(int Id);

        new IEnumerable<User> GetAll();
    }
}

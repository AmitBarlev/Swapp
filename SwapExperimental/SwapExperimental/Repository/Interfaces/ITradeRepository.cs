using Swap.Object.GeneralObjects;
using System;
using System.Collections.Generic;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface ITradeRepository : IRepository<Trade>
    {
        IEnumerable<Trade> GetAll(Func<Trade, bool> func);
    }
}

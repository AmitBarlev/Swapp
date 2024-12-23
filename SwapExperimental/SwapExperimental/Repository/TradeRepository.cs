using Swap.Object.GeneralObjects;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories;
using Swap.WebApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Swap.WebApi.Repository
{
    public class TradeRepository : Repository<Trade>, ITradeRepository
    {
        private SwapContext _tradeContext => _context as SwapContext;

        public TradeRepository(SwapContext context) : base(context) { }

        public IEnumerable<Trade> GetAll(Func<Trade, bool> func)
        {
            return _tradeContext.Trades.Where(func);
        }
    }
}

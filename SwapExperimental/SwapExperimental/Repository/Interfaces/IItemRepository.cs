using Swap.Object.GeneralObjects;
using Swap.Object.Items;
using System;
using System.Collections.Generic;

namespace Swap.WebApi.Repositories.Interfaces
{
    public interface IItemRepository : IRepository<Item>
    {
        new Item Get(int id);
        Item Get(Func<Item, bool> func);
        new IEnumerable<Item> GetAll();
        IEnumerable<Item> GetAll(Func<Item, bool> func);
        IEnumerable<Item> GetAll(Filter filter);
    }
}

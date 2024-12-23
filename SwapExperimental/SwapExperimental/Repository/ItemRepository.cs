using Microsoft.EntityFrameworkCore;
using Swap.Object.GeneralObjects;
using Swap.Object.Items;
using Swap.WebApi.Entities;
using Swap.WebApi.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Swap.WebApi.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private SwapContext _itemContext => _context as SwapContext;

        public ItemRepository(SwapContext context) : base(context) { }

        public new Item Get(int id) => GetAll(i => i.Id == id).FirstOrDefault();

        public Item Get(Func<Item, bool> func) => GetAll(func).FirstOrDefault();

        public new IEnumerable<Item> GetAll() => _itemContext.Items.Include(i => i.ImagesOfItem).Include(i => i.Owner);

        public IEnumerable<Item> GetAll(Func<Item, bool> func)
        {
            IEnumerable<Item> items = GetAll();
            if (null != func)
                items = items.Where(func);

            return items;
        }

        public IEnumerable<Item> GetAll(Filter filter)
        {
            IQueryable<Item> query = filter.ToQueryable(GetAll().ToList());
            IEnumerable<Item> filteredItems = query.AsEnumerable();

            foreach (Item item in filteredItems)
            {
                item.LoadPictures();
            }

            return filteredItems;
        }

    }
}

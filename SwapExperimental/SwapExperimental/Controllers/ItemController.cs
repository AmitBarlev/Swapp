using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swap.Object.GeneralObjects;
using Swap.Object.Items;
using Swap.Object.Tools;
using Swap.WebApi.Repository;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Swap.Controllers
{
    public class ItemController : Controller
    {
        private IConfiguration _configuration;
        private Database _database;

        private readonly string _pictureFolder = null;
        private const string _idKey = "id";

        public ItemController(Database database, IConfiguration configuration)
        {
            _database = database;
            _configuration = configuration;
            _pictureFolder = configuration.GetSection("Addresses").GetValue<string>("PicturePath");
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetItems()
        {
            if (!Request.TryGetValue(_idKey, out int id))
                return BadRequest();

            IEnumerable<Item> itemsOfUser = _database.ItemTable.GetAll(i => i.IdCustomer == id);
            foreach (Item item in itemsOfUser)
            {
                item.LoadPictures();
            }
            return Ok(itemsOfUser);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetItem()
        {
            if (!Request.TryGetValue(_idKey, out int itemId))
                return BadRequest();

            Item item = _database.ItemTable.Get(itemId);
            if (null == item)
                return BadRequest();

            item.LoadPictures();
            return Ok(item);
        }

        [Authorize]
        [HttpGet]
        public IActionResult GetLastItem()
        {

            if (!Request.TryGetValue(_idKey, out int id))
                return BadRequest();

            Item item = _database.ItemTable.GetAll(i => i.IdCustomer == id)
                .OrderByDescending(x => x.UploadDate).FirstOrDefault();
            item.LoadPictures();
            return Ok(item);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Create([FromBody] Item item)
        {
            if (!Request.TryGetToken(out string token))
                return BadRequest();

            User userToCreateItem = _database.UserTable.Get(u => u.Token == token);
            if (null == userToCreateItem)
                return BadRequest();

            item.Owner = userToCreateItem;
            item.UploadDate = DateTime.Now;
            item.SavePictures(_pictureFolder);
            _database.ItemTable.Add(item);
            return Ok(item.Id);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Update([FromBody] Item item)
        {
            if (!Request.TryGetToken(out string token))
                return BadRequest();

            User user = _database.UserTable.Get(u => u.Token == token);
            Item itemToUpdate = _database.ItemTable.Get(i => i.Id == item.Id);
            if (null == itemToUpdate || null == user)
                return BadRequest();

            itemToUpdate.Owner = user;
            itemToUpdate.Update(item, _pictureFolder);
            _database.ItemTable.Update(itemToUpdate);
            return Ok();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Delete([FromBody] Item itemToDelete)
        {
            Item item = _database.ItemTable.Get(i => i.Id == itemToDelete.Id);
            if (null == item)
                return BadRequest();

            item.DeletePictures();
            _database.ItemTable.Remove(item);
            return Ok();
        }

        [HttpGet]
        public IActionResult Filter()
        {
            if (!Request.TryGetToken(out string token) && Request.QueryString.HasValue)
                return BadRequest();

            User filteringUser = _database.UserTable.Get(u => u.Token == token);
            NameValueCollection filterParameters = HttpUtility.ParseQueryString(Request.QueryString.Value);

            return Ok(_database.ItemTable.GetAll(new Filter(filteringUser, filterParameters)));
        }

        [HttpGet]
        public IActionResult IncView()
        {
            if (!Request.TryGetValue(_idKey, out int id))
                return BadRequest();

            Item itemToIncrementView = _database.ItemTable.Get(i => i.Id == id);
            if (null == itemToIncrementView)
                return BadRequest();

            itemToIncrementView.IncView();
            _database.ItemTable.Update(itemToIncrementView);
            return Ok();
        }

        [HttpGet]
        public IActionResult GetMostViewed()
        {
            int maximumItems = 20;
            const int bookFlag = 1, videoGameFlag = 2;

            Func<Item, bool> func = null;
            if (Request.QueryString.HasValue)
            {
                NameValueCollection urlParams = HttpUtility.ParseQueryString(Request.QueryString.Value);
                if (urlParams.TryGetValue("ty", out int type))
                {
                    if (bookFlag == type)
                        func = i => i is Book;
                    else if (videoGameFlag == type)
                        func = i => i is VideoGame;
                }

                if (urlParams.TryGetValue("lim", out int limit))
                    maximumItems = limit;
            }

            IEnumerable<Item> output = _database.ItemTable.GetAll(func).
                OrderByDescending(i => i.Views).Take(maximumItems);

            foreach (Item item in output)
            {
                item.LoadPictures();
            }

            return Ok(output);
        }
    }
}

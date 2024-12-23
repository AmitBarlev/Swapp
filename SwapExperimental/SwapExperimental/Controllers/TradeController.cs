using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swap.Object.GeneralObjects;
using Swap.Object.Items;
using Swap.Object.PushNotifications;
using Swap.Object.Tools;
using Swap.Objects.PushNotification;
using Swap.WebApi.Repository;
using Swap.WebApi.Services;
using System.Collections.Generic;


namespace Swap.WebApi.Controllers
{
    [Authorize]
    public class TradeController : Controller
    {
        private IConfiguration _configuration;
        private Database _database;
        private const string _cloudMessging = "Cloud Messaging";
        private const string _title = "Title";
        private const string _body = "Body";

        public TradeController(Database database, IConfiguration configuration)
        {
            _database = database;
            _configuration = configuration;
        }

        [HttpPost]
        [Authorize]
        public IActionResult OfferTrade([FromBody] Trade trade)
        {
            Item itemOffered = _database.ItemTable.Get(i => i.Id == trade.ItemId);
            User userOfferingTrade = _database.UserTable.Get(u => u.Id == trade.OfferedById);
            User userOfferedTrade = itemOffered.Owner;

            if (null == itemOffered || null == userOfferingTrade || null == userOfferedTrade)
                return BadRequest();

            trade.SetForeignKeys(userOfferedTrade, userOfferingTrade, itemOffered);
            using (ApiHttpClient client = new ApiHttpClient(_configuration))
            {
                IConfigurationSection tradeSection = _configuration.GetSection(_cloudMessging).GetSection("OfferTrade");
                CloudMessage message = CloudMessageFactory.GetCloudMessage(userOfferedTrade, tradeSection, userOfferingTrade.FirstName);
                client.SendPushNotification(message);
            }
            _database.TradeTable.Add(trade);
            return Ok();
        }

        [HttpPost]
        [Authorize]
        public IActionResult AnswerTrade([FromBody] Trade trade)
        {
            const int decline = 0;
            User userAnsweringToTrade = _database.UserTable.Get(u => u.Id == trade.OfferedToId);
            User userToAnswerTo = _database.UserTable.Get(u => u.Id == trade.OfferedById);
            if (null == userAnsweringToTrade || null == userToAnswerTo)
                return BadRequest();

            CloudMessage message = null;
            Trade tradeToUpdate = _database.TradeTable.Get(trade.Id);
            if (default(int) == trade.ItemId)
            {
                IConfigurationSection declineSection = _configuration.GetSection(_cloudMessging).GetSection("DeclineTrade");
                message = CloudMessageFactory.GetCloudMessage(userToAnswerTo, declineSection, userAnsweringToTrade.FirstName);
                tradeToUpdate.SetStatus(decline);
            }
            else
            {
                IConfigurationSection approveSection = _configuration.GetSection(_cloudMessging).GetSection("ApproveTrade");
                Item item = _database.ItemTable.Get(i => i.Id == trade.ItemId);
                if (null == item)
                    return BadRequest();

                message = CloudMessageFactory.GetCloudMessage(userToAnswerTo, approveSection, userAnsweringToTrade.FirstName, item.Name);
                tradeToUpdate.SetStatus(int.Parse(trade.ItemsToTrade));
            }
            _database.TradeTable.Update(tradeToUpdate);

            using (ApiHttpClient http = new ApiHttpClient(_configuration))
            {
                http.SendPushNotification(message);
            }

            return Ok();
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetTradeNotifications()
        {
            if (!Request.TryGetValue("id", out int id))
                return BadRequest();

            IEnumerable<Trade> tradesOfUser = _database.TradeTable.GetAll(t => t.OfferedToId == id || t.OfferedById == id);
            foreach (Trade trade in tradesOfUser)
            {
                trade.WipeForeignKeys();
            }

            return Ok(tradesOfUser);
        }
    }
}

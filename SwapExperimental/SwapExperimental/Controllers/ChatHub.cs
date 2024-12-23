using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Swap.Object.ChatObjects;
using Swap.Object.GeneralObjects;
using Swap.Object.PushNotifications;
using Swap.Object.Tools;
using Swap.Objects.PushNotification;
using Swap.WebApi.Repository;
using Swap.WebApi.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Swap.WebApi.SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private Database _database = null;
        private MemoryCache _cache = null;
        private IConfiguration _configuration = null;

        public ChatHub(Database database, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _database = database;
            _cache = memoryCache as MemoryCache;
            _configuration = configuration;
        }

        public async Task Connect(int id)
        {
            User userToAdd = _database.UserTable.Get(id);
            if (null == userToAdd)
                await Task.FromResult<User>(null);

            string perviousConnectionId = userToAdd.ChatConnectionId;

            userToAdd.SetConnectionId(Context.ConnectionId);
            _database.UserTable.Update(userToAdd);
            foreach (UserToGroup userToGroup in _database.UserToGroupTable.GetAll(u => u.UserId == id))
            {
                string groupGuid = userToGroup.Guid.ToString();
                await Groups.RemoveFromGroupAsync(perviousConnectionId, groupGuid);
                _cache.Remove(groupGuid);
                await Groups.AddToGroupAsync(userToAdd.ChatConnectionId, groupGuid);
            }
        }

        public async Task SendMessage(string chatId, int fromId, int toId, string body)
        {
            User from = _database.UserTable.Get(fromId);
            User to = _database.UserTable.Get(toId);
            if (null == from || null == to)
                await Task.FromResult<object>(null);

            if (string.IsNullOrEmpty(chatId) || !_cache.TryGetValue(chatId, out Chat chat))
            {
                if (!_database.UserToGroupTable.TryGetGroupName(fromId, toId, out chatId))
                {
                    chat = ChatObjectsFactory.GetChat(from, to);
                    _database.ChatsTable.Add(chat);
                    chatId = chat.Id.ToString();
                    await Groups.AddToGroupAsync(from.ChatConnectionId, chatId);
                    await Groups.AddToGroupAsync(to.ChatConnectionId, chatId);
                }
                chat = _database.ChatsTable.Get(chatId);
                _cache.Set(chatId, chat);
            }
            chatId = chat.Id.ToString();
            InstantMessage instantMessage = ChatObjectsFactory.GetInstantMessage(body, from, chat);
            _database.InstantMessageTable.Add(instantMessage);
            using (ApiHttpClient http = new ApiHttpClient(_configuration))
            {
                IConfigurationSection FCMSection = _configuration.GetSection("Cloud Messaging");
                CloudMessage message = CloudMessageFactory.GetCloudMessage(to, FCMSection.GetSection("GetChatMessage"),
                    from.FirstName, body);

                http.SendPushNotification(message);
            }
            await Clients.Group(chatId).SendAsync("ReceiveMessage", chatId, to.Id, to.FirstName, from.FirstName, body, fromId);
            await Clients.Group(chatId).SendAsync("ReceiveMessageViewModel", from.FirstName, fromId, body);
        }
    }
}

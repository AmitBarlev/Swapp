using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Swap.Object.GeneralObjects;
using Swap.Object.PushNotifications;
using Swap.Object.Tools;
using Swap.Objects.PushNotification;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Swap.WebApi.Services
{
    public class ApiHttpClient : HttpClient
    {
        private readonly string _authServerAddress = null;
        private IConfiguration _configuration = null;

        public ApiHttpClient(string authServerAddress, IConfiguration configuration)
        {
            _authServerAddress = authServerAddress;
            _configuration = configuration;
        }

        public ApiHttpClient(IConfiguration configuration) => _configuration = configuration;

        public ApiHttpClient(string authServerAddress) => _authServerAddress = authServerAddress;

        public ApiHttpClient()
        {

        }

        private static FormUrlEncodedContent GetTokenRequestForm(User user)
        {
            return new FormUrlEncodedContent(new KeyValuePair<string, string>[]
                {
                    new KeyValuePair<string,string>("client_id", "Swap"),
                    new KeyValuePair<string,string>("client_secret", "secret"),
                    new KeyValuePair<string,string>("grant_type", "password"),
                    new KeyValuePair<string,string>("username", user.Email),
                    new KeyValuePair<string,string>("password", user.Password)
            });
        }

        public string GetAccessToken(User user)
        {
            return GetAccessTokenPack(user).Result["access_token"];
        }

        private async Task<Dictionary<string,string>> GetAccessTokenPack(User user)
        {
            HttpResponseMessage tokenResponse = await PostAsync($"{_authServerAddress}/connect/token",
                GetTokenRequestForm(user));
            return FormatTools.ParseJson(tokenResponse.Content.ReadAsStringAsync().Result);
        }

        public void SendPushNotification(User user, string sectionName)
        {
            IConfigurationSection section = _configuration.GetSection("Cloud Messaging").GetSection(sectionName);
            CloudMessage message = CloudMessageFactory.GetCloudMessage(user, section);
            message.data = new Dictionary<string, string>
            {
                ["chatId"] = "2",
                ["fromUserId"] = user.FirstName,
                ["toUserId"] = user.Email,
                ["fromUserName"] = user.Password,
            };
            SendPushNotification(message);
        }

        public void SendPushNotification(CloudMessage cloudMessage)
        {
            IConfigurationSection FCMsection = _configuration.GetSection("FCM");
            string firebaseUrl = FCMsection.GetValue<string>("URL");

            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, firebaseUrl);
            string authorizationString = FCMsection.GetValue<string>("Authorization");
            string senderId = FCMsection.GetValue<string>("Sender");
            httpRequestMessage.Headers.TryAddWithoutValidation("Authorization", authorizationString);
            httpRequestMessage.Headers.TryAddWithoutValidation("Sender", senderId);

            string jsonContent = JsonConvert.SerializeObject(cloudMessage);

            httpRequestMessage.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            SendAsync(httpRequestMessage).GetAwaiter().GetResult();
        }
    }
}

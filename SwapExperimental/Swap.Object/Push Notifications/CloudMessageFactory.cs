using Microsoft.Extensions.Configuration;
using Swap.Object.GeneralObjects;
using Swap.Objects.PushNotification;
using System.Linq;
using System.Text;

namespace Swap.Object.PushNotifications
{
    public static class CloudMessageFactory
    {
        public static CloudMessage GetCloudMessage(User[] users, string title, string body, object data = null)
        {
            return new CloudMessage(users.Select(u => u.FirebaseToken).ToArray(), title, body, data);
        }

        public static CloudMessage GetCloudMessage(User[] users, IConfigurationSection section, string titleImbue = null,
            string bodyImbue = null, object data = null)
        {
            StringBuilder title = new StringBuilder();
            string sectionTitle = section.GetValue<string>("Title");
            if (null == titleImbue)
                title.Append(sectionTitle);
            else
                title.AppendFormat(sectionTitle, titleImbue);

            StringBuilder body = new StringBuilder();
            string sectionBody = section.GetValue<string>("Body");
            if (null == bodyImbue)
                body.Append(sectionBody);
            else
                body.AppendFormat(sectionBody, bodyImbue);

            return GetCloudMessage(users, title.ToString(), body.ToString(), data);
        }

        public static CloudMessage GetCloudMessage(User user, string title, string body, object data = null)
        {
            return GetCloudMessage(new User[] { user }, title, body, data);
        }

        public static CloudMessage GetCloudMessage(User user, IConfigurationSection section, string titleImbue = null,
            string bodyImbue = null, object data = null)
        {
            return GetCloudMessage(new User[] { user }, section, titleImbue, bodyImbue, data);
        }
    }
}

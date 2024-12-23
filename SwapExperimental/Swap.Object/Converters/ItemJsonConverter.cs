using Newtonsoft.Json.Linq;
using Swap.Object.Items;
using System;

namespace Swap.Object.Converters
{
    public class ItemJsonConverter : JsonCreationConverter<Item>
    {
        protected override Item Create(Type objectType, JObject json)
        {
            if (json == null)
                throw new ArgumentNullException("Json object is null");

            if (json["Platform"] != null)
            {
                return new VideoGame();
                
            }
            else if (json["Author"] != null || json["Pages"] != null)
            {
                return new Book();
            }
            else
            {
                return new Item();
            }
        }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Swap.Object.Converters
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        public override bool CanWrite => false;
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        protected abstract T Create(Type objectType, JObject json);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("Reader is null");
            if (serializer == null)
                throw new ArgumentNullException("Serializer is null");
            if (reader.TokenType == JsonToken.Null)
                return null;

            JObject json = JObject.Load(reader);
            T target = Create(objectType, json);
            serializer.Populate(json.CreateReader(), target);
            return target;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Playground.ImportExport
{
    public class JsonDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(decimal) || objectType == typeof(decimal?);
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            decimal? d = value as decimal?;
            string s = d.Value.ToString(CultureInfo.InvariantCulture);

            if (d.HasValue)
            {
                writer.WriteRawValue("{\"$type\":\"System.Decimal\",\"$value\":" + s + "}");
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}

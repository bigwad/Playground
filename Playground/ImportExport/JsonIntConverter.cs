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
    public class JsonIntConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ushort) || objectType == typeof(ushort?)
                || objectType == typeof(short) || objectType == typeof(short?)
                || objectType == typeof(uint) || objectType == typeof(uint?)
                || objectType == typeof(int) || objectType == typeof(int?)
                || objectType == typeof(ulong) || objectType == typeof(ulong?)
                || objectType == typeof(long) || objectType == typeof(long?);
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
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            switch (value)
            {
                case int i:
                    writer.WriteRawValue("{\"$type\":\"System.Int32\",\"$value\":" + i.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
                case uint ui:
                    writer.WriteRawValue("{\"$type\":\"System.UInt32\",\"$value\":" + ui.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
                case long l:
                    writer.WriteRawValue("{\"$type\":\"System.Int64\",\"$value\":" + l.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
                case ulong ul:
                    writer.WriteRawValue("{\"$type\":\"System.UInt64\",\"$value\":" + ul.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
                case short s:
                    writer.WriteRawValue("{\"$type\":\"System.Int16\",\"$value\":" + s.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
                case ushort us:
                    writer.WriteRawValue("{\"$type\":\"System.UInt16\",\"$value\":" + us.ToString(CultureInfo.InvariantCulture) + "}");
                    break;
            }
        }
    }
}

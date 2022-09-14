using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace XRNeckSafer
{
    //public class ActionPropertyConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(ActionProperty);
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        JObject jObject = JObject.Load(reader);
    //        var value = jObject.GetValue("Value");
    //        switch (value.Type)
    //        {
    //            case JTokenType.Boolean:
    //                return JsonConvert.DeserializeObject<BooleanActionProperty>(jObject.ToString());
    //            case JTokenType.Integer:
    //                return JsonConvert.DeserializeObject<IntegerActionProperty>(jObject.ToString());
    //            default:
    //                throw new Exception();
    //        }
    //        throw new NotImplementedException();
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        serializer.Serialize(writer, value);
    //    }
    //}

    //public class BooleanActionPropertyConverter : JsonConverter
    //{
    //    public override bool CanConvert(Type objectType)
    //    {
    //        return objectType == typeof(BooleanActionProperty);
    //    }

    //    public override bool CanRead => false;

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return serializer.Deserialize<BooleanActionProperty>(reader);
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        var property = value as BooleanActionProperty;
    //        if (property != null && property.Invert)
    //        {
    //            property.Value = !property.Value;
    //        }
    //        writer.WriteValue(value);
    //    }
    //}
}

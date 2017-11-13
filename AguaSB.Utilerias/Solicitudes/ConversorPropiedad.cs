using System;
using Newtonsoft.Json;

namespace AguaSB.Utilerias.Solicitudes
{
    public class PropiedadConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(Propiedad);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) =>
            new Propiedad { Nombre = reader.Value?.ToString() };

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            writer.WriteValue(((Propiedad)value).Nombre);
    }
}

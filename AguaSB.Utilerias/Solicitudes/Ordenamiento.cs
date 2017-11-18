using Newtonsoft.Json;
using System.ComponentModel;
using System;

namespace AguaSB.Utilerias.Solicitudes
{
    public class ListSortDirectionConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => typeof(ListSortDirection?) == objectType;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var valor = reader.Value?.ToString() ?? "";

            if (nameof(ListSortDirection.Ascending).StartsWith(valor, StringComparison.OrdinalIgnoreCase))
                return ListSortDirection.Ascending;
            else if (nameof(ListSortDirection.Descending).StartsWith(valor, StringComparison.OrdinalIgnoreCase))
                return ListSortDirection.Descending;
            else
                return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((ListSortDirection?)value)
            {
                case ListSortDirection.Ascending:
                    writer.WriteValue(nameof(ListSortDirection.Ascending).Substring(0, 3));
                    break;
                case ListSortDirection.Descending:
                    writer.WriteValue(nameof(ListSortDirection.Descending).Substring(0, 4));
                    break;
                default:
                    writer.WriteNull();
                    break;
            }
        }

        public override bool CanWrite => true;
    }

    public sealed class Ordenamiento
    {
        public Propiedad Propiedad { get; set; }

        [JsonConverter(typeof(ListSortDirectionConverter))]
        public ListSortDirection? Direccion { get; set; }

        public void Cambiar()
        {
            switch (Direccion)
            {
                case ListSortDirection.Ascending:
                    Direccion = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    Direccion = null;
                    break;
                default:
                    Direccion = ListSortDirection.Ascending;
                    break;
            }
        }
    }
}

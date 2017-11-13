using System;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace AguaSB.Utilerias.Solicitudes
{
    public class CondicionConverterContract : DefaultContractResolver
    {
        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (typeof(Condicion).IsAssignableFrom(objectType) && !objectType.IsAbstract)
                return null;
            return base.ResolveContractConverter(objectType);
        }
    }

    public class CondicionConverter : JsonConverter
    {
        private static readonly JsonSerializerSettings ConversionSubclase = new JsonSerializerSettings()
        {
            ContractResolver = new CondicionConverterContract()
        };

        public override bool CanConvert(Type objectType) => objectType == typeof(Condicion);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            var objeto = jo.ToString();

            const string nombreValor = nameof(Igual<object>.Valor);
            const string nombreDesde = nameof(Rango<object>.Desde);
            const string nombreHasta = nameof(Rango<object>.Hasta);

            if (jo.Properties().Any(_ => _.Name == nombreValor))
            {
                return DeserializarIgualUsando(jo[nombreValor].Value<string>(), objeto);
            }
            else if (jo.Properties().Any(_ => _.Name == nombreDesde))
            {
                return DeserializarRangoUsando(jo[nombreDesde].Value<string>(), objeto);
            }
            else if (jo.Properties().Any(_ => _.Name == nombreHasta))
            {
                return DeserializarRangoUsando(jo[nombreHasta].Value<string>(), objeto);
            }
            else
            {
                throw new FormatException($"No hay suficiente información para determinar el tipo de la condicion:\n{objeto}");
            }
        }

        private object DeserializarIgualUsando(string valor, string objeto)
        {
            if (EsFechaValida(valor))
                return JsonConvert.DeserializeObject<Igual<DateTime?>>(objeto, ConversionSubclase);
            else if (decimal.TryParse(valor, out var _))
                return JsonConvert.DeserializeObject<Igual<decimal?>>(objeto, ConversionSubclase);
            else
                return JsonConvert.DeserializeObject<Igual<string>>(objeto, ConversionSubclase);
        }

        private object DeserializarRangoUsando(string valor, string objeto)
        {
            if (EsFechaValida(valor))
                return JsonConvert.DeserializeObject<Rango<DateTime?>>(objeto, ConversionSubclase);
            else if (decimal.TryParse(valor, out var _))
                return JsonConvert.DeserializeObject<Rango<decimal?>>(objeto, ConversionSubclase);
            else
                throw new FormatException("No se reconoció como rango tipo fecha o cantidad válido:\n" + objeto);
        }

        private bool EsFechaValida(string valor)
        {
            try
            {
                JsonConvert.DeserializeObject<DateTime?>($"\"{valor}\"");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            throw new InvalidOperationException();
    }
}

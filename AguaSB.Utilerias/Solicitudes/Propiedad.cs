using Newtonsoft.Json;

namespace AguaSB.Utilerias.Solicitudes
{
    [JsonConverter(typeof(PropiedadConverter))]
    public sealed class Propiedad
    {
        public static implicit operator Propiedad(string propiedad) => new Propiedad { Nombre = propiedad };

        public string Nombre { get; set; }

        public override string ToString() => $"Propiedad: {Nombre}";
    }
}

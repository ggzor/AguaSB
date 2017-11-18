using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Utilerias.Solicitudes
{
    public sealed class Solicitud
    {
        private static readonly JsonSerializerSettings Configuracion = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
        };

        public List<Propiedad> Agrupadores { get; set; } = new List<Propiedad>();

        public List<Propiedad> Columnas { get; set; } = new List<Propiedad>();

        public List<Condicion> Filtros { get; set; } = new List<Condicion>();

        public List<Ordenamiento> Ordenamientos { get; set; } = new List<Ordenamiento>();

        public bool Filtro<T>(string propiedad, out T filtro) where T : Condicion
        {
            var posiblesFiltros = Filtros.OfType<T>().Where(_ => _?.Propiedad?.Nombre == propiedad);

            filtro = posiblesFiltros.FirstOrDefault();
            return filtro != null;
        }

        public void Coercer()
        {
            bool EsPropiedadValida(Propiedad propiedad) => propiedad != null && !string.IsNullOrWhiteSpace(propiedad.Nombre);

            Agrupadores = Agrupadores.Where(EsPropiedadValida).ToList();
            Columnas = Columnas.Where(EsPropiedadValida).ToList();
            Ordenamientos = Ordenamientos.Where(_ => _ != null && EsPropiedadValida(_.Propiedad) && _.Direccion != null).ToList();

            Filtros.ForEach(_ => _?.Coercer());

            Filtros = Filtros.Where(_ => _ != null && EsPropiedadValida(_.Propiedad)).ToList();
        }

        public override string ToString() => JsonConvert.SerializeObject(this, Configuracion);

        public static bool IntentarObtener(string cadena, out Solicitud solicitud)
        {
            try
            {
                solicitud = JsonConvert.DeserializeObject<Solicitud>(cadena, Configuracion);
                return true;
            }
            catch (Exception)
            {
                solicitud = null;
                return false;
            }
        }
    }
}

using AguaSB.Utilerias;
using System.Collections;
using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Busqueda : Notificante
    {
        private bool? hayResultados;
        private bool? buscando;
        private long? conteo;
        private bool? tieneErrores;
        private string error;
        private IEnumerable resultados;
        private IEnumerable<PuntoNavegacion> puntosNavegacion;
        private string solicitud;

        public bool? HayResultados
        {
            get { return hayResultados; }
            set { N.Set(ref hayResultados, value); N.Change(nameof(NoHayResultados)); }
        }

        public bool? NoHayResultados => HayResultados == null ? null : !HayResultados;

        public bool? Buscando
        {
            get { return buscando; }
            set { N.Set(ref buscando, value); }
        }

        public long? Conteo
        {
            get { return conteo; }
            set { N.Set(ref conteo, value); }
        }

        public bool? TieneErrores
        {
            get { return tieneErrores; }
            set { N.Set(ref tieneErrores, value); }
        }

        public string Error
        {
            get { return error; }
            set { N.Set(ref error, value); }
        }

        public IEnumerable Resultados
        {
            get { return resultados; }
            set { N.Set(ref resultados, value); }
        }

        internal IList<ResultadoUsuario> Originales { get; set; }

        public IEnumerable<PuntoNavegacion> PuntosNavegacion
        {
            get { return puntosNavegacion; }
            set { N.Set(ref puntosNavegacion, value); }
        }

        public string Solicitud
        {
            get { return solicitud; }
            set { N.Set(ref solicitud, value); }
        }
    }
}

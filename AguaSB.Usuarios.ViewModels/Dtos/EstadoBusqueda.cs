using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class EstadoBusqueda : Notificante
    {
        private bool? hayResultados;

        public bool? HayResultados
        {
            get { return hayResultados; }
            set { N.Set(ref hayResultados, value); N.Change(nameof(NoHayResultados)); }
        }

        public bool? NoHayResultados => HayResultados == null ? null : !HayResultados;

        private bool? buscando;

        public bool? Buscando
        {
            get { return buscando; }
            set { N.Set(ref buscando, value); }
        }
    }
}

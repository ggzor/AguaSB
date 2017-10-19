using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Agrupador : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Raise([CallerMemberName]string propiedad = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));

        private bool activo;
        private string descripcion;

        public string Nombre { get; set; }

        public bool Activo
        {
            get { return activo; }
            set { activo = value; Raise(); }
        }

        public string Descripcion
        {
            get { return descripcion; }
            set { descripcion = value; Raise(); Raise(nameof(TieneDescripcion)); }
        }

        public bool TieneDescripcion => !string.IsNullOrWhiteSpace(Descripcion);
    }
}

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Agrupador : INotifyPropertyChanged
    {
        public static readonly Agrupador Ninguno = new Agrupador() { Activo = true, Nombre = nameof(Ninguno) };

        public event PropertyChangedEventHandler PropertyChanged;
        public void Raise([CallerMemberName]string propiedad = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propiedad));

        private bool activo;

        public string Nombre { get; set; }

        public bool Activo
        {
            get { return activo; }
            set { activo = value; Raise(); }
        }

        public string Descripcion { get; set; }

        public bool TieneDescripcion => !string.IsNullOrWhiteSpace(Descripcion);

        public string Propiedad { get; set; }

        public Func<object, object, int> Ordenador { get; set; }
        public Func<object, string> Conversor { get; set; }

        public override string ToString() => Nombre;
    }
}

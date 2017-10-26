using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Solicitud : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void Raise([CallerMemberName] string property = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));

        private string texto;

        public string Texto
        {
            get { return texto; }
            set { texto = value; Raise(); }
        }

        private Agrupador agrupador;
        private Filtros filtros;
        private Columnas columnas;

        public Agrupador Agrupador
        {
            get { return agrupador; }
            set { agrupador = value; Raise(); }
        }

        public Filtros Filtros
        {
            get { return filtros; }
            set { filtros = value; Raise(); }
        }

        public Columnas Columnas
        {
            get { return columnas; }
            set { columnas = value; Raise(); }
        }
    }
}

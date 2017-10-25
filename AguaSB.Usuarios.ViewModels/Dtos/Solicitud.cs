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

        public Agrupador Agrupador { get; set; }

        public Filtros Filtros { get; set; }
    }
}

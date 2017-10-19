using System;
using System.Collections;
using System.ComponentModel;

using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public abstract class Filtro : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private bool activo;

        public bool Activo
        {
            get { return activo; }
            set { N.Set(ref activo, value); }
        }

        public Filtro()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));
        }

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private Lazy<Notificador> notificador;
        protected Notificador N => notificador.Value;
        #endregion
    }
}

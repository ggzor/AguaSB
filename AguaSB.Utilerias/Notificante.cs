using System;
using System.Collections;
using System.ComponentModel;

namespace AguaSB.Utilerias
{
    public abstract class Notificante : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        protected Notificante()
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

        private readonly Lazy<Notificador> notificador;
        protected Notificador N => notificador.Value;
        #endregion        
    }
}

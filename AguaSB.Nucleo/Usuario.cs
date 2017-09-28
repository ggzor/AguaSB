using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Usuarios")]
    public abstract class Usuario : IEntidad, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }

        private ICollection<Contacto> contactos;

        public ICollection<Contacto> Contactos
        {
            get { return contactos; }
            set { N.Set(ref contactos, value); }
        }

        public Usuario()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            Contactos = new List<Contacto>();
        }

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        [NotMapped]
        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private Lazy<Notificador> notificador;
        [NotMapped]
        protected Notificador N => notificador.Value;
        #endregion
    }
}

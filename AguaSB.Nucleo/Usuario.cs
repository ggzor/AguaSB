using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Usuarios")]
    public abstract class Usuario : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; }

        [NotMapped]
        public abstract string NombreCompleto { get; }

        private ICollection<Contacto> contactos;
        private ICollection<Contrato> contratos;

        public ICollection<Contacto> Contactos
        {
            get { return contactos; }
            set { N.Set(ref contactos, value); }
        }

        public ICollection<Contrato> Contratos
        {
            get { return contratos; }
            set { N.Set(ref contratos, value); }
        }

        protected Usuario()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            Contactos = new List<Contacto>();
            Contratos = new List<Contrato>();
        }

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        [NotMapped]
        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private readonly Lazy<Notificador> notificador;
        [NotMapped]
        protected Notificador N => notificador.Value;
        #endregion

        public override string ToString() => NombreCompleto;
    }
}

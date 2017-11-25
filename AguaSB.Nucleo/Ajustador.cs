using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Ajustadores")]
    public class Ajustador : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; }

        private string nombre;
        private decimal multiplicador;

        [Required(ErrorMessage = Mensajes.Validacion.CampoRequerido)]
        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Nombre
        {
            get { return nombre; }
            set { N.Set(ref nombre, value); }
        }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal Multiplicador
        {
            get { return multiplicador; }
            set { N.Validate(ref multiplicador, value); }
        }

        public Ajustador()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));
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
    }
}

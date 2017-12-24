using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("DesactivacionesNotas")]
    public class DesactivacionNota : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        private string razon;
        private Nota nota;

        #region Propiedades
        [Required(ErrorMessage = "Se requiere referenciar la nota que se desactivará.")]
        public Nota Nota
        {
            get { return nota; }
            set { N.Validate(ref nota, value); }
        }

        [MaxLength(100)]
        public string Razon
        {
            get { return razon; }
            set { N.Validate(ref razon, value); }
        }
        #endregion

        public DesactivacionNota()
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

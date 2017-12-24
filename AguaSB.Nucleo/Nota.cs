using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Notas")]
    public class Nota : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }
        private TipoNota tipo;
        private string informacion;
        private int referencia;
        private DesactivacionNota desactivacion;

        #region Propiedades
        [Required(ErrorMessage = "Todas las notas deben tener su tipo definido")]
        public TipoNota Tipo
        {
            get { return tipo; }
            set { N.Validate(ref tipo, value); }
        }

        [MaxLength(50)]
        public string Informacion
        {
            get { return informacion; }
            set { N.Validate(ref informacion, value); }
        }

        public int Referencia
        {
            get { return referencia; }
            set { N.Validate(ref referencia, value); }
        }

        public DesactivacionNota Desactivacion
        {
            get { return desactivacion; }
            set { N.Validate(ref desactivacion, value); }
        }
        #endregion

        public Nota()
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

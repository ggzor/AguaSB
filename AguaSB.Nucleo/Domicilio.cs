using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Nucleo.Mensajes;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Domicilios")]
    public class Domicilio : IEntidad, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Calle calle;
        private string numero;

        public int Id { get; set; }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public Calle Calle
        {
            get { return calle; }
            set { N.Validate(ref calle, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public string Numero
        {
            get { return numero; }
            set { N.Validate(ref numero, value); }
        }

        public Domicilio()
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

        private Lazy<Notificador> notificador;
        [NotMapped]
        protected Notificador N => notificador.Value;
        #endregion

        public bool TieneCamposRequeridosVacios => string.IsNullOrWhiteSpace(Numero);

        public override string ToString() => $"Sección {Calle?.Seccion}, Calle {Calle}, Número {Numero}";
    }
}
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Nucleo.Mensajes;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    public enum ClaseContrato { Doméstico, Comercial, Industrial }

    [Table("TiposContrato")]
    public class TipoContrato : IEntidad, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private decimal multiplicador;
        private string nombre;
        private ClaseContrato claseContrato;

        public int Id { get; set; }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public string Nombre
        {
            get { return nombre; }
            set { N.Validate(ref nombre, value); }
        }

        // TODO: Agregar validación
        public decimal Multiplicador
        {
            get { return multiplicador; }
            set { N.Validate(ref multiplicador, value); }
        }

        public ClaseContrato ClaseContrato
        {
            get { return claseContrato; }
            set { N.Set(ref claseContrato, value); }
        }

        public TipoContrato()
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

        public override string ToString() => $"{Nombre} - {ClaseContrato}";
    }
}
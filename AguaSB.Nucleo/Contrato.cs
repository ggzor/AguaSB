using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Nucleo.Mensajes;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Contratos")]
    public class Contrato : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private Usuario usuario;
        private TipoContrato tipoContrato;
        private string medidaToma;
        private decimal adeudoInicial;
        private Domicilio domicilio;

        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public Usuario Usuario
        {
            get { return usuario; }
            set { N.Validate(ref usuario, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public TipoContrato TipoContrato
        {
            get { return tipoContrato; }
            set { N.Validate(ref tipoContrato, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public string MedidaToma
        {
            get { return medidaToma; }
            set { N.Validate(ref medidaToma, value); }
        }

        public decimal AdeudoInicial
        {
            get { return adeudoInicial; }
            set { N.Set(ref adeudoInicial, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public Domicilio Domicilio
        {
            get { return domicilio; }
            set { N.Validate(ref domicilio, value); }
        }

        public virtual ICollection<Pago> Pagos { get; set; }

        public Contrato()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            Pagos = new List<Pago>();
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

        public bool TieneCamposRequeridosVacios => string.IsNullOrWhiteSpace(MedidaToma);

        public override string ToString() => Domicilio?.ToString();
    }
}

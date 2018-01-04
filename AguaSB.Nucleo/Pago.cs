using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Pagos")]
    public class Pago : IEntidad, IAuditable, ICoercible, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; } = Fecha.Ahora;

        private DateTime fechaPago;
        private DateTime desde;
        private DateTime hasta;
        private Contrato contrato;
        private decimal monto;
        private decimal montoParcial;

        public DateTime FechaPago
        {
            get { return fechaPago; }
            set { N.Set(ref fechaPago, value); }
        }

        public DateTime Desde
        {
            get { return desde; }
            set { N.Set(ref desde, value); }
        }

        public DateTime Hasta
        {
            get { return hasta; }
            set { N.Set(ref hasta, value); }
        }

        public Contrato Contrato
        {
            get { return contrato; }
            set { N.Set(ref contrato, value); }
        }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal CantidadPagada
        {
            get { return monto; }
            set { N.Validate(ref monto, value); }
        }

        [Range(typeof(decimal), "0", "1000000")]
        public decimal Monto
        {
            get { return montoParcial; }
            set { N.Validate(ref montoParcial, value); }
        }

        public Pago()
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

        public void Coercer()
        {

        }
    }
}

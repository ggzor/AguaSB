using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Tarifas")]
    public class Tarifa : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }
        public DateTime FechaRegistro { get; set; }

        private decimal monto;
        private DateTime inicio;

        #region Propiedades
        [Range(typeof(decimal), "0", "1000000")]
        public decimal Monto
        {
            get { return monto; }
            set { N.Set(ref monto, value); }
        }

        public DateTime Inicio
        {
            get { return inicio; }
            set { inicio = value; }
        }
        #endregion

        public Tarifa()
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

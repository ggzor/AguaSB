using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Usuarios")]
    public abstract class Usuario : IEntidad, IAuditable, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; } = Fecha.Ahora;

        [NotMapped]
        public abstract string NombreCompleto { get; }

        public string NombreSolicitud { get; set; }

        public virtual ICollection<Contacto> Contactos { get; set; }

        public virtual ICollection<Contrato> Contratos { get; set; }

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

        protected void EstablecerNombreSolicitud(string nombre) => NombreSolicitud = ConvertirATextoSolicitud(nombre);

        public static string ConvertirATextoSolicitud(string texto)
        {
            var sb = new StringBuilder();

            char[] caracteres = texto.AsEnumerable()
                .Select(char.ToLower)
                .ToArray();

            sb.Append(caracteres);

            foreach (var k in "áéíóú".Zip("aeiou", (c1, c2) => new KeyValuePair<char, char>(c1, c2)))
                sb.Replace(k.Key, k.Value);

            return sb.ToString();
        }
    }
}

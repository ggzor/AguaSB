using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

using AguaSB.Nucleo.Mensajes;
using AguaSB.Utilerias;

namespace AguaSB.Nucleo
{
    [Table("Contactos")]
    public class Contacto : IEntidad, INotifyPropertyChanged, INotifyDataErrorInfo
    {
        private string informacion;

        public int Id { get; set; }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public TipoContacto TipoContacto { get; set; }

        [CustomValidation(typeof(Contacto), nameof(ValidarInformacion))]
        public string Informacion
        {
            get { return informacion; }
            set { N.Validate(ref informacion, value); }
        }

        public Contacto()
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

        public static ValidationResult ValidarInformacion(string informacion, ValidationContext contexto)
        {
            if (contexto.ObjectInstance is Contacto contacto)
            {
                if (contacto.TipoContacto != null)
                {
                    string expresionRegular = contacto.TipoContacto.ExpresionRegular;

                    if (string.IsNullOrEmpty(expresionRegular))
                        return ValidationResult.Success;
                    else
                        return Regex.IsMatch(informacion, expresionRegular)
                            ? ValidationResult.Success
                            : new ValidationResult($"El {contacto.TipoContacto.Nombre.ToLower()} no es válido.");
                }
                else
                {
                    throw new ValidationException("El tipo de contacto debe estar establecido antes que la información.");
                }
            }
            else
            {
                throw new ValidationException("Este método sólo puede validar la información de un objeto de la clase Contacto");
            }
        }
    }
}

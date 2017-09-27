using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace AguaSB.Nucleo.Mensajes
{
    public static class Validacion
    {
        public const string CampoRequerido = "Este campo no puede estar vacío.";

        private const string PatronUnSoloNombrePersona = @"[A-ZÑÁÉÍÓÚÜ][a-zñáéíóúü]{2,}";

        /// <summary>
        /// Coincidirá con cualquier cantidad de nombres separados por espacio.
        /// </summary>
        /// 
        public const string PatronNombrePersona = "( *)" + PatronUnSoloNombrePersona + @"( *)(" + PatronUnSoloNombrePersona + "( *))*";

        public const string NombrePersonaInvalido = "El nombre proporcionado no es válido.";

        public const string ApellidoInvalido = "El apellido proporcionado no es válido.";

        public static ValidationResult RegexValido(string regex)
        {
            try
            {
                new Regex(regex);
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult(ex.Message.Split('-').Last().Trim());
            }
        }
    }
}

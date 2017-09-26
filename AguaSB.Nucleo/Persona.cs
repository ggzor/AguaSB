using AguaSB.Nucleo.Mensajes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AguaSB.Nucleo
{
    [Table("Personas")]
    public class Persona : Usuario
    {
        private string nombre;
        private string apellidoPaterno;
        private string apellidoMaterno;

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        [RegularExpression(Validacion.PatronNombrePersona, ErrorMessage = Validacion.NombrePersonaInvalido)]
        public string Nombre
        {
            get { return nombre; }
            set { N.Validate(ref nombre, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        [RegularExpression(Validacion.PatronNombrePersona, ErrorMessage = Validacion.ApellidoInvalido)]
        public string ApellidoPaterno
        {
            get { return apellidoPaterno; }
            set { N.Validate(ref apellidoPaterno, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        [RegularExpression(Validacion.PatronNombrePersona, ErrorMessage = Validacion.ApellidoInvalido)]
        public string ApellidoMaterno
        {
            get { return apellidoMaterno; }
            set { N.Validate(ref apellidoMaterno, value); }
        }
    }
}

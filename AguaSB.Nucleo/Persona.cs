using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Nucleo.Mensajes;

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
            set { N.Validate(ref nombre, value); NombreCompletoModificado(); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        [RegularExpression(Validacion.PatronNombrePersona, ErrorMessage = Validacion.ApellidoInvalido)]
        public string ApellidoPaterno
        {
            get { return apellidoPaterno; }
            set { N.Validate(ref apellidoPaterno, value); NombreCompletoModificado(); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        [RegularExpression(Validacion.PatronNombrePersona, ErrorMessage = Validacion.ApellidoInvalido)]
        public string ApellidoMaterno
        {
            get { return apellidoMaterno; }
            set { N.Validate(ref apellidoMaterno, value); NombreCompletoModificado(); }
        }

        [NotMapped]
        public bool TieneCamposRequeridosVacios => new[] { Nombre, ApellidoPaterno, apellidoMaterno }.Any(string.IsNullOrWhiteSpace);

        [NotMapped]
        public override string NombreCompleto => $"{Nombre} {ApellidoPaterno} {ApellidoMaterno}";

        private void NombreCompletoModificado()
        {
            N.Change(nameof(NombreCompleto));
            EstablecerNombreSolicitud(NombreCompleto);
        }
    }
}

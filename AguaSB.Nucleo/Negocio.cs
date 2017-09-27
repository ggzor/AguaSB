using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using AguaSB.Nucleo.Mensajes;

namespace AguaSB.Nucleo
{
    [Table("Negocios")]
    public class Negocio : Usuario
    {
        private string nombre;
        private string rfc;
        private Persona representante;

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public string Nombre
        {
            get { return nombre; }
            set { N.Validate(ref nombre, value); }
        }

        public string Rfc
        {
            get { return rfc; }
            set { N.Validate(ref rfc, value); }
        }

        [Required(ErrorMessage = Validacion.CampoRequerido)]
        public Persona Representante
        {
            get { return representante; }
            set { N.Validate(ref representante, value); }
        }

        [NotMapped]
        public bool TieneCamposRequeridosVacios => string.IsNullOrWhiteSpace(Nombre);
    }
}

using AguaSB.Nucleo.Mensajes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AguaSB.Nucleo
{
    [Table("Negocios")]
    public class Negocio : Usuario
    {
        private string nombre;
        private string rfc;

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
    }
}

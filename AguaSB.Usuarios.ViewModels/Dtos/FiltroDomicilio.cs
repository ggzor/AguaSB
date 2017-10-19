using AguaSB.Nucleo;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class FiltroDomicilio : Filtro
    {
        private Seccion seccion;
        private Calle calle;

        public Seccion Seccion
        {
            get { return seccion; }
            set { N.Validate(ref seccion, value); }
        }

        public Calle Calle
        {
            get { return calle; }
            set { N.Validate(ref calle, value); }
        }
    }
}

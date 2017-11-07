using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Inicializadores
{
    public class AgregarTiposContacto : IInicializador
    {
        public AgregarTiposContacto(IRepositorio<TipoContacto> tiposContactoRepo) => Inicializar(tiposContactoRepo);

        private async void Inicializar(IRepositorio<TipoContacto> tiposContactoRepo)
        {
            var telefono = new TipoContacto
            {
                Nombre = "Teléfono",
                ExpresionRegular = @"^( *\d){10} *$"
            };

            var email = new TipoContacto
            {
                Nombre = "Email",
                ExpresionRegular = ".*"
            };

            foreach (var tipoContacto in new[] { telefono, email })
                await tiposContactoRepo.Agregar(tipoContacto).ConfigureAwait(true);
        }
    }
}

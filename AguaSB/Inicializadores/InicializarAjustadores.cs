using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Inicializadores
{
    public class InicializarAjustadores : IInicializador
    {
        public InicializarAjustadores(IRepositorio<Ajustador> ajustadoresRepo) => Inicializar(ajustadoresRepo);

        private async void Inicializar(IRepositorio<Ajustador> ajustadoresRepo)
        {
            var ajustador = new Ajustador
            {
                Nombre = "Registro",
                Multiplicador = 1
            };

            await ajustadoresRepo.Agregar(ajustador).ConfigureAwait(true);
        }
    }
}

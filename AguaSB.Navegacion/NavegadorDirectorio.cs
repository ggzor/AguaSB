using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class NavegadorDirectorio<T> : INavegador
    {
        public IReadOnlyDictionary<string, T> Directorio { get; }

        public IManejadorNavegacion<T> Manejador { get; }

        public NavegadorDirectorio(IReadOnlyDictionary<string, T> directorio, IManejadorNavegacion<T> manejador)
        {
            Directorio = directorio ?? throw new ArgumentNullException(nameof(directorio));
            Manejador = manejador ?? throw new ArgumentNullException(nameof(manejador));
        }

        public async Task Navegar(string direccion, object parametro)
        {
            try
            {
                var objeto = Directorio[direccion];
                await Manejador.Navegar(objeto, parametro);
            }
            catch (KeyNotFoundException)
            {
                await Manejador.EnDireccionNoEncontrada(direccion);
            }
        }
    }
}

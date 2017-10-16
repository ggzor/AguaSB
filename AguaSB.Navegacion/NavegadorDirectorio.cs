using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class NavegadorDirectorio<T> : INavegador
    {
        public Dictionary<string, T> Directorio { get; } = new Dictionary<string, T>();

        public ManejadorNavegacion<T> Manejador { get; }

        public NavegadorDirectorio(ManejadorNavegacion<T> manejador)
        {
            Manejador = manejador ?? throw new ArgumentNullException(nameof(manejador));
        }

        public async Task Navegar(string direccion, object parametro)
        {
            try
            {
                var objeto = Directorio[direccion];
                if (Manejador.Navegar != null)
                    await Manejador.Navegar(objeto, parametro);
            }
            catch (KeyNotFoundException)
            {
                if (Manejador.EnDireccionNoEncontrada != null)
                    await Manejador.EnDireccionNoEncontrada(direccion);
            }
        }
    }
}

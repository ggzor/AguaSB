using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public abstract class Navegador
    {
        public Task NavegarA(string direccion, params object[] elementos) =>
            NavegarA(Direccion.Descomponer(direccion).Cast<object>().Concat(elementos));

        public Task NavegarA(IEnumerable<object> elementos) =>
            NavegarA(new ColaNavegacion(elementos));

        public abstract Task NavegarA(ColaNavegacion direccion);
    }
}

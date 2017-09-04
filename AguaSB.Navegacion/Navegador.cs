using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public abstract class Navegador
    {
        public Task<bool> NavegarA(string direccion) => NavegarA(Direccion.Descomponer(direccion));

        public Task<bool> NavegarA(params string[] elementos) => NavegarA(new ColaNavegacion(elementos));

        public abstract Task<bool> NavegarA(ColaNavegacion direccion);
    }
}

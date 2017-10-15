using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public interface IManejadorNavegacion<T>
    {
        Task Navegar(T objeto, object parametro);

        Task EnDireccionNoEncontrada(string direccion);
    }
}

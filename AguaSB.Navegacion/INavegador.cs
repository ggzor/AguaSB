using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public interface INavegador
    {
        Task Navegar(string direccion, object parametro);
    }
}

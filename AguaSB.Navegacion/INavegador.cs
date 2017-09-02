using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public interface INavegador
    {
        Task<bool> NavegarA(string direccion);    
    }
}

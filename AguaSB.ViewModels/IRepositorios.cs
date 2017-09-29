using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.ViewModels
{
    public interface IRepositorios
    {

        IRepositorio<Usuario> Usuarios { get; }

    }
}
using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.ViewModels
{
    public interface IProveedorRepositorios
    {

        IRepositorio<Usuario> Usuarios { get; }

    }
}
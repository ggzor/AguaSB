using AguaSB.Navegacion;

namespace AguaSB.ViewModels
{
    public interface IViewModel
    {
        INodo<IProveedorServicios> Nodo { get; }
    }
}

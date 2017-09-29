
using AguaSB.Notificaciones;

namespace AguaSB.ViewModels
{
    public interface IProveedorServicios
    {
        IRepositorios Repositorios { get; }       
        
        ManejadorNotificaciones ManejadorNotificaciones { get; }
    }
}
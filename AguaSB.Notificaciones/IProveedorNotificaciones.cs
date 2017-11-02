using System;

namespace AguaSB.Notificaciones
{
    public interface IProveedorNotificaciones
    {
        IObservable<Notificacion> Notificaciones { get; }
    }
}

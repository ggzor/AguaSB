using System;

namespace AguaSB.Notificaciones
{
    public interface IAdministradorNotificaciones
    {
        void AgregarFuente(IObservable<Notificacion> fuente);

        void Lanzar(Notificacion notificacion);
    }
}

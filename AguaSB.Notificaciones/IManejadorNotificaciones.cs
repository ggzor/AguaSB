using System;

namespace AguaSB.Notificaciones
{
    public interface IManejadorNotificaciones
    {
        IObservable<Notificacion> Notificaciones { get; }

        void AgregarFuente(IObservable<Notificacion> fuente);
    }
}

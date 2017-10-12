using System;
using System.Linq;
using System.Reactive.Linq;

namespace AguaSB.Notificaciones
{
    public class ManejadorNotificaciones : IManejadorNotificaciones
    {
        public IObservable<Notificacion> Notificaciones { get; }

        public ManejadorNotificaciones()
        {
            var notificadores = Observable.FromEventPattern<IObservable<Notificacion>>(
                h => NotificadorAgregado += h,
                h => NotificadorAgregado -= h
            ).Select(e => e.EventArgs);

            var notificaciones = Observable.Merge(notificadores).Publish();

            Notificaciones = notificaciones;
            notificaciones.Connect();
        }

        private event EventHandler<IObservable<Notificacion>> NotificadorAgregado;

        public void AgregarFuente(IObservable<Notificacion> fuente) =>
            NotificadorAgregado?.Invoke(this, fuente);
    }
}

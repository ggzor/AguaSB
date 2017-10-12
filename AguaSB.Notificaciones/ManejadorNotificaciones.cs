using System;
using System.Linq;
using System.Reactive.Linq;

namespace AguaSB.Notificaciones
{
    public class ManejadorNotificaciones
    {

        private event EventHandler<IObservable<Notificacion>> NotificadorAgregado;

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

        public void AgregarProveedor(IObservable<Notificacion> proveedor) => NotificadorAgregado?.Invoke(this, proveedor);
    }
}

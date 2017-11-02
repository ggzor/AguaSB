using System;
using System.Linq;
using System.Reactive.Linq;

namespace AguaSB.Notificaciones
{
    public class ManejadorNotificaciones : IAdministradorNotificaciones, IProveedorNotificaciones
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

            AgregarObservadorDeInstancia();
        }

        private void AgregarObservadorDeInstancia()
        {
            var obs = Observable.FromEventPattern<Notificacion>(
                h => AgregarNotificacion += h,
                h => AgregarNotificacion -= h).Select(e => e.EventArgs);

            AgregarFuente(obs);
        }

        private event EventHandler<IObservable<Notificacion>> NotificadorAgregado;

        public void AgregarFuente(IObservable<Notificacion> fuente) =>
            NotificadorAgregado?.Invoke(this, fuente);

        private event EventHandler<Notificacion> AgregarNotificacion;

        public void Lanzar(Notificacion notificacion) =>
            AgregarNotificacion?.Invoke(this, notificacion);
    }
}

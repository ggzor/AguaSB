using System;
using System.Linq;
using System.Reactive.Linq;

using AguaSB.Nucleo;
using AguaSB.Notificaciones;

namespace AguaSB.Datos.Decoradores
{
    public class RepositorioNotificador<T> : IRepositorio<T> where T : class, IEntidad
    {
        public IRepositorio<T> Repositorio { get; }

        public event EventHandler<NotificacionEntidad<T>> RepositorioCambiado;

        public RepositorioNotificador(IRepositorio<T> repositorio, IAdministradorNotificaciones notificaciones)
        {
            if (notificaciones == null)
                throw new ArgumentNullException(nameof(notificaciones));

            Repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));

            var observable = Observable.FromEventPattern<NotificacionEntidad<T>>(
                h => RepositorioCambiado += h,
                h => RepositorioCambiado -= h
            ).Select(e => e.EventArgs);

            notificaciones.AgregarFuente(observable);
        }

        public IQueryable<T> Datos => Repositorio.Datos;

        public T Agregar(T entidad)
        {
            var resultado = Repositorio.Agregar(entidad);

            var notificacion = new EntidadAgregada<T>(resultado);
            RepositorioCambiado?.Invoke(this, notificacion);

            return resultado;
        }

        public T Actualizar(T entidad)
        {
            var resultado = Repositorio.Actualizar(entidad);

            var notificacion = new EntidadActualizada<T>(resultado);
            RepositorioCambiado?.Invoke(this, notificacion);

            return resultado;
        }

        public T Eliminar(T entidad)
        {
            var resultado = Repositorio.Eliminar(entidad);

            var notificacion = new EntidadEliminada<T>(resultado);
            RepositorioCambiado?.Invoke(this, notificacion);

            return resultado;
        }
    }
}

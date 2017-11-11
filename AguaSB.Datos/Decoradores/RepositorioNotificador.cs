using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using AguaSB.Nucleo;
using AguaSB.Notificaciones;

namespace AguaSB.Datos.Decoradores
{
    public class RepositorioNotificador<T> : IRepositorio<T> where T : IEntidad
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

        public IEnumerable<T> Datos => Repositorio.Datos;

        public async Task<T> Agregar(T entidad)
        {
            var resultado = await Repositorio.Agregar(entidad).ConfigureAwait(false);

            var notificacion = new EntidadAgregada<T>(resultado);
            RepositorioCambiado?.Invoke(this, notificacion);

            return resultado;
        }

        public async Task<T> Actualizar(T entidad)
        {
            var resultado = await Repositorio.Actualizar(entidad).ConfigureAwait(false);

            var notificacion = new EntidadActualizada<T>(resultado);
            RepositorioCambiado?.Invoke(this, notificacion);

            return resultado;
        }
    }
}

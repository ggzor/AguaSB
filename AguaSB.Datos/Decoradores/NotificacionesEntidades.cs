﻿using AguaSB.Notificaciones;
using AguaSB.Nucleo;

namespace AguaSB.Datos.Decoradores
{
    public abstract class NotificacionEntidad<T> : Notificacion where T : IEntidad
    {
        public T Entidad { get; }

        public NotificacionEntidad(T entidad) =>
            Entidad = entidad;
    }

    public sealed class EntidadAgregada<T> : NotificacionEntidad<T> where T : IEntidad
    {
        public EntidadAgregada(T entidad) : base(entidad) { }
    }
}

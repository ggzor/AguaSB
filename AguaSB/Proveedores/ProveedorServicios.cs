using System;
using AguaSB.Notificaciones;
using AguaSB.ViewModels;

namespace AguaSB.Proveedores
{
    public class ProveedorServicios : IProveedorServicios
    {
        private readonly Lazy<ProveedorRepositorios> proveedorRepositorios = new Lazy<ProveedorRepositorios>(() => new ProveedorRepositorios());
        private readonly ManejadorNotificaciones manejadorNotificaciones = new ManejadorNotificaciones();

        public IRepositorios Repositorios => proveedorRepositorios.Value;

        public ManejadorNotificaciones ManejadorNotificaciones => manejadorNotificaciones;
    }
}

using System;

using AguaSB.ViewModels;

namespace AguaSB.Proveedores
{
    public class ProveedorServicios : IProveedorServicios
    {
        private Lazy<ProveedorRepositorios> proveedorRepositorios = new Lazy<ProveedorRepositorios>(() => new ProveedorRepositorios());

        public IProveedorRepositorios Repositorios => proveedorRepositorios.Value;
    }
}

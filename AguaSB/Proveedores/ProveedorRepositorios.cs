using System;

using AguaSB.Datos;
using AguaSB.Datos.Decoradores;
using AguaSB.Notificaciones;
using AguaSB.Nucleo;
using AguaSB.ViewModels;

namespace AguaSB.Proveedores
{
    internal class ProveedorRepositorios : IRepositorios
    {
        private Lazy<IRepositorio<Usuario>> usuarios;

        public ProveedorRepositorios(ManejadorNotificaciones notificaciones)
        {
            usuarios = new Lazy<IRepositorio<Usuario>>(() => CrearRepositorioUsuarios(notificaciones));
        }

        private IRepositorio<Usuario> CrearRepositorioUsuarios(ManejadorNotificaciones notificaciones)
        {
            var repo = new RepositorioEnMemoria<Usuario>();

            return new RepositorioNotificador<Usuario>(repo, notificaciones);
        }

        public IRepositorio<Usuario> Usuarios => usuarios.Value;
    }
}
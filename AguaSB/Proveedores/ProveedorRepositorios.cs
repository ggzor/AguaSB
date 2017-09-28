using System;

using AguaSB.Datos;
using AguaSB.Nucleo;
using AguaSB.ViewModels;

namespace AguaSB.Proveedores
{
    internal class ProveedorRepositorios : IProveedorRepositorios
    {
        private Lazy<RepositorioEnMemoria<Usuario>> usuarios = new Lazy<RepositorioEnMemoria<Usuario>>(() => new RepositorioEnMemoria<Usuario>());

        public IRepositorio<Usuario> Usuarios => usuarios.Value;
    }
}
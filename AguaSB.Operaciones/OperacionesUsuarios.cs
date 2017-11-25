﻿using System.Linq;
using System.Threading.Tasks;

using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Operaciones
{
    public static class OperacionesUsuarios
    {
        public static Usuario BuscarDuplicados(Usuario usuario, IRepositorio<Usuario> repositorio) =>
            repositorio.Datos.Where(u => u.Id != usuario.Id).SingleOrDefault(u => u.NombreSolicitud == usuario.NombreSolicitud);

        public static Task<Usuario> BuscarDuplicadosAsync(Usuario usuario, IRepositorio<Usuario> repositorio) =>
            Task.Run(() => BuscarDuplicados(usuario, repositorio));
    }
}

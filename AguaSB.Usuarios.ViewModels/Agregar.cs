using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Operaciones;
using GGUtils.MVVM.Async;
using System;
using System.Threading.Tasks;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ModificarUsuarioBase
    {
        #region Comandos
        public AsyncDelegateCommand<int> AgregarPersonaComando { get; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; }
        #endregion

        #region Dependencias
        public INavegador Navegador { get; }
        #endregion

        public Agregar(IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContacto> tiposContactoRepo, INavegador navegador) : base(usuariosRepo, tiposContactoRepo)
        {
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            ConfigurarVerificador(() => new[] { AgregarPersonaComando, AgregarNegocioComando });

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, TodosCamposPersonaValidos);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, TodosCamposNegocioValidos);
        }

        private Task<int> AgregarPersona(IProgress<(double, string)> progreso) =>
            EjecutarAccionEnPersona(AccionAgregarUsuario, progreso);

        private Task<int> AgregarNegocio(IProgress<(double, string)> progreso) =>
            EjecutarAccionEnNegocio(AccionAgregarUsuario, progreso);

        private async Task<int> AccionAgregarUsuario(IProgress<(double, string)> progreso)
        {
            var resultado = await AgregarUsuario(progreso).ConfigureAwait(true);

            var _ = Navegador.Navegar("Contratos/Agregar", resultado);

            return resultado;
        }

        private Task<int> AgregarUsuario(IProgress<(double, string)> progreso = null) => Task.Run(() =>
        {
            progreso.Report((0.0, "Buscando duplicados..."));

            if (OperacionesUsuarios.BuscarDuplicados(Usuario, UsuariosRepo) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

            progreso.Report((50.0, "Agregando usuario..."));

            var usuario = UsuariosRepo.Agregar(Usuario).Result;

            progreso.Report((100.0, "Completado."));

            return usuario.Id;
        });
    }
}

using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Operaciones;
using GGUtils.MVVM.Async;
using Mehdime.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ModificarUsuarioBase
    {
        #region Comandos
        public AsyncDelegateCommand<int> AgregarPersonaComando { get; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; }

        public DelegateCommand NavegarA { get; }
        #endregion

        #region Dependencias
        public INavegador Navegador { get; }
        #endregion

        public Agregar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContacto> tiposContactoRepo, INavegador navegador) : base(ambito, usuariosRepo, tiposContactoRepo)
        {
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            ConfigurarVerificador(() => new[] { AgregarPersonaComando, AgregarNegocioComando });

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, TodosCamposPersonaValidos);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, TodosCamposNegocioValidos);

            NavegarA = new DelegateCommand(o => Navegador.Navegar((string)o, null));
        }

        private Task<int> AgregarPersona(IProgress<(double, string)> progreso) =>
            EjecutarAccionEnPersona(p => AccionAgregarUsuario(PrepararPersona, p), progreso);

        private void PrepararPersona()
        {
            Persona.Contactos = ContactosPersona.ToList();
            NormalizarContactos(Persona.Contactos);

            Usuario = Persona;
        }

        private Task<int> AgregarNegocio(IProgress<(double, string)> progreso) =>
            EjecutarAccionEnNegocio(p => AccionAgregarUsuario(PrepararNegocio, p), progreso);

        private void PrepararNegocio()
        {
            Negocio.Contactos = ContactosNegocio.ToList();
            Negocio.Representante.Contactos = ContactosRepresentante.ToList();

            NormalizarContactos(Negocio.Contactos);
            NormalizarContactos(Negocio.Representante.Contactos);

            Usuario = Negocio;
        }

        private async Task<int> AccionAgregarUsuario(Action preparar, IProgress<(double, string)> progreso)
        {
            var resultado = await Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.Create())
                {
                    preparar();

                    var usuario = AgregarUsuario(progreso);

                    baseDeDatos.SaveChanges();

                    progreso.Report((100.0, "Listo."));

                    return usuario.Id;
                }
            }).ConfigureAwait(true);

            var _ = Navegador.Navegar("Contratos/Agregar", resultado);

            return resultado;
        }

        private Usuario AgregarUsuario(IProgress<(double, string)> progreso = null)
        {
            progreso.Report((0.0, "Buscando duplicados..."));

            if (OperacionesUsuarios.BuscarDuplicados(Usuario, UsuariosRepo) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

            var usuario = UsuariosRepo.Agregar(Usuario);

            progreso.Report((50.0, "Agregando usuario..."));

            return usuario;
        }
    }
}

using AguaSB.Datos;
using AguaSB.Nucleo;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Linq;
using MoreLinq;
using System;
using AguaSB.Navegacion;
using AguaSB.Notificaciones;
using GGUtils.MVVM.Async;
using System.Waf.Applications;

namespace AguaSB.Usuarios.ViewModels
{
    public enum TipoUsuario { Persona, Negocio }

    public class Editar : ModificarUsuarioBase
    {
        #region Campos
        private TipoUsuario tipoUsuario;

        private bool mostrarProgreso;
        private string textoProgreso;
        #endregion

        #region Propiedades
        public TipoUsuario TipoUsuario
        {
            get { return tipoUsuario; }
            set { SetProperty(ref tipoUsuario, value); }
        }

        public bool MostrarProgreso
        {
            get { return mostrarProgreso; }
            set { SetProperty(ref mostrarProgreso, value); }
        }

        public string TextoProgreso
        {
            get { return textoProgreso; }
            set { SetProperty(ref textoProgreso, value); }
        }
        #endregion

        #region Comandos
        public AsyncDelegateCommand<int> EditarPersonaComando { get; }
        public AsyncDelegateCommand<int> EditarNegocioComando { get; }

        public DelegateCommand CancelarComando { get; }
        #endregion

        #region Dependencias
        private INavegador Navegador { get; }
        private IAdministradorNotificaciones Notificaciones { get; }
        #endregion

        public Editar(IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContacto> tiposContactoRepo, INavegador navegador, IAdministradorNotificaciones notificaciones)
            : base(usuariosRepo, tiposContactoRepo)
        {
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));

            EditarPersonaComando = new AsyncDelegateCommand<int>(EditarPersona, TodosCamposPersonaValidos);
            EditarNegocioComando = new AsyncDelegateCommand<int>(EditarNegocio, TodosCamposNegocioValidos);
            CancelarComando = new DelegateCommand(Cancelar, () => PuedeReestablecerNegocio && PuedeReestablecerPersona);

            ConfigurarVerificador(() => new ICommand[] { EditarPersonaComando, EditarNegocioComando });
        }

        protected override async Task Entrar(object arg)
        {
            if (arg is int id)
            {
                try
                {
                    MostrarProgreso = true;
                    TextoProgreso = "Obteniendo información de usuario...";
                    var usuario = await Task.Run(() => UsuariosRepo.Datos.SingleOrDefault(_ => _.Id == id)).ConfigureAwait(true);

                    if (usuario != null)
                    {
                        if (usuario is Persona persona)
                        {
                            Usuario = Persona = persona;
                            TipoUsuario = TipoUsuario.Persona;

                            ContactosPersona.Clear();
                            persona.Contactos.ForEach(ContactosPersona.Add);
                        }
                        else if (usuario is Negocio negocio)
                        {
                            Usuario = Negocio = negocio;
                            TipoUsuario = TipoUsuario.Negocio;

                            ContactosNegocio.Clear();
                            negocio.Contactos.ForEach(ContactosNegocio.Add);

                            negocio.Representante.Contactos.Clear();
                            negocio.Representante.Contactos.ForEach(ContactosRepresentante.Add);
                        }
                        else
                        {
                            // TODO: Log tipo desconocido
                        }
                    }
                    else
                    {
                        Notificaciones.Lanzar(new NotificacionError()
                        {
                            Titulo = "Error",
                            Clase = "Base de datos",
                            Descripcion = $"No se encontró al usuario con Id = {id}."
                        });

                        var _ = Navegador.Navegar("Usuarios/Listado", null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ocurrió un error al buscar al usuario con Id {id}: {ex.Message}");
                    Notificaciones.Lanzar(new NotificacionError()
                    {
                        Titulo = "Error",
                        Clase = "Base de datos",
                        Descripcion = $"Ocurrió un error al conectar con la base de datos: {ex.Message}"
                    });

                    var _ = Navegador.Navegar("Usuarios/Listado", null);
                    // TODO: Log error
                }
                finally
                {
                    MostrarProgreso = false;
                }
            }
            else
            {
                Console.WriteLine("No se recibió un parámetro en Editar Usuario");
                var _ = Navegador.Navegar("Usuarios/Listado", null);
                //TODO: Log
            }

            await base.Entrar(arg).ConfigureAwait(true);
        }

        private void Cancelar()
        {
            var _ = Navegador.Navegar("Usuarios/Listado", Usuario?.NombreCompleto);

            Persona = null;
            Negocio = null;
            Usuario = null;

            new[] { ContactosPersona, ContactosNegocio, ContactosRepresentante }
            .ForEach(__ => __.Clear());
        }

        private Task<int> EditarPersona(IProgress<(double, string)> progreso)
        {
            CancelarComando.RaiseCanExecuteChanged();
            return EjecutarAccionEnPersona(AccionEditarUsuario, progreso);
        }

        private Task<int> EditarNegocio(IProgress<(double, string)> progreso)
        {
            CancelarComando.RaiseCanExecuteChanged();
            return EjecutarAccionEnNegocio(AccionEditarUsuario, progreso);
        }

        private async Task<int> AccionEditarUsuario(IProgress<(double, string)> progreso)
        {
            var resultado = await EditarUsuario(progreso).ConfigureAwait(true);

            var _ = Navegador.Navegar("Usuarios/Listado", resultado);

            return resultado;
        }

        private Task<int> EditarUsuario(IProgress<(double, string)> progreso = null) => Task.Run(() =>
        {
            progreso.Report((0.0, "Buscando duplicados..."));

            // TODO: Reactivar en release
            /*
            if (OperacionesUsuarios.BuscarDuplicados(Usuario, UsuariosRepo) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");*/

            progreso.Report((50.0, "Actualizando información de usuario..."));

            var usuario = UsuariosRepo.Actualizar(Usuario).Result;

            progreso.Report((100.0, "Completado."));

            return usuario.Id;
        });
    }
}

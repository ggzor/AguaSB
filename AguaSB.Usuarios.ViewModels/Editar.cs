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
using Mehdime.Entity;
using AguaSB.Operaciones;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace AguaSB.Usuarios.ViewModels
{
    public enum TipoUsuario { Persona, Negocio }

    public class Editar : ModificarUsuarioBase
    {
        #region Campos
        private TipoUsuario tipoUsuario;
        private readonly List<Contacto> contactosPersonaBorrados = new List<Contacto>();
        private readonly List<Contacto> contactosNegocioBorrados = new List<Contacto>();
        private readonly List<Contacto> contactosRepresentanteBorrados = new List<Contacto>();

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
        private IRepositorio<Contacto> ContactosRepo { get; }

        private INavegador Navegador { get; }
        private IAdministradorNotificaciones Notificaciones { get; }
        #endregion

        public Editar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contacto> contactosRepo,
            IRepositorio<TipoContacto> tiposContactoRepo, INavegador navegador, IAdministradorNotificaciones notificaciones)
            : base(ambito, usuariosRepo, tiposContactoRepo)
        {
            ContactosRepo = contactosRepo ?? throw new ArgumentNullException(nameof(contactosRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));

            EditarPersonaComando = new AsyncDelegateCommand<int>(EditarPersona, TodosCamposPersonaValidos);
            EditarNegocioComando = new AsyncDelegateCommand<int>(EditarNegocio, TodosCamposNegocioValidos);
            CancelarComando = new DelegateCommand(Cancelar, () => PuedeReestablecerNegocio && PuedeReestablecerPersona);

            ConfigurarVerificador(() => new ICommand[] { EditarPersonaComando, EditarNegocioComando });

            NotifyCollectionChangedEventHandler ManejarBorrados(List<Contacto> listaContactos) =>
                (object _, NotifyCollectionChangedEventArgs evento) =>
                {
                    if (evento.Action == NotifyCollectionChangedAction.Remove)
                        evento.OldItems.OfType<Contacto>().ForEach(c => listaContactos.Add(c));
                };

            ContactosPersona.CollectionChanged += ManejarBorrados(contactosPersonaBorrados);
            ContactosNegocio.CollectionChanged += ManejarBorrados(contactosNegocioBorrados);
            ContactosRepresentante.CollectionChanged += ManejarBorrados(contactosRepresentanteBorrados);
        }

        protected override async Task Entrar(object arg)
        {
            if (arg is int id)
            {
                try
                {
                    MostrarProgreso = true;
                    TextoProgreso = "Obteniendo información de usuario...";

                    var accion = await Task.Run<Action>(() =>
                    {
                        using (var baseDeDatos = Ambito.CreateReadOnly())
                        {
                            Contacto[] ObtenerContactos(ICollection<Contacto> contactos) =>
                                contactos.Select(_ => new { _.Id, _.TipoContacto, _.Informacion }).ToArray()
                                    .Select(_ => new Contacto { Id = _.Id, TipoContacto = _.TipoContacto, Informacion = _.Informacion })
                                    .ToArray();

                            var usuario = UsuariosRepo.Datos.SingleOrDefault(_ => _.Id == id);

                            if (usuario != null)
                            {
                                if (usuario is Persona persona)
                                {
                                    Usuario = Persona = persona;
                                    TipoUsuario = TipoUsuario.Persona;

                                    var contactos = ObtenerContactos(persona.Contactos);

                                    return () =>
                                    {
                                        contactosPersonaBorrados.Clear();
                                        ContactosPersona.Clear();

                                        contactos.ForEach(ContactosPersona.Add);
                                    };
                                }
                                else if (usuario is Negocio negocio)
                                {
                                    Usuario = Negocio = negocio;
                                    TipoUsuario = TipoUsuario.Negocio;

                                    var contactosNegocio = ObtenerContactos(negocio.Contactos);
                                    var contactosRepresentante = ObtenerContactos(negocio.Representante.Contactos);

                                    return () =>
                                    {
                                        contactosNegocioBorrados.Clear();
                                        ContactosNegocio.Clear();
                                        contactosRepresentanteBorrados.Clear();
                                        ContactosRepresentante.Clear();

                                        contactosNegocio.ForEach(ContactosNegocio.Add);
                                        contactosRepresentante.ForEach(ContactosRepresentante.Add);
                                    };
                                }
                                else
                                {
                                    throw new Exception("No se pudo cargar el usuario porque su tipo es desconocido.");
                                }
                            }
                            else
                            {
                                return () =>
                                {
                                    Notificaciones.Lanzar(new NotificacionError()
                                    {
                                        Titulo = "Error",
                                        Clase = "Base de datos",
                                        Descripcion = $"No se encontró al usuario con Id = {id}."
                                    });

                                    var _ = Navegador.Navegar("Usuarios/Listado", null);
                                };
                            }
                        }
                    }).ConfigureAwait(true);

                    accion();
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
            var _ = Navegador.Navegar("Usuarios/Listado", null);

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);

            new[] { ContactosPersona, ContactosNegocio, ContactosRepresentante }
            .ForEach(__ => __.Clear());
        }

        private Task<int> EditarPersona(IProgress<(double, string)> progreso)
        {
            CancelarComando.RaiseCanExecuteChanged();

            return EjecutarAccionEnPersona(p => AccionEditarUsuario(p, AccionEditarPersona), progreso);
        }

        private Task<Usuario> AccionEditarPersona(IProgress<(double, string)> progreso)
        {
            Persona persona = CopiarPersona(Persona);

            return EditarUsuario(progreso, persona, new[] { ((Usuario)persona, ContactosPersona.ToList()) }, contactosPersonaBorrados);
        }

        private Persona CopiarPersona(Persona persona) => new Persona
        {
            Id = persona.Id,
            Nombre = persona.Nombre,
            ApellidoPaterno = persona.ApellidoPaterno,
            ApellidoMaterno = persona.ApellidoMaterno,
            FechaRegistro = persona.FechaRegistro
        };

        private Task<int> EditarNegocio(IProgress<(double, string)> progreso)
        {
            CancelarComando.RaiseCanExecuteChanged();

            return EjecutarAccionEnNegocio(p => AccionEditarUsuario(p, AccionEditarNegocio), progreso);
        }

        private Task<Usuario> AccionEditarNegocio(IProgress<(double, string)> progreso)
        {
            var negocio = new Negocio
            {
                Id = Negocio.Id,
                FechaRegistro = Negocio.FechaRegistro,
                Nombre = Negocio.Nombre,
                Rfc = Negocio.Rfc
            };

            var representante = CopiarPersona(Negocio.Representante);

            return EditarUsuario(progreso, negocio,
                new[] { ((Usuario)negocio, ContactosNegocio.ToList()), (representante, ContactosRepresentante.ToList()) }, contactosNegocioBorrados.Concat(contactosRepresentanteBorrados));
        }

        private async Task<int> AccionEditarUsuario(IProgress<(double, string)> progreso, Func<IProgress<(double, string)>, Task<Usuario>> accion)
        {
            var resultado = await accion(progreso).ConfigureAwait(true);

            var _ = Navegador.Navegar("Usuarios/Listado", resultado.NombreCompleto);

            return resultado.Id;
        }

        private Task<Usuario> EditarUsuario(IProgress<(double, string)> progreso, Usuario principal, IEnumerable<(Usuario, List<Contacto>)> contactos,
            IEnumerable<Contacto> contactosBorrados) => Task.Run(() =>
        {
            using (var baseDeDatos = Ambito.Create())
            {
                progreso.Report((0.0, "Buscando duplicados..."));

                if (OperacionesUsuarios.BuscarDuplicados(Usuario, UsuariosRepo) is Usuario u)
                    throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

                progreso.Report((20.0, "Recopilando información..."));

                contactos.SelectMany(lista => NormalizarContactos(lista.Item2))
                    .Concat(contactosBorrados)
                    .Where(_ => _.Id != 0)
                    .ForEach(c => ContactosRepo.Eliminar(c));

                progreso.Report((60.0, "Actualizando información..."));
                contactos.Select(_ => _.Item1).ForEach(usuario => UsuariosRepo.Actualizar(usuario));

                contactos.ForEach(_ => _.Item2.ForEach(contacto => contacto.Usuario = _.Item1));

                var todosLosContactos = contactos.SelectMany(_ => _.Item2);

                todosLosContactos.Where(_ => _.Id != 0).ForEach(c => ContactosRepo.Actualizar(c));
                todosLosContactos.Where(_ => _.Id == 0).ForEach(c => ContactosRepo.Agregar(c));

                baseDeDatos.SaveChanges();

                progreso.Report((100.0, "Listo."));
                return principal;
            }
        });
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;
using MoreLinq;

using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using AguaSB.Navegacion;
using AguaSB.Datos;
using AguaSB.Operaciones;

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos
        // Serán inicializadas junto con los comandos.
        private Persona persona;
        private Negocio negocio;

        private bool puedeReestablecerPersona = true;
        private bool mostrarMensajeErrorPersona = true;

        private bool puedeReestablecerNegocio = true;
        private bool mostrarMensajeErrorNegocio = true;

        private IEnumerable<TipoContacto> tiposContacto;
        private ObservableCollection<Contacto> contactosPersona = new ObservableCollection<Contacto>();
        private ObservableCollection<Contacto> contactosNegocio = new ObservableCollection<Contacto>();
        private ObservableCollection<Contacto> contactosRepresentante = new ObservableCollection<Contacto>();
        #endregion

        #region Propiedades
        private Usuario Usuario { get; set; }

        public Persona Persona
        {
            get { return persona; }
            set { SetProperty(ref persona, value); }
        }

        public Negocio Negocio
        {
            get { return negocio; }
            set { SetProperty(ref negocio, value); }
        }

        public bool MostrarMensajeErrorPersona
        {
            get { return mostrarMensajeErrorPersona; }
            set { SetProperty(ref mostrarMensajeErrorPersona, value); }
        }

        public bool MostrarMensajeErrorNegocio
        {
            get { return mostrarMensajeErrorNegocio; }
            set { SetProperty(ref mostrarMensajeErrorNegocio, value); }
        }

        public bool PuedeReestablecerPersona
        {
            get { return puedeReestablecerPersona; }
            set
            {
                SetProperty(ref puedeReestablecerPersona, value);
                ReestablecerPersonaComando.RaiseCanExecuteChanged();
            }
        }

        public bool PuedeReestablecerNegocio
        {
            get { return puedeReestablecerNegocio; }
            set
            {
                SetProperty(ref puedeReestablecerNegocio, value);
                ReestablecerNegocioComando.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<TipoContacto> TiposContacto
        {
            get { return tiposContacto; }
            set { SetProperty(ref tiposContacto, value); }
        }

        public ObservableCollection<Contacto> ContactosPersona
        {
            get { return contactosPersona; }
            set { SetProperty(ref contactosPersona, value); }
        }

        public ObservableCollection<Contacto> ContactosNegocio
        {
            get { return contactosNegocio; }
            set { SetProperty(ref contactosNegocio, value); }
        }

        public ObservableCollection<Contacto> ContactosRepresentante
        {
            get { return contactosRepresentante; }
            set { SetProperty(ref contactosRepresentante, value); }
        }
        #endregion

        #region Comandos
        private DelegateCommand reestablecerPersonaComando;
        private DelegateCommand reestablecerNegocioComando;

        private AsyncDelegateCommand<int> agregarPersonaComando;
        private AsyncDelegateCommand<int> agregarNegocioComando;

        public DelegateCommand ReestablecerPersonaComando
        {
            get { return reestablecerPersonaComando; }
            private set { SetProperty(ref reestablecerPersonaComando, value); }
        }

        public DelegateCommand ReestablecerNegocioComando
        {
            get { return reestablecerNegocioComando; }
            private set { SetProperty(ref reestablecerNegocioComando, value); }
        }

        public AsyncDelegateCommand<int> AgregarPersonaComando
        {
            get { return agregarPersonaComando; }
            private set { SetProperty(ref agregarPersonaComando, value); }
        }

        public AsyncDelegateCommand<int> AgregarNegocioComando
        {
            get { return agregarNegocioComando; }
            set { SetProperty(ref agregarNegocioComando, value); }
        }
        #endregion

        #region Dependencias
        private IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<TipoContacto> TiposContactoRepo { get; }

        public INavegador Navegador { get; }
        #endregion

        public event EventHandler Enfocar;

        public INodo Nodo { get; }

        public Agregar(IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContacto> tiposContactoRepo, INavegador navegador)
        {
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            TiposContactoRepo = tiposContactoRepo ?? throw new ArgumentNullException(nameof(tiposContactoRepo));

            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            Nodo = new Nodo { Entrada = Entrar };
        }

        private void ConfigurarComandos()
        {
            ReestablecerPersonaComando = new DelegateCommand(() =>
            {
                Persona = new Persona();

                ContactosPersona.Clear();
                IntentarAgregarTelefono(ContactosPersona);

                MostrarMensajeErrorPersona = false;

                InvocarEnfocar();
            }, () => PuedeReestablecerPersona);

            ReestablecerNegocioComando = new DelegateCommand(() =>
            {
                Negocio = new Negocio()
                {
                    Representante = new Persona()
                };

                ContactosNegocio.Clear();
                ContactosRepresentante.Clear();

                IntentarAgregarTelefono(ContactosNegocio);
                IntentarAgregarTelefono(ContactosRepresentante);

                MostrarMensajeErrorNegocio = false;

                InvocarEnfocar();
            }, () => PuedeReestablecerNegocio);

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, PuedeAgregarPersona);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, PuedeAgregarNegocio);
        }

        private async void InvocarEnfocar()
        {
            await Task.Delay(100).ConfigureAwait(true);

            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private void ConfigurarUniones() =>
            new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[] {
                Persona, Negocio, Negocio.Representante,
                }
                .Concat(new[] { ContactosPersona, ContactosNegocio, ContactosRepresentante }.SelectMany(_ => _)),
                () => new INotifyPropertyChanged[] { this, ContactosPersona, ContactosNegocio, ContactosRepresentante },
                () => new[] { AgregarPersonaComando, AgregarNegocioComando });

        private void IntentarAgregarTelefono(ObservableCollection<Contacto> lista)
        {
            if (TiposContacto.SingleOrDefault(_ => _.Nombre == "Teléfono") is TipoContacto tipoContacto)
                lista.Add(new Contacto { TipoContacto = tipoContacto });
        }

        private Task Entrar(object arg)
        {
            TiposContacto = TiposContactoRepo.Datos.ToList();

            ConfigurarComandos();
            ConfigurarUniones();

            InvocarEnfocar();

            return Task.CompletedTask;
        }

        private bool PuedeAgregarPersona() =>
            UtileriasErrores.NingunoTieneErrores(
                ((INotifyDataErrorInfo)Persona)
                .Concat(ContactosPersona)
                .ToArray())
            && !Persona.TieneCamposRequeridosVacios;

        private bool PuedeAgregarNegocio() =>
            UtileriasErrores.NingunoTieneErrores(
                new INotifyDataErrorInfo[] { Negocio, Negocio.Representante }
                .Concat(ContactosNegocio)
                .Concat(ContactosRepresentante)
                .ToArray())
            && !Negocio.TieneCamposRequeridosVacios && !Negocio.Representante.TieneCamposRequeridosVacios;

        private Task<int> AgregarPersona(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorPersona = true;

            Persona.Contactos = ContactosPersona.ToList();

            return AgregarUsuarioManejando(Persona, b => PuedeReestablecerPersona = b, ReestablecerPersonaComando, progreso);
        }

        private Task<int> AgregarNegocio(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorNegocio = true;

            Negocio.Contactos = ContactosNegocio.ToList();
            Negocio.Representante.Contactos = ContactosRepresentante.ToList();

            return AgregarUsuarioManejando(Negocio, b => PuedeReestablecerNegocio = b, ReestablecerNegocioComando, progreso);
        }

        private async Task<int> AgregarUsuarioManejando(Usuario usuario, Action<bool> puedeReestablecer, ICommand reestablecer, IProgress<(double, string)> progreso)
        {
            foreach (var contacto in usuario.Contactos.ToArray())
            {
                if (string.IsNullOrWhiteSpace(contacto?.Informacion))
                    usuario.Contactos.Remove(contacto);
            }

            puedeReestablecer(false);
            Usuario = usuario;

            try
            {
                var resultado = await AgregarUsuario(progreso).ConfigureAwait(true);

                puedeReestablecer(true);
                reestablecer.Execute(null);

                await Navegador.Navegar("Contratos/Agregar", resultado).ConfigureAwait(true);

                return resultado;
            }
            finally
            {
                puedeReestablecer(true);
            }
        }

        private async Task<int> AgregarUsuario(IProgress<(double, string)> progreso = null)
        {
            progreso.Report((0.0, "Buscando duplicados..."));

            if (await OperacionesUsuarios.BuscarDuplicadosAsync(Usuario, UsuariosRepo).ConfigureAwait(false) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

            progreso.Report((50.0, "Agregando usuario..."));

            var usuario = await UsuariosRepo.Agregar(Usuario).ConfigureAwait(false);

            progreso.Report((100.0, "Completado."));

            return usuario.Id;
        }
    }
}

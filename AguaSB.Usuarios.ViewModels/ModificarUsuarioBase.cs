using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.Utilerias.IO;
using AguaSB.ViewModels;
using Mehdime.Entity;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

namespace AguaSB.Usuarios.ViewModels
{
    public abstract class ModificarUsuarioBase : ValidatableModel, IViewModel
    {
        #region Campos
        private Persona persona;
        private Negocio negocio;

        private IEnumerable<string> sugerenciasNombres;
        private IEnumerable<string> sugerenciasApellidos;

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
        protected Usuario Usuario { get; set; }

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

        public IEnumerable<string> SugerenciasNombres
        {
            get { return sugerenciasNombres; }
            set { SetProperty(ref sugerenciasNombres, value); }
        }

        public IEnumerable<string> SugerenciasApellidos
        {
            get { return sugerenciasApellidos; }
            set { SetProperty(ref sugerenciasApellidos, value); }
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
        public DelegateCommand ReestablecerPersonaComando { get; }
        public DelegateCommand ReestablecerNegocioComando { get; }
        #endregion

        #region Eventos
        public event EventHandler Enfocar;
        #endregion

        #region Dependencias
        protected IDbContextScopeFactory Ambito { get; }

        protected IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<TipoContacto> TiposContactoRepo { get; }
        #endregion

        public INodo Nodo { get; }

        protected ModificarUsuarioBase(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContacto> tiposContactoRepo)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            TiposContactoRepo = tiposContactoRepo ?? throw new ArgumentNullException(nameof(tiposContactoRepo));

            Nodo = new Nodo { Entrada = Entrar, PrimeraEntrada = Inicializar };

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

            Task.Factory.StartNew(CargarSugerencias);
        }

        protected void ConfigurarVerificador(Func<IEnumerable<ICommand>> comandos)
        {
            if (comandos == null)
                throw new ArgumentNullException(nameof(comandos));

            var verificador = new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[] { Persona, Negocio, Negocio?.Representante }
                    .Where(_ => _ != null)
                    .Concat(new[] { ContactosPersona, ContactosNegocio, ContactosRepresentante }
                    .SelectMany(_ => _)),
                    () => new INotifyPropertyChanged[] { this, ContactosPersona, ContactosNegocio, ContactosRepresentante },
                    comandos);

            new[] { contactosPersona, ContactosNegocio, ContactosRepresentante }
                .ForEach(_ => _.CollectionChanged += (__, ___) => verificador.RegistrarObservadoresDeCambios());
        }

        private void CargarSugerencias()
        {
            try
            {
                IEnumerable<string> nombresArchivo = Enumerable.Empty<string>();
                IEnumerable<string> apellidosArchivo = Enumerable.Empty<string>();

                try
                {
                    nombresArchivo = Configuracion.Cargar<List<string>>(nameof(SugerenciasNombres), subdirectorio: "Datos");
                    apellidosArchivo = Configuracion.Cargar<List<string>>(nameof(SugerenciasApellidos), subdirectorio: "Datos");
                }
                catch (FileNotFoundException archivo)
                {
                    // TODO: Log
                    Console.WriteLine($"No se encontró el archivo de sugerencias: {archivo.FileName}");
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ocurrió un error al leer los archivos de sugerencias: {ex.Message}");
                }

                SugerenciasNombres = nombresArchivo.OrderBy(_ => _).Distinct().ToList();
                SugerenciasApellidos = apellidosArchivo.OrderBy(_ => _).Distinct().ToList();

                using (var baseDeDatos = Ambito.CreateReadOnly())
                {
                    IEnumerable<Persona> personas = UsuariosRepo.Datos.OfType<Persona>();

                    var nombresBaseDeDatos = personas.Select(_ => _.Nombre)
                        .OrderBy(_ => _)
                        .Distinct()
                        .ToList();

                    var apellidosBaseDeDatos = personas.Select(_ => _.ApellidoPaterno)
                        .Concat(personas.Select(_ => _.ApellidoMaterno))
                        .OrderBy(_ => _)
                        .Distinct()
                        .ToList();

                    SugerenciasNombres = SugerenciasNombres.OrderedMerge(nombresBaseDeDatos).ToList();
                    SugerenciasApellidos = SugerenciasApellidos.OrderedMerge(apellidosBaseDeDatos).ToList();
                }

                try
                {
                    Configuracion.Guardar(SugerenciasNombres, nombre: nameof(SugerenciasNombres), subdirectorio: "Datos", indentar: false);
                    Configuracion.Guardar(SugerenciasApellidos, nombre: nameof(SugerenciasApellidos), subdirectorio: "Datos", indentar: false);
                }
                catch (IOException ex)
                {
                    Console.WriteLine($"Ocurrió un error al guardar las sugerencias: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocurrió un error al obtener las sugerencias: {ex.Message}");
            }
        }

        protected virtual async Task Inicializar()
        {
            TiposContacto = await Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.CreateReadOnly())
                    return TiposContactoRepo.Datos.ToList();
            }).ConfigureAwait(true);

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);
        }

        protected virtual Task Entrar(object arg)
        {
            InvocarEnfocar();
            return Task.CompletedTask;
        }

        private void IntentarAgregarTelefono(ObservableCollection<Contacto> lista)
        {
            if (TiposContacto.SingleOrDefault(_ => _.Nombre == "Teléfono") is TipoContacto tipoContacto)
                lista.Add(new Contacto { TipoContacto = tipoContacto });
        }

        protected async void InvocarEnfocar()
        {
            await Task.Delay(300).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        protected bool TodosCamposPersonaValidos() =>
            UtileriasErrores.NingunoTieneErrores(
                ContactosPersona.Cast<INotifyDataErrorInfo>()
                .Concat(Persona)
                .Where(_ => _ != null)
                .ToArray())
            && (!Persona?.TieneCamposRequeridosVacios ?? false);

        protected bool TodosCamposNegocioValidos() =>
            UtileriasErrores.NingunoTieneErrores(
                new INotifyDataErrorInfo[] { Negocio, Negocio?.Representante }
                .Concat(ContactosNegocio)
                .Concat(ContactosRepresentante)
                .Where(_ => _ != null)
                .ToArray())
            && (!Negocio?.TieneCamposRequeridosVacios ?? false) && (!Negocio?.Representante?.TieneCamposRequeridosVacios ?? false);

        protected Task<int> EjecutarAccionEnPersona(Func<IProgress<(double, string)>, Task<int>> accion, IProgress<(double, string)> progreso) =>
            EjecutarAccionManejando(accion, Persona, b => MostrarMensajeErrorPersona = b, b => PuedeReestablecerPersona = b, ReestablecerPersonaComando, progreso);

        protected Task<int> EjecutarAccionEnNegocio(Func<IProgress<(double, string)>, Task<int>> accion, IProgress<(double, string)> progreso) =>
            EjecutarAccionManejando(accion, Negocio, b => MostrarMensajeErrorNegocio = b, b => PuedeReestablecerNegocio = b, ReestablecerNegocioComando, progreso);

        protected IEnumerable<Contacto> NormalizarContactos(ICollection<Contacto> contactos)
        {
            var tiposContactos = TiposContactoRepo.Datos.ToArray();
            var borrados = new List<Contacto>();

            contactos.Where(contacto => string.IsNullOrWhiteSpace(contacto?.Informacion)).ToList()
                .ForEach(c => { contactos.Remove(c); borrados.Add(c); });

            foreach (var contacto in contactos)
                contacto.TipoContacto = tiposContactos.Single(tc => tc.Nombre == contacto.TipoContacto.Nombre);

            return borrados;
        }

        private async Task<int> EjecutarAccionManejando(Func<IProgress<(double, string)>, Task<int>> accion,
            Usuario usuario, Action<bool> mostrarMensajeError, Action<bool> puedeReestablecer, ICommand reestablecer, IProgress<(double, string)> progreso)
        {
            mostrarMensajeError(true);
            puedeReestablecer(false);
            Usuario = usuario;

            try
            {
                int resultado = await accion(progreso).ConfigureAwait(true);

                puedeReestablecer(true);
                reestablecer.Execute(null);

                return resultado;
            }
            finally
            {
                puedeReestablecer(true);
            }
        }
    }
}

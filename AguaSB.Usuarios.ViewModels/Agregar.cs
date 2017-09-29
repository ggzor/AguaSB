using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using MoreLinq;
using GGUtils.MVVM.Async;

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
                puedeReestablecerPersona = value;
                ReestablecerPersonaComando.RaiseCanExecuteChanged();
            }
        }

        public bool PuedeReestablecerNegocio
        {
            get { return puedeReestablecerNegocio; }
            set
            {
                puedeReestablecerNegocio = value;
                ReestablecerNegocioComando.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region Comandos

        public DelegateCommand ReestablecerPersonaComando { get; private set; }
        public DelegateCommand ReestablecerNegocioComando { get; private set; }

        public AsyncDelegateCommand<int> AgregarPersonaComando { get; private set; }
        public AsyncDelegateCommand<int> AgregarNegocioComando { get; private set; }

        #endregion

        public INodo<IProveedorServicios> Nodo { get; }

        public Agregar()
        {
            Nodo = new NodoHoja<IProveedorServicios>()
            {
                Inicializacion = Inicializacion
            };

            ConfigurarComandos();

            this.ToObservableProperties().Subscribe(RegistrarObservadoresDeCambios);

            // Registrar observadores por primera vez
            RaisePropertyChanged(nameof(Persona));
        }

        #region Servicios

        public IRepositorio<Usuario> Usuarios { get; private set; }

        #endregion

        private Task Inicializacion(IProveedorServicios servicios)
        {
            Usuarios = servicios.Repositorios.Usuarios;

            return Task.CompletedTask;
        }

        private IDisposable ObservadorDePropiedades;

        private void RegistrarObservadoresDeCambios((object Source, PropertyChangedEventArgs PropiedadCambiada) parametro)
        {
            ObservadorDePropiedades?.Dispose();

            var propiedadesAObservar = new INotifyPropertyChanged[] {
                Persona, Negocio, Negocio.Representante,
                Persona.Contactos.FirstOrDefault(),
                Negocio.Contactos.FirstOrDefault(),
                Negocio.Representante.Contactos.FirstOrDefault()
            }.Where(elem => elem != null)
            .Select(NotifyPropertyChangedEx.ToObservableProperties);

            ObservadorDePropiedades = Observable.Merge(propiedadesAObservar).ObserveOnDispatcher().Subscribe(_ => VerificarPuedeEjecutar());
        }

        private void VerificarPuedeEjecutar() => new ICommand[]
        {
            AgregarPersonaComando, AgregarNegocioComando
        }.ForEach(comando =>
        {
            switch (comando)
            {
                case DelegateCommand c:
                    c.RaiseCanExecuteChanged();
                    break;
                case AsyncDelegateCommand<int> c:
                    c.RaiseCanExecuteChanged();
                    break;
                default:
                    break;
            };
        });

        private void ConfigurarComandos()
        {
            var telefono = new TipoContacto() { Nombre = "Teléfono", ExpresionRegular = @"\A[0-9 ]*\z" };

            ReestablecerPersonaComando = new DelegateCommand(() =>
            {
                var persona = new Persona();
                persona.Contactos.Add(new Contacto() { TipoContacto = telefono });

                Persona = persona;

                VerificarPuedeEjecutar();
                MostrarMensajeErrorPersona = false;
            }, () => PuedeReestablecerPersona);

            ReestablecerNegocioComando = new DelegateCommand(() =>
            {
                var negocio = new Negocio()
                {
                    Representante = new Persona()
                };

                negocio.Contactos.Add(new Contacto() { TipoContacto = telefono });
                negocio.Representante.Contactos.Add(new Contacto() { TipoContacto = telefono });

                Negocio = negocio;

                VerificarPuedeEjecutar();
                MostrarMensajeErrorNegocio = false;
            }, () => PuedeReestablecerNegocio);

            ReestablecerPersonaComando.Execute(null);
            ReestablecerNegocioComando.Execute(null);

            AgregarPersonaComando = new AsyncDelegateCommand<int>(AgregarPersona, PuedeAgregarPersona);
            AgregarNegocioComando = new AsyncDelegateCommand<int>(AgregarNegocio, PuedeAgregarNegocio);
        }

        private bool PuedeAgregarPersona() =>
            NingunoTieneErrores(Persona, Persona.Contactos.First())
            && !Persona.TieneCamposRequeridosVacios;

        private bool PuedeAgregarNegocio() =>
            NingunoTieneErrores(Negocio, Negocio.Representante, Negocio.Contactos.First(), Negocio.Representante.Contactos.First())
            && !Negocio.TieneCamposRequeridosVacios && !Negocio.Representante.TieneCamposRequeridosVacios;

        private bool NingunoTieneErrores(params INotifyDataErrorInfo[] objetos) =>
            objetos
                .Select(i => !i.HasErrors)
                .Aggregate((v1, v2) => v1 && v2);


        private async Task<int> AgregarPersona(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorPersona = true;
            return await AgregarUsuarioManejando(Persona, b => PuedeReestablecerPersona = b, ReestablecerPersonaComando, progreso);
        }

        private async Task<int> AgregarNegocio(IProgress<(double, string)> progreso)
        {
            MostrarMensajeErrorNegocio = true;
            return await AgregarUsuarioManejando(Negocio, b => PuedeReestablecerNegocio = b, ReestablecerNegocioComando, progreso);
        }

        private async Task<int> AgregarUsuarioManejando(Usuario usuario, Action<bool> puedeReestablecer, ICommand reestablecer, IProgress<(double, string)> progreso)
        {
            puedeReestablecer(false);
            Usuario = usuario;

            try
            {
                var resultado = await AgregarUsuario(progreso);

                puedeReestablecer(true);
                reestablecer.Execute(null);

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

            if (OperacionesUsuarios.BuscarDuplicados(Usuario, Usuarios) is Usuario u)
                throw new Exception($"El usuario \"{u.NombreCompleto}\" ya está registrado en el sistema.");

            progreso.Report((50.0, "Agregando usuario..."));

            var usuario = await Usuarios.Agregar(Usuario);

            progreso.Report((100.0, "Completado."));

            return usuario.Id;
        }
    }
}

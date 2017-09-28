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

namespace AguaSB.Usuarios.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos

        // Serán inicializadas junto con los comandos.
        private Persona persona;
        private Negocio negocio;

        private bool puedeReestablecerPersona = true;
        private bool puedeReestablecerNegocio = true;

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
                Entrada = Entrando
            };

            ConfigurarComandos();

            this.ToObservableProperties().Subscribe(RegistrarObservadoresDeCambios);

            // Registrar observadores por primera vez
            RaisePropertyChanged(nameof(Persona));
        }

        private async Task Entrando(ColaNavegacion arg)
        {

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
            PuedeReestablecerPersona = false;
            Usuario = Persona;

            var resultado = await AgregarUsuario(progreso);

            PuedeReestablecerPersona = true;
            ReestablecerPersonaComando.Execute(null);

            return resultado;
        }

        private async Task<int> AgregarNegocio(IProgress<(double, string)> progreso)
        {
            PuedeReestablecerNegocio = false;
            Usuario = Negocio;

            var resultado = await AgregarUsuario(progreso);

            PuedeReestablecerNegocio = true;
            ReestablecerNegocioComando.Execute(null);

            return resultado;
        }

        private async Task<int> AgregarUsuario(IProgress<(double, string)> progreso = null)
        {
            async Task Reportar(string textoProgreso)
            {
                progreso?.Report((0.0, textoProgreso));
                await Task.Delay(5000);
            }

            await Reportar("Comenzando ejecución...");
            await Reportar("Buscando duplicados...");
            await Reportar("Registrando usuario...");

            return 0;
        }
    }
}

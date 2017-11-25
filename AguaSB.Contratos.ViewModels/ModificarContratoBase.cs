using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Notificaciones;
using AguaSB.Nucleo;
using AguaSB.Nucleo.Datos;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using Mehdime.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

namespace AguaSB.Contratos.ViewModels
{
    public abstract class ModificarContratoBase : ValidatableModel, IViewModel
    {
        #region Campos
        private bool mostrarProgreso;
        private string textoProgreso;

        private bool mostrarMensajeError = true;
        private bool puedeReestablecer = true;

        private Contrato contrato;

        private DateTime pagadoHasta;

        private IEnumerable<TipoContrato> tiposContrato;
        private IDictionary<Seccion, IList<Calle>> callesAgrupadas;
        #endregion

        #region Propiedades
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

        public bool MostrarMensajeError
        {
            get { return mostrarMensajeError; }
            set { SetProperty(ref mostrarMensajeError, value); }
        }

        public bool PuedeReestablecer
        {
            get { return puedeReestablecer; }
            set
            {
                SetProperty(ref puedeReestablecer, value);
                ReestablecerComando.RaiseCanExecuteChanged();
            }
        }

        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }

        public DateTime PagadoHasta
        {
            get { return pagadoHasta; }
            set { SetProperty(ref pagadoHasta, value); }
        }

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return tiposContrato; }
            set { SetProperty(ref tiposContrato, value); }
        }

        public IEnumerable<string> SugerenciasMedidasToma { get; } = new[] { "1/2", "1", "1 1/2", "2" };

        public IDictionary<Seccion, IList<Calle>> CallesAgrupadas
        {
            get { return callesAgrupadas; }
            set { SetProperty(ref callesAgrupadas, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; }

        public DelegateCommand NavegarA { get; }
        #endregion

        #region Dependencias
        protected IDbContextScopeFactory Ambito { get; }

        protected IRepositorio<Usuario> UsuariosRepo { get; }
        protected IRepositorio<Contrato> ContratosRepo { get; }
        protected IRepositorio<TipoContrato> TiposContratoRepo { get; }
        protected IRepositorio<Seccion> SeccionesRepo { get; }
        protected IRepositorio<Calle> CallesRepo { get; }

        protected IAdministradorNotificaciones Notificaciones { get; }
        protected INavegador Navegador { get; }
        #endregion

        #region Eventos
        public event EventHandler Enfocar;
        #endregion

        public INodo Nodo { get; }

        protected ModificarContratoBase(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Seccion> seccionesRepo, IRepositorio<Calle> callesRepo, IAdministradorNotificaciones notificaciones, INavegador navegador)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));

            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            ContratosRepo = contratosRepo ?? throw new ArgumentNullException(nameof(contratosRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            CallesRepo = callesRepo ?? throw new ArgumentNullException(nameof(callesRepo));

            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            ReestablecerComando = new DelegateCommand(Reestablecer, () => PuedeReestablecer);
            NavegarA = new DelegateCommand(o => Navegador.Navegar((string)o, null));

            Nodo = new Nodo() { PrimeraEntrada = Inicializar, Entrada = Entrar };

            Reestablecer();
        }

        protected void ConfigurarVerificador(Func<IEnumerable<ICommand>> comandos) =>
        new VerificadorPropiedades(this,
            () => new INotifyDataErrorInfo[]
            {
                        this, Contrato, Contrato?.Domicilio
            },
            Enumerable.Empty<INotifyPropertyChanged>,
            comandos);

        private void Reestablecer()
        {
            MostrarMensajeError = false;
            Contrato = new Contrato()
            {
                Domicilio = new Domicilio()
            };

            PagadoHasta = Fecha.MesDe(Fecha.Ahora);
        }

        private async Task Inicializar()
        {
            await Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.CreateReadOnly())
                {
                    TextoProgreso = "Cargando información de secciones y calles...";
                    MostrarProgreso = true;

                    CallesAgrupadas = Domicilios.CallesAgrupadas(SeccionesRepo);

                    TiposContrato = (from tipo in TiposContratoRepo.Datos
                                     orderby tipo.Nombre
                                     select tipo).ToList();

                    MostrarProgreso = false;
                }
            }).ConfigureAwait(true);

            InvocarEnfocar();
        }

        protected abstract Task Entrar(object arg);

        protected bool TodosCamposValidos() =>
            UtileriasErrores.NingunoTieneErrores(this, Contrato, Contrato.Domicilio)
            && !Contrato.TieneCamposRequeridosVacios
            && !Contrato.Domicilio.TieneCamposRequeridosVacios
            && Contrato.Usuario != null;

        protected async void InvocarEnfocar()
        {
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        protected async Task<int> EjecutarAccion(Func<IProgress<(double, string)>, Task<Contrato>> accion, IProgress<(double, string)> progreso)
        {
            MostrarMensajeError = true;
            PuedeReestablecer = false;

            try
            {
                var resultado = await accion(progreso).ConfigureAwait(true);

                var _ = Navegador.Navegar("Usuarios/Listado", Contrato.Usuario.NombreCompleto);

                PuedeReestablecer = true;
                Reestablecer();

                return resultado.Id;
            }
            finally
            {
                PuedeReestablecer = true;
            }
        }
    }
}

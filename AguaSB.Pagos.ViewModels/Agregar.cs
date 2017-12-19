using AguaSB.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using System.Waf.Foundation;
using AguaSB.Nucleo;
using AguaSB.Pagos.ViewModels.Dtos;
using AguaSB.Utilerias;
using System.Reactive.Linq;
using Mehdime.Entity;
using AguaSB.Datos;
using System.Waf.Applications;
using MoreLinq;
using System.ComponentModel;
using AguaSB.Notificaciones;
using AguaSB.Pagos.ViewModels.Notificaciones;

namespace AguaSB.Pagos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Configuración
        private static readonly TimeSpan TiempoEsperaBusqueda = TimeSpan.FromSeconds(1.5);
        private const int CantidadOpciones = 12;
        #endregion

        #region Campos
        private bool buscando;
        private bool buscandoInformacionPago;
        private ResultadosBusquedaUsuarios busqueda;
        private string textoBusqueda;
        private OpcionesPago opcionesPago;
        private bool usuarioSeleccionado;
        #endregion

        #region Propiedades
        public bool Buscando
        {
            get { return buscando; }
            set { SetProperty(ref buscando, value); }
        }

        public bool BuscandoInformacionPago
        {
            get { return buscandoInformacionPago; }
            set { SetProperty(ref buscandoInformacionPago, value); }
        }

        public ResultadosBusquedaUsuarios Busqueda
        {
            get { return busqueda; }
            set { SetProperty(ref busqueda, value); }
        }

        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { SetProperty(ref textoBusqueda, value); }
        }

        public OpcionesPago OpcionesPago
        {
            get { return opcionesPago; }
            set { SetProperty(ref opcionesPago, value); }
        }

        public bool UsuarioSeleccionado
        {
            get { return usuarioSeleccionado; }
            set { SetProperty(ref usuarioSeleccionado, value); }
        }

        public ControladorCubierta ControladorCubierta { get; } = new ControladorCubierta();
        #endregion

        #region Comandos
        public DelegateCommand BuscarEnListadoComando { get; set; }
        public DelegateCommand PagarSeleccionComando { get; }
        public DelegateCommand PagarOtraCantidadComando { get; }
        #endregion

        #region Servicios
        private IDbContextScopeFactory Ambito { get; }

        private IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<Contrato> ContratosRepo { get; }
        private IRepositorio<Tarifa> TarifasRepo { get; set; }
        private IRepositorio<Pago> PagosRepo { get; set; }

        private INavegador Navegador { get; }
        private IAdministradorNotificaciones Notificaciones { get; }
        #endregion

        #region Eventos
        public event EventHandler IniciandoBusqueda;
        public event EventHandler Enfocar;
        public event EventHandler EncontradoUsuarioUnico;
        public event EventHandler UsuarioCambiado;
        #endregion

        public INodo Nodo { get; }

        public Agregar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<Pago> pagosRepo,
            IRepositorio<Tarifa> tarifasRepo, INavegador navegador, IAdministradorNotificaciones notificaciones)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));

            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            ContratosRepo = contratosRepo ?? throw new ArgumentNullException(nameof(contratosRepo));
            PagosRepo = pagosRepo ?? throw new ArgumentNullException(nameof(pagosRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));

            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));

            Nodo = new Nodo { Entrada = Entrar };

            BuscarEnListadoComando = new DelegateCommand(() => Navegador.Navegar("Usuarios/Listado", TextoBusqueda));
            PagarSeleccionComando = new DelegateCommand(PagarSeleccion, PuedePagarSeleccion);
            PagarOtraCantidadComando = new DelegateCommand(PagarOtraCantidad, PuedePagarPorPropiedades);

            ActivarEscuchaCambioTexto();

            var verificador = new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[] { OpcionesPago?.PagoPorPropiedades }
                      .Where(o => o != null),
                () => new INotifyPropertyChanged[] { OpcionesPago?.PagoPorRangos, OpcionesPago?.PagoPorPropiedades }
                      .Concat(OpcionesPago?.PagoPorRangos?.PagosContratos ?? Enumerable.Empty<INotifyPropertyChanged>())
                      .Where(obs => obs != null),
                () => new[] { PagarSeleccionComando, PagarOtraCantidadComando });

            UsuarioSeleccionado = false;
        }

        private void ActivarEscuchaCambioTexto()
        {
            this.ToObservableProperties()
                .Where(p => p.Args.PropertyName == nameof(TextoBusqueda))
                .Throttle(TiempoEsperaBusqueda)
                .Select(_ => TextoBusqueda.Trim())
                .DistinctUntilChanged()
                .Where(_ => !string.IsNullOrWhiteSpace(TextoBusqueda))
                .ObserveOnDispatcher()
                .Subscribe(async texto => await ObtenerOpciones(texto).ConfigureAwait(true));
        }

        private Task Entrar(object arg)
        {
            Busqueda = new ResultadosBusquedaUsuarios(CantidadOpciones);
            TextoBusqueda = string.Empty;

            UsuarioSeleccionado = false;
            InvocarEnfocar();

            return Task.CompletedTask;
        }

        private async void InvocarEnfocar()
        {
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private readonly Sincronizador ObtencionOpcionesUsuario = new Sincronizador();

        private async Task ObtenerOpciones(string nombreUsuario)
        {
            Task<ResultadosBusquedaUsuarios> BuscarOpciones = Task.Run(() =>
            {
                var busqueda = new ResultadosBusquedaUsuarios(CantidadOpciones);

                using (var baseDatos = Ambito.CreateReadOnly())
                    busqueda.Buscar(UsuariosRepo.Datos, nombreUsuario);

                return busqueda;
            });

            var id = ObtencionOpcionesUsuario.ObtenerId();

            Buscando = true;
            IniciandoBusqueda?.Invoke(this, EventArgs.Empty);

            try
            {
                var resultadoBusqueda = await BuscarOpciones.ConfigureAwait(true);

                ObtencionOpcionesUsuario.Intentar(id,
                    () => Busqueda = resultadoBusqueda);


                if (Busqueda.TotalResultados == 1)
                {
                    EncontradoUsuarioUnico?.Invoke(this, EventArgs.Empty);
                    await SeleccionarUsuario(Busqueda.Resultados.Single()).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                //TODO: Log
                Console.WriteLine("Excepcion: " + ex.Message);
            }

            Buscando = false;
        }

        private readonly Sincronizador ObtencionInformacionPagos = new Sincronizador();

        public async Task SeleccionarUsuario(Usuario usuario)
        {
            Task<OpcionesPago> ObtenerInformacion() => Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.CreateReadOnly())
                {
                    var tarifas = TarifasRepo.Datos.OrderBy(t => t.FechaRegistro).ToArray();

                    return OpcionesPago.Para(UsuariosRepo.Datos.Single(u => u.Id == usuario.Id), tarifas);
                }
            });

            var id = ObtencionInformacionPagos.ObtenerId();

            UsuarioSeleccionado = false;
            BuscandoInformacionPago = true;

            var informacion = await ObtenerInformacion().ConfigureAwait(true);

            ObtencionInformacionPagos.Intentar(id,
                () => OpcionesPago = informacion);

            BuscandoInformacionPago = false;
            UsuarioSeleccionado = true;

            // Invocar usuario cambiado
            await Task.Delay(200).ConfigureAwait(true);
            UsuarioCambiado?.Invoke(this, EventArgs.Empty);
        }

        private bool TieneContratoSeleccionado() => UsuarioSeleccionado && OpcionesPago?.PagoPorRangos.PagoContratoSeleccionado != null;
        private bool PuedePagarSeleccion() => TieneContratoSeleccionado() && OpcionesPago?.PagoPorRangos.PagoContratoSeleccionado?.RangoPagoSeleccionado != null;

        private bool PuedePagarPorPropiedades() => TieneContratoSeleccionado() && OpcionesPago.PagoPorPropiedades != null && !OpcionesPago.PagoPorPropiedades.HasErrors;

        private void PagarOtraCantidad() => ManejarPago(OpcionesPago.PagoPorPropiedades);

        private void PagarSeleccion() => ManejarPago(OpcionesPago.PagoPorRangos);

        private async void ManejarPago(OpcionPago opcionPago)
        {
            try
            {
                using (var cubierta = ControladorCubierta.Mostrar("Realizando pago..."))
                {
                    var (pago, domicilio) = await HacerPago(opcionPago);

                    Notificaciones.Lanzar(new PagoRealizado(pago, domicilio));
                }

                UsuarioSeleccionado = false;
                InvocarEnfocar();
            }
            catch (Exception ex)
            {
                Notificaciones.Lanzar(new NotificacionError { Titulo = "Error", Descripcion = ex.Message, Clase = "Pagos" });
                //TODO: Log
            }
        }

        private Task<(Pago Pago, string Domicilio)> HacerPago(OpcionPago opcionPago) => Task.Run(() =>
        {
            var pago = opcionPago.GenerarPago();

            using (var baseDeDatos = Ambito.Create())
            {
                pago.Contrato = ContratosRepo.Datos.Single(c => c.Id == pago.Contrato.Id);

                var domicilioContrato = pago.Contrato.ToString();

                PagosRepo.Agregar(pago);

                baseDeDatos.SaveChanges();

                return (pago, domicilioContrato);
            }
        });
    }
}

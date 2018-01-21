using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Linq;

using System.Waf.Applications;
using System.Waf.Foundation;
using MoreLinq;

using AguaSB.Navegacion;
using AguaSB.Notificaciones;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;

using AguaSB.Operaciones;
using AguaSB.Operaciones.Adeudos;
using AguaSB.Operaciones.Contratos;
using AguaSB.Operaciones.Montos;
using AguaSB.Operaciones.Pagos;
using AguaSB.Operaciones.Usuarios;

using AguaSB.Pagos.ViewModels.Dtos;
using AguaSB.Pagos.ViewModels.Notificaciones;
using AguaSB.Servicios;

namespace AguaSB.Pagos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Configuración
        private static readonly TimeSpan TiempoEsperaBusqueda = TimeSpan.FromMilliseconds(300);
        private const int CantidadOpciones = 10;
        #endregion

        #region Campos
        private string textoBusqueda;
        private Busqueda<ResultadoBusquedaUsuariosConContrato> busquedaOpcionesUsuarios;

        private OpcionesPago opcionesPago;
        private bool usuarioSeleccionado;
        private Pago pagoAnterior;
        #endregion

        #region Propiedades
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { SetProperty(ref textoBusqueda, value); }
        }

        public Busqueda<ResultadoBusquedaUsuariosConContrato> BusquedaOpcionesUsuarios
        {
            get { return busquedaOpcionesUsuarios; }
            set { SetProperty(ref busquedaOpcionesUsuarios, value); }
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

        public Pago PagoAnterior
        {
            get { return pagoAnterior; }
            set { SetProperty(ref pagoAnterior, value); DeshacerPagoAnteriorComando.RaiseCanExecuteChanged(); }
        }
        #endregion

        #region Comandos
        public DelegateCommand BuscarEnListadoComando { get; set; }
        public DelegateCommand PagarSeleccionComando { get; }
        public DelegateCommand PagarOtraCantidadComando { get; }
        public DelegateCommand DeshacerPagoAnteriorComando { get; }
        #endregion

        #region Dependencias
        public IInformador<Pago> Informador { get; }
        private IProveedorAmbito Proveedor { get; }

        private IProveedorSugerenciasUsuarios SugerenciasUsuarios { get; }
        private IOperacionesPagos Pagos { get; }
        private ILocalizadorContratos Contratos { get; }
        private ICalculadorAdeudos Adeudos { get; }
        private ICalculadorMontos Montos { get; }

        private INavegador Navegador { get; }
        private IAdministradorNotificaciones Notificaciones { get; }
        #endregion

        #region Eventos
        public event EventHandler Enfocar;
        #endregion

        public INodo Nodo { get; }

        public Agregar(IInformador<Pago> informador, IProveedorAmbito proveedor, IProveedorSugerenciasUsuarios sugerenciasUsuarios, IOperacionesPagos pagos,
            ILocalizadorContratos contratos, ICalculadorAdeudos adeudos, ICalculadorMontos montos, INavegador navegador,
            IAdministradorNotificaciones notificaciones)
        {
            Informador = informador ?? throw new ArgumentNullException(nameof(informador));
            Proveedor = proveedor ?? throw new ArgumentNullException(nameof(proveedor));
            SugerenciasUsuarios = sugerenciasUsuarios ?? throw new ArgumentNullException(nameof(sugerenciasUsuarios));
            Pagos = pagos ?? throw new ArgumentNullException(nameof(pagos));
            Contratos = contratos ?? throw new ArgumentNullException(nameof(contratos));
            Adeudos = adeudos ?? throw new ArgumentNullException(nameof(adeudos));
            Montos = montos ?? throw new ArgumentNullException(nameof(montos));

            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));

            Nodo = new Nodo { Entrada = Entrar };

            BuscarEnListadoComando = new DelegateCommand(() => Navegador.Navegar("Usuarios/Listado", TextoBusqueda));
            PagarSeleccionComando = new DelegateCommand(PagarSeleccion, PuedePagarSeleccion);
            PagarOtraCantidadComando = new DelegateCommand(PagarPorPropiedades, PuedePagarPorPropiedades);
            DeshacerPagoAnteriorComando = new DelegateCommand(DeshacerPagoAnterior, () => PagoAnterior != null);

            ActivarEscuchaCambioTexto();

            var verificador = new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[] { OpcionesPago?.PagoPorPropiedades }
                      .Where(o => o != null),
                () => new INotifyPropertyChanged[] { OpcionesPago?.PagoPorRangos, OpcionesPago?.PagoPorPropiedades }
                      .Concat(OpcionesPago?.PagoPorRangos?.PagosContratos ?? Enumerable.Empty<INotifyPropertyChanged>())
                      .Where(obs => obs != null),
                () => new[] { PagarSeleccionComando, PagarOtraCantidadComando });
        }

        private void ActivarEscuchaCambioTexto()
        {
            var valoresBusqueda = from valor in this.ObservableProperty(vm => vm.TextoBusqueda).Throttle(TiempoEsperaBusqueda)
                                  where !string.IsNullOrWhiteSpace(valor)
                                  select valor.Trim();

            valoresBusqueda.DistinctUntilChanged()
                .ObserveOnDispatcher()
                .Subscribe(async texto => await ObtenerOpcionesUsuarios(texto).ConfigureAwait(true));
        }

        private Task Entrar(object arg)
        {
            BusquedaOpcionesUsuarios = Busquedas.Nueva(ResultadoBusquedaUsuariosConContrato.Vacio);
            TextoBusqueda = arg?.ToString() ?? string.Empty;

            UsuarioSeleccionado = false;
            InvocarEnfocar();

            return Task.CompletedTask;
        }

        private async void InvocarEnfocar()
        {
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private readonly Sincronizador ObtencionOpcionesUsuarios = new Sincronizador();

        private async Task ObtenerOpcionesUsuarios(string nombreUsuario)
        {
            var id = ObtencionOpcionesUsuarios.ObtenerId();

            var busqueda = Busquedas.Nueva(ResultadoBusquedaUsuariosConContrato.Vacio);

            ObtencionOpcionesUsuarios.Intentar(id,
                () => BusquedaOpcionesUsuarios = busqueda);

            await ObtencionOpcionesUsuarios.IntentarAsync(id, () => busqueda.BuscarAsync(() =>
            {
                using (Proveedor.CrearSoloLectura())
                    return new ResultadoBusquedaUsuariosConContrato(SugerenciasUsuarios.Obtener(nombreUsuario), CantidadOpciones);
            })).ConfigureAwait(true);
        }

        private readonly Sincronizador ObtencionInformacionPagos = new Sincronizador();

        public async Task SeleccionarUsuario(Usuario usuario)
        {
            Task<OpcionesPago> ObtenerInformacion() => Task.Run(() =>
            {
                using (Proveedor.CrearSoloLectura())
                {
                    var adeudosContratos = Contratos.ObtenerContratos(usuario)
                        .Select(Adeudos.ObtenerAdeudo)
                        .ToArray();

                    return new OpcionesPago(usuario, adeudosContratos, Montos);
                }
            });

            var id = ObtencionInformacionPagos.ObtenerId();

            UsuarioSeleccionado = false;

            using (ControladorCubierta.Mostrar("Obteniendo información de usuario..."))
            {
                var informacion = await ObtenerInformacion().ConfigureAwait(true);

                ObtencionInformacionPagos.Intentar(id,
                    () => OpcionesPago = informacion);
            }

            UsuarioSeleccionado = true;
        }

        private bool TieneContratoSeleccionado() => UsuarioSeleccionado && OpcionesPago?.PagoPorRangos.PagoContratoSeleccionado != null;
        private bool PuedePagarSeleccion() => TieneContratoSeleccionado() && OpcionesPago?.PagoPorRangos.PagoContratoSeleccionado?.RangoPagoSeleccionado != null;
        private void PagarSeleccion() => ManejarPago(OpcionesPago.PagoPorRangos);

        private bool PuedePagarPorPropiedades() => TieneContratoSeleccionado() && OpcionesPago.PagoPorPropiedades != null && !OpcionesPago.PagoPorPropiedades.HasErrors;
        private void PagarPorPropiedades() => ManejarPago(OpcionesPago.PagoPorPropiedades);

        private async void ManejarPago(OpcionPago opcionPago)
        {
            Task HacerPago(Pago pago) => Task.Run(() =>
            {
                using (var baseDeDatos = Proveedor.CrearConTransaccion())
                {
                    Pagos.Hacer(pago);

                    Informador.Informar(pago);

                    baseDeDatos.GuardarCambios();
                }
            });

            try
            {
                using (var cubierta = ControladorCubierta.Mostrar("Realizando pago..."))
                {
                    var pagoARealizar = opcionPago.GenerarPago();

                    await HacerPago(pagoARealizar).ConfigureAwait(true);

                    Notificaciones.Lanzar(new PagoRealizado(pagoARealizar));
                }

                OpcionesPago = null;
                UsuarioSeleccionado = false;
                InvocarEnfocar();
            }
            catch (Exception ex)
            {
                Notificaciones.Lanzar(new NotificacionError
                {
                    Titulo = "Error",
                    Descripcion = ex.Message,
                    Clase = "Pagos"
                });
                //TODO: Log
            }
        }

        private async void DeshacerPagoAnterior()
        {
            try
            {
                await Task.Run(() => Pagos.Deshacer(PagoAnterior)).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                Notificaciones.Lanzar(new NotificacionError
                {
                    Clase = "Pagos",
                    Descripcion = ex.Message,
                    Titulo = "Error"
                });
            }
        }
    }
}

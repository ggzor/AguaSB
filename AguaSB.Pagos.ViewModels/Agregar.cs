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
using AguaSB.Servicios;
using AguaSB.Nucleo.Datos;
using AguaSB.Operaciones;
using AguaSB.Operaciones.Pagos;
using AguaSB.Operaciones.Usuarios;
using System.Collections.Generic;

namespace AguaSB.Pagos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Configuración
        private static readonly TimeSpan TiempoEsperaBusqueda = TimeSpan.FromSeconds(1.5);
        private const int CantidadOpciones = 15;
        #endregion

        #region Campos
        private Busqueda<ResultadoBusquedaUsuariosConContrato> busquedaOpcionesUsuarios;

        private string textoBusqueda;
        private OpcionesPago opcionesPago;
        private bool usuarioSeleccionado;
        private Pago pagoAnterior;
        #endregion

        #region Propiedades
        public Busqueda<ResultadoBusquedaUsuariosConContrato> BusquedaOpcionesUsuarios
        {
            get { return busquedaOpcionesUsuarios; }
            set { SetProperty(ref busquedaOpcionesUsuarios, value); }
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

        #region Servicios
        private IProveedorAmbito Proveedor { get; }

        private IProveedorSugerenciasUsuarios SugerenciasUsuarios { get; }
        private IOperacionesPagos Pagos { get; }

        private IDbContextScopeFactory Ambito { get; }

        private IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<Contrato> ContratosRepo { get; }
        private IRepositorio<Tarifa> TarifasRepo { get; }
        private IRepositorio<Pago> PagosRepo { get; }
        private IRepositorio<TipoNota> TiposNotaRepo { get; }

        private INavegador Navegador { get; }
        private IAdministradorNotificaciones Notificaciones { get; }
        private IInformador<Pago> Informador { get; }
        #endregion

        #region Eventos
        public event EventHandler Enfocar;
        public event EventHandler UsuarioCambiado;
        #endregion

        public INodo Nodo { get; }

        public Agregar(IProveedorAmbito proveedor, IProveedorSugerenciasUsuarios sugerenciasUsuarios, IOperacionesPagos pagos, IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<Pago> pagosRepo,
            IRepositorio<Tarifa> tarifasRepo, IRepositorio<TipoNota> tiposNotaRepo, INavegador navegador,
            IAdministradorNotificaciones notificaciones, IInformador<Pago> informador)
        {
            Proveedor = proveedor ?? throw new ArgumentNullException(nameof(proveedor));
            SugerenciasUsuarios = sugerenciasUsuarios ?? throw new ArgumentNullException(nameof(sugerenciasUsuarios));
            Pagos = pagos ?? throw new ArgumentNullException(nameof(pagos));

            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));

            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            ContratosRepo = contratosRepo ?? throw new ArgumentNullException(nameof(contratosRepo));
            PagosRepo = pagosRepo ?? throw new ArgumentNullException(nameof(pagosRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));
            TiposNotaRepo = tiposNotaRepo ?? throw new ArgumentNullException(nameof(tiposNotaRepo));

            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));

            Informador = informador ?? throw new ArgumentNullException(nameof(informador));

            Nodo = new Nodo { Entrada = Entrar };

            BuscarEnListadoComando = new DelegateCommand(() => Navegador.Navegar("Usuarios/Listado", TextoBusqueda));
            PagarSeleccionComando = new DelegateCommand(PagarSeleccion, PuedePagarSeleccion);
            PagarOtraCantidadComando = new DelegateCommand(PagarOtraCantidad, PuedePagarPorPropiedades);
            DeshacerPagoAnteriorComando = new DelegateCommand(DeshacerPagoAnterior, () => PagoAnterior != null);

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
                    return new ResultadoBusquedaUsuariosConContrato(SugerenciasUsuarios.Obtener(nombreUsuario), 15);
            }));

            await ObtencionOpcionesUsuarios.IntentarAsync(id, () =>
            {
                var resultados = busqueda.Resultado.Resultados;
                if (resultados.Count == 1)
                    return SeleccionarUsuario(resultados.Single());
                else
                    return Task.CompletedTask;
            });
        }

        private readonly Sincronizador ObtencionInformacionPagos = new Sincronizador();

        public async Task SeleccionarUsuario(Usuario usuario)
        {
            Task<OpcionesPago> ObtenerInformacion() => Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.CreateReadOnly())
                {
                    var tarifas = Tarifas.Obtener(TarifasRepo);

                    return OpcionesPago.Para(UsuariosRepo.Datos.Single(u => u.Id == usuario.Id), tarifas);
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
                    var pagoARealizar = opcionPago.GenerarPago();

                    using (var baseDeDatos = Proveedor.Crear())
                    {
                        Pagos.Hacer(pagoARealizar);
                        baseDeDatos.GuardarCambios();
                    }

                    var (pago, domicilio) = await HacerPago(pagoARealizar);

                    Notificaciones.Lanzar(new PagoRealizado(pago, domicilio));
                    PagoAnterior = pago;
                }

                OpcionesPago = null;
                UsuarioSeleccionado = false;
                InvocarEnfocar();
            }
            catch (Exception ex)
            {
                Notificaciones.Lanzar(new NotificacionError { Titulo = "Error", Descripcion = ex.Message, Clase = "Pagos" });
                //TODO: Log
            }
        }

        private Task<(Pago Pago, string Domicilio)> HacerPago(Pago pago) => Task.Run(() =>
        {
            using (var baseDeDatos = Ambito.Create())
            {
                var datos = (from Contrato in ContratosRepo.Datos
                             where Contrato.Id == pago.Contrato.Id
                             select new { Contrato, Contrato.Usuario }).Single();

                pago.Contrato = datos.Contrato;
                pago.Contrato.Usuario = datos.Usuario;

                var domicilioContrato = pago.Contrato.ToString();

                PagosRepo.Agregar(pago);
                Informador.Informar(pago);

                baseDeDatos.SaveChanges();

                return (pago, domicilioContrato);
            }
        });

        private async void DeshacerPagoAnterior()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (ControladorCubierta.Mostrar("Deshaciendo pago..."))
                    using (var baseDeDatos = Ambito.Create())
                    {
                        var pago = PagosRepo.Datos.Where(p => p.Id == PagoAnterior.Id).Single();
                        PagosRepo.Eliminar(pago);

                        baseDeDatos.SaveChanges();
                    }
                }).ConfigureAwait(true);

                await SeleccionarUsuario(PagoAnterior.Contrato.Usuario).ConfigureAwait(true);
                PagoAnterior = null;
            }
            catch (Exception ex)
            {
                Notificaciones.Lanzar(new NotificacionError
                {
                    Clase = "Pagos",
                    Descripcion = $"No se pudo deshacer el pago: {ex.Message}",
                    Titulo = "Error"
                });
            }
        }
    }
}

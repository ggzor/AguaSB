using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Nucleo.Datos;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Utilerias;
using AguaSB.Utilerias.Solicitudes;
using AguaSB.ViewModels;

using MoreLinq;
using Mehdime.Entity;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using AguaSB.Reportes;
using AguaSB.Notificaciones;
using AguaSB.Operaciones.Adeudos;

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Configuracion
        private static readonly TimeSpan TiempoEsperaBusqueda = TimeSpan.FromSeconds(1.5);
        #endregion

        #region Campos
        private IEnumerable<Seccion> secciones;
        private IEnumerable<Calle> calles;
        private IEnumerable<ClaseContrato> clasesContrato;
        private IEnumerable<TipoContrato> tiposContrato;
        private IEnumerable<IAgrupador> agrupadores;
        private Ordenamientos ordenamientos;

        private IAgrupador agrupador;
        private Columnas columnas;
        private Filtros filtros;
        private IOrdenamiento ordenamiento;
        private string textoBusqueda;

        private Busqueda busqueda;
        private bool mostrarCubierta;
        private string textoCubierta;
        #endregion

        #region Propiedades
        public IEnumerable<Seccion> Secciones
        {
            get { return secciones; }
            set { SetProperty(ref secciones, value); }
        }

        public IEnumerable<Calle> Calles
        {
            get { return calles; }
            set { SetProperty(ref calles, value); }
        }

        public IEnumerable<ClaseContrato> ClasesContrato
        {
            get { return clasesContrato; }
            set { SetProperty(ref clasesContrato, value); }
        }

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return tiposContrato; }
            set { SetProperty(ref tiposContrato, value); }
        }

        public IEnumerable<IAgrupador> Agrupadores
        {
            get { return agrupadores; }
            set { SetProperty(ref agrupadores, value); }
        }

        public Ordenamientos Ordenamientos
        {
            get { return ordenamientos; }
            set { SetProperty(ref ordenamientos, value); }
        }

        public IAgrupador Agrupador
        {
            get { return agrupador; }
            set { SetProperty(ref agrupador, value); }
        }

        public Columnas Columnas
        {
            get { return columnas; }
            set { SetProperty(ref columnas, value); }
        }

        public Filtros Filtros
        {
            get { return filtros; }
            set { SetProperty(ref filtros, value); }
        }

        public IOrdenamiento Ordenamiento
        {
            get { return ordenamiento; }
            set { ordenamiento = value; OrdenamientoCambiado?.Invoke(this, value); RaisePropertyChanged(); }
        }

        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { SetProperty(ref textoBusqueda, value); }
        }

        public Busqueda Busqueda
        {
            get { return busqueda; }
            set { SetProperty(ref busqueda, value); }
        }

        public bool MostrarCubierta
        {
            get { return mostrarCubierta; }
            set { SetProperty(ref mostrarCubierta, value); }
        }

        public string TextoCubierta
        {
            get { return textoCubierta; }
            set { SetProperty(ref textoCubierta, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand DesactivarFiltrosComando { get; }
        public DelegateCommand MostrarColumnasTodasComando { get; }

        public DelegateCommand AgregarContratoComando { get; }
        public DelegateCommand EditarUsuarioComando { get; }
        public DelegateCommand ExportarComando { get; }

        public DelegateCommand PagarUsuarioComando { get; }
        #endregion

        #region Eventos
        public event EventHandler<IOrdenamiento> OrdenamientoCambiado;
        public event EventHandler Enfocar;

        public ICalculadorAdeudos CalculadorAdeudos { get; }
        #endregion

        #region Dependencias
        private IDbContextScopeFactory Ambito { get; }

        public IRepositorio<Usuario> UsuariosRepo { get; }
        public IRepositorio<Seccion> SeccionesRepo { get; }
        public IRepositorio<TipoContrato> TiposContratoRepo { get; }
        public IRepositorio<Tarifa> TarifasRepo { get; }

        private INavegador Navegador { get; }
        private IGeneradorTablas GeneradorTablas { get; }
        private IAdministradorNotificaciones AdministradorNotificaciones { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado(ICalculadorAdeudos calculadorAdeudos, IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Tarifa> tarifasRepo, INavegador navegador, IGeneradorTablas generadorTablas, IAdministradorNotificaciones administradorNotificaciones)
        {
            CalculadorAdeudos = calculadorAdeudos ?? throw new ArgumentNullException(nameof(calculadorAdeudos));
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            GeneradorTablas = generadorTablas ?? throw new ArgumentNullException(nameof(generadorTablas));
            AdministradorNotificaciones = administradorNotificaciones ?? throw new ArgumentNullException(nameof(administradorNotificaciones));

            Nodo = new Nodo { PrimeraEntrada = Inicializar, Entrada = Entrar };

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            MostrarColumnasTodasComando = new DelegateCommand(MostrarColumnasTodas);

            AgregarContratoComando = new DelegateCommand(AgregarContrato);
            EditarUsuarioComando = new DelegateCommand(EditarUsuario);
            ExportarComando = new DelegateCommand(Exportar);

            PagarUsuarioComando = new DelegateCommand(PagarUsuario);

            Columnas = new Columnas();

            Columnas.FechaRegistro.Activo = false;
            Columnas.UltimoPago.Activo = false;

            Filtros = new Filtros();

            Ordenamientos = new Ordenamientos();

            Busqueda = new Busqueda();

            RegistrarUniones();
        }

        private void RegistrarUniones()
        {
            var filtros = Filtros.ToObservableProperties()
                .Select(_ => _.Args.PropertyName)
                .Where(_ => _ != nameof(Filtros.NombreCompleto));

            // Observar listados
            filtros.Where(p => p == nameof(Filtros.Seccion))
                .ObserveOnDispatcher()
                .Subscribe(_ => ActualizarListadoDeCalles());

            filtros.Where(p => p == nameof(Filtros.ClaseContrato))
                .ObserveOnDispatcher()
                .Subscribe(_ => ActualizarListadoDeTiposContrato());

            // Post operaciones
            this.ToObservableProperties()
                .Select(_ => _.Args.PropertyName)
                .Where(_ => _ == nameof(Agrupador) || _ == nameof(Ordenamiento))
                .Throttle(TimeSpan.FromMilliseconds(500))
                .Subscribe(_ => Buscar(ModoBusqueda.PostOperaciones));

            var textoBusqueda = this.ToObservableProperties()
                .Select(_ => _.Args.PropertyName)
                .Where(_ => _ == nameof(TextoBusqueda) && !string.IsNullOrWhiteSpace(TextoBusqueda));

            new[] { filtros, textoBusqueda }.Merge().Throttle(TiempoEsperaBusqueda)
                .Skip(1)
                .ObserveOnDispatcher()
                .Subscribe(_ => Buscar());
        }

        private void MostrarColumnasTodas() => Columnas = new Columnas();

        private void DesactivarFiltros() => Filtros.Todos.ForEach(_ => _.Activo = false);

        private void AgregarContrato(object o)
        {
            if (o is ResultadoUsuario u)
                Navegador.Navegar("Contratos/Agregar", u.Usuario.Id);
        }

        private void EditarUsuario(object o)
        {
            if (o is ResultadoUsuario u)
                Navegador.Navegar("Usuarios/Editar", u.Usuario.Id);
        }

        private async void Exportar(object o)
        {
            MostrarCubierta = true;
            TextoCubierta = "Generando archivo...";

            if (Busqueda != null)
                await GenerarArchivo(Busqueda.Resultados);

            MostrarCubierta = false;
        }

        private void PagarUsuario(object o)
        {
            if (o is ResultadoUsuario u)
                Navegador.Navegar("Pagos/Agregar", u.Usuario.NombreCompleto);
        }

        private static readonly string[] encabezado = { "Id", "Nombre", "Pagado Hasta", "Adeudo", "Sección", "Calle", "Número", "Tipo de contrato", "Último pago" };

        private Task GenerarArchivo(IEnumerable resultados) => Task.Run(() =>
        {
            try
            {
                if (resultados == null || !resultados.OfType<object>().Any())
                    throw new Exception("No hay resultados para exportar.");

                if (Busqueda.Buscando == true)
                    throw new Exception("Espere a que finalice la búsqueda.");

                var nombre = $"{Fecha.Ahora.ToString("yyyy-MM-dd hh.mm.ss")} - Listado de Usuarios";
                var libro = GeneradorTablas.CrearLibro(nombre);

                if (resultados.OfType<ResultadoUsuario>().FirstOrDefault()?.NoEsTitulo == true)
                {
                    var tabla = libro.CrearNueva("Usuarios");
                    var escritor = tabla.Escritor;

                    escritor.ColorEncabezado = new RGB(0x11, 0x9E, 0xDA);
                    escritor.ColorTextoEncabezado = new RGB(255, 255, 255);
                    escritor.EscribirEncabezado(encabezado);

                    int i = 2;
                    foreach (var usuario in resultados.OfType<ResultadoUsuario>().Where(u => u.Domicilio != null))
                    {
                        tabla[1, i] = usuario.Usuario.Id;
                        tabla[2, i] = usuario.Usuario.NombreCompleto;

                        tabla.Formato[3, i] = "mmmm yyyy";
                        tabla[3, i] = usuario.UltimoMesPagado;

                        tabla.Formato[4, i] = "$0.00";
                        tabla[4, i] = usuario.Adeudo;

                        tabla[5, i] = usuario.Domicilio.Calle.Seccion.Nombre;
                        tabla[6, i] = usuario.Domicilio.Calle.Nombre;

                        if (int.TryParse(usuario.Domicilio.Numero, out int numero))
                            tabla[7, i] = numero;
                        else
                            tabla[7, i] = usuario.Domicilio.Numero;

                        tabla[8, i] = usuario.Contratos.Select(r => r.Contrato.TipoContrato.Nombre).ToDelimitedString(", ");

                        tabla.Formato[9, i] = "dd/mm/yyyy";
                        tabla[9, i] = usuario.UltimoPago;

                        i++;
                    }
                }
                else
                {

                }


                libro.Generar();

                AdministradorNotificaciones.Lanzar(new ArchivoGenerado(nombre));
            }
            catch (Exception ex)
            {
                AdministradorNotificaciones.Lanzar(new NotificacionError { Descripcion = ex.Message, Titulo = "Error" });
                //TODO: Log
                Console.WriteLine(ex.Message);
            }
        });

        #region Inicializacion
        private IDictionary<Seccion, IList<Calle>> CallesAgrupadas;
        private IDictionary<ClaseContrato, IList<TipoContrato>> TiposContratoAgrupados;

        private Task Inicializar() => Task.Run(() =>
        {
            using (var baseDeDatos = Ambito.CreateReadOnly())
            {
                CallesAgrupadas = Domicilios.CallesAgrupadas(SeccionesRepo);
                TiposContratoAgrupados = Contratos.TiposContratoAgrupados(TiposContratoRepo);

                Secciones = CallesAgrupadas.Keys.OrderBy(_ => _.Orden).ToList();
                ClasesContrato = TiposContratoAgrupados.Keys.ToList();

                if (Secciones.FirstOrDefault() is Seccion seccion)
                {
                    Filtros.Seccion.Valor.Valor = seccion.Nombre;

                    ActualizarListadoDeCalles();
                }

                if (ClasesContrato.FirstOrDefault() is ClaseContrato claseContrato)
                {
                    Filtros.ClaseContrato.Valor.Valor = claseContrato.ToString();

                    ActualizarListadoDeTiposContrato();
                }

                Agrupadores = Dtos.Agrupadores.Todos;
            }
        });

        private void ActualizarListadoDeCalles()
        {
            if (Filtros.Seccion.Valor.Valor is string nombreSeccion && Secciones.SingleOrDefault(_ => _.Nombre == nombreSeccion) is Seccion seccion)
            {
                Calles = CallesAgrupadas[seccion];
                Filtros.Calle.Valor.Valor = Calles.FirstOrDefault()?.Nombre;
            }
        }

        private void ActualizarListadoDeTiposContrato()
        {
            if (Filtros.ClaseContrato.Valor.Valor is string nombreClaseContrato
                && ClasesContrato.SingleOrDefault(_ => _.ToString() == nombreClaseContrato) is ClaseContrato claseContrato)
            {
                TiposContrato = TiposContratoAgrupados[claseContrato];
                Filtros.TipoContrato.Valor.Valor = TiposContrato.FirstOrDefault()?.Nombre;
            }
        }
        #endregion

        private async Task Entrar(object arg)
        {
            if (arg != null)
            {
                if (arg is string texto)
                {
                    if (Solicitud.IntentarObtener(texto, out Solicitud solicitud))
                    {

                    }
                    else
                    {
                        DesactivarFiltros();
                        Ordenamiento = null;
                        Agrupador = null;
                        TextoBusqueda = texto;
                    }
                }

                await InvocarEnfocar().ConfigureAwait(true);
                await Task.Delay(TiempoEsperaBusqueda).ConfigureAwait(true);

                var _ = Buscar();
            }
            else
            {
                var _ = Buscar();
                await InvocarEnfocar().ConfigureAwait(true);
            }
        }

        private async Task InvocarEnfocar()
        {
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        #region Busqueda
        private static long IdBusqueda;
        private static readonly object token = new object();

        public enum ModoBusqueda { Intentar, Forzar, PostOperaciones }

        public Task Buscar(ModoBusqueda modoBusqueda = ModoBusqueda.Intentar) => Task.Run(() =>
        {
            lock (token)
            {
                // Seguridad extra
                if (Busqueda != null && !(Busqueda.Buscando ?? true) && modoBusqueda == ModoBusqueda.PostOperaciones)
                {
                    AplicarPostOperaciones(Busqueda, Busqueda.Originales);
                    return;
                }
            }

            SincronizarTextoBusquedaConFiltros();

            Solicitud solicitudActual = ObtenerSolicitudActual();
            var busquedaActual = new Busqueda { Buscando = true, Solicitud = solicitudActual.ToString() };

            if (modoBusqueda == ModoBusqueda.Intentar && Busqueda?.Solicitud == busquedaActual.Solicitud)
                return;

            var id = Interlocked.Increment(ref IdBusqueda);

            lock (token)
            {
                // Ya está por ejecutarse una búsqueda más reciente
                if (Interlocked.Read(ref IdBusqueda) > id)
                    return;

                Busqueda = busquedaActual;
            }

            Console.WriteLine(solicitudActual);

            try
            {
                var resultadosUsuarios = ObtenerUsuariosDesdeBaseDeDatos(solicitudActual);

                // Ya hay una busqueda mas reciente en ejecucion.
                if (Interlocked.Read(ref IdBusqueda) > id)
                    return;

                AplicarPostOperaciones(busquedaActual, resultadosUsuarios);

                busquedaActual.Buscando = false;
                busquedaActual.Conteo = resultadosUsuarios.Count;
                busquedaActual.AdeudoTotal = resultadosUsuarios.Select(u => u.Adeudo).Sum();
                busquedaActual.HayResultados = resultadosUsuarios.Count > 0;
            }
            catch (Exception ex)
            {
                busquedaActual.TieneErrores = true;
                busquedaActual.Error = ex.Message;
            }
        });

        private void SincronizarTextoBusquedaConFiltros()
        {
            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                Filtros.NombreCompleto.Valor.Valor = TextoBusqueda.Trim();
                Filtros.NombreCompleto.Activo = true;
            }
            else
            {
                Filtros.NombreCompleto.Valor.Valor = "";
                Filtros.NombreCompleto.Activo = false;
            }
        }

        private Solicitud ObtenerSolicitudActual() => new Solicitud
        {
            Agrupadores = new List<Propiedad> { Agrupador?.Propiedad },
            Columnas = Columnas.Todas.Where(_ => _.Activo).Select(_ => _.Nombre).Select(_ => (Propiedad)_).ToList(),
            Filtros = Filtros.Todos.Where(_ => _.Activo).Select(_ => _.Valor).Cast<Condicion>().ToList(),
            Ordenamientos = new List<Ordenamiento> { new Ordenamiento { Propiedad = Ordenamiento?.Propiedad, Direccion = Ordenamiento?.Direccion } }
        }.Coercer();

        private IList<ResultadoUsuario> ObtenerUsuariosDesdeBaseDeDatos(Solicitud solicitud)
        {
            using (var baseDeDatos = Ambito.CreateReadOnly())
            {
                var tarifas = Tarifas.Obtener(TarifasRepo);

                return new EjecutorSolicitud(solicitud, CalculadorAdeudos).Ejecutar(UsuariosRepo.Datos.AsQueryable());
            }
        }

        private void AplicarPostOperaciones(Busqueda busqueda, IList<ResultadoUsuario> resultadosUsuarios)
        {
            var (puntosNavegacion, resultados) = AgruparOrdenarResultados(resultadosUsuarios);
            busqueda.PuntosNavegacion = puntosNavegacion;
            busqueda.Resultados = resultados;
            busqueda.Originales = resultadosUsuarios;
        }

        private (IEnumerable<PuntoNavegacion> PuntosNavegacion, IEnumerable Resultados) AgruparOrdenarResultados(IEnumerable<ResultadoUsuario> resultadosUsuarios)
        {
            var puntosNavegacion = new List<PuntoNavegacion>();

            ResultadoUsuario ConvertirGrupo(Grupo g) =>
                new ResultadoUsuario { Titulo = g.ToString(), Subtitulo = $"{g.Valores.Select(_ => _.Adeudo).Sum():C}" };

            if (Agrupador != null)
            {
                var resultadosAgrupados = Agrupador.Agrupar(resultadosUsuarios);

                var indiceAcumulado = 0;
                foreach (var grupo in resultadosAgrupados)
                {
                    puntosNavegacion.Add(new PuntoNavegacion { Nombre = grupo.ToString(), Indice = indiceAcumulado });
                    indiceAcumulado += grupo.Valores.Count() + 1;
                }

                if (Ordenamiento?.Direccion != null)
                    return (puntosNavegacion, resultadosAgrupados.SelectMany(g => ConvertirGrupo(g).Concat(Ordenamiento.Ordenar(g.Valores).ToList())));
                else
                    return (puntosNavegacion, resultadosAgrupados.SelectMany(g => ConvertirGrupo(g).Concat(g.Valores)));
            }
            else
            {
                if (Ordenamiento?.Direccion != null)
                    return (puntosNavegacion, Ordenamiento.Ordenar(resultadosUsuarios).ToList());
                else
                    return (puntosNavegacion, resultadosUsuarios);
            }
        }
        #endregion
    }
}

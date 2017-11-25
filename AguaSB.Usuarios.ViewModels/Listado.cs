using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Nucleo.Datos;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using GGUtils.MVVM.Async;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using AguaSB.Utilerias.Solicitudes;
using System.Collections;
using Mehdime.Entity;
using System.Threading;

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
        #endregion

        #region Comandos
        public DelegateCommand DesactivarFiltrosComando { get; }
        public DelegateCommand MostrarColumnasTodasComando { get; }

        public DelegateCommand AgregarContratoComando { get; }
        public DelegateCommand EditarUsuarioComando { get; }
        #endregion

        #region Eventos
        public event EventHandler<IOrdenamiento> OrdenamientoCambiado;
        public event EventHandler Enfocar;
        #endregion

        #region Dependencias
        private IDbContextScopeFactory Ambito { get; }

        public IRepositorio<Usuario> UsuariosRepo { get; }
        public IRepositorio<Seccion> SeccionesRepo { get; }
        public IRepositorio<TipoContrato> TiposContratoRepo { get; }
        public IRepositorio<Tarifa> TarifasRepo { get; }

        private INavegador Navegador { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Tarifa> tarifasRepo, INavegador navegador)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            Nodo = new Nodo { PrimeraEntrada = Inicializar, Entrada = Entrar };

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            MostrarColumnasTodasComando = new DelegateCommand(MostrarColumnasTodas);

            AgregarContratoComando = new DelegateCommand(AgregarContrato);
            EditarUsuarioComando = new DelegateCommand(EditarUsuario);

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
                .Where(_ => _ == nameof(TextoBusqueda) && !string.IsNullOrEmpty(TextoBusqueda));

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

        private static long IdBusqueda;
        private static readonly object token = new object();

        public enum ModoBusqueda { Intentar, Forzar, PostOperaciones }

        public Task Buscar(ModoBusqueda modoBusqueda = ModoBusqueda.Intentar) => Task.Run(() =>
        {
            if (modoBusqueda == ModoBusqueda.PostOperaciones)
            {
                AplicarPostOperaciones(Busqueda, Busqueda.Originales);
                return;
            }

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                Filtros.NombreCompleto.Valor.Valor = TextoBusqueda;
                Filtros.NombreCompleto.Activo = true;
            }
            else
            {
                Filtros.NombreCompleto.Valor.Valor = "";
                Filtros.NombreCompleto.Activo = false;
            }

            Solicitud solicitud = ObtenerSolicitudActual();

            var busqueda = new Busqueda { Buscando = true, Solicitud = solicitud.ToString() };

            if (modoBusqueda == ModoBusqueda.Intentar && Busqueda?.Solicitud == busqueda.Solicitud)
                return;

            var id = Interlocked.Increment(ref IdBusqueda);

            lock (token)
            {
                Busqueda = busqueda;
            }

            Console.WriteLine(solicitud);

            try
            {
                var resultadosUsuarios = ObtenerUsuariosDesdeBaseDeDatos(solicitud);

                // Ya hay una busqueda mas reciente en ejecucion.
                if (Interlocked.Read(ref IdBusqueda) > id)
                    return;

                AplicarPostOperaciones(busqueda, resultadosUsuarios);

                busqueda.Buscando = false;
                busqueda.Conteo = resultadosUsuarios.Count;
                busqueda.HayResultados = resultadosUsuarios.Count > 0;
            }
            catch (Exception ex)
            {
                busqueda.TieneErrores = true;
                busqueda.Error = ex.Message;
            }
        });

        private void AplicarPostOperaciones(Busqueda busqueda, IList<ResultadoUsuario> resultadosUsuarios)
        {
            var (puntosNavegacion, resultados) = AgruparOrdenar(resultadosUsuarios);
            busqueda.PuntosNavegacion = puntosNavegacion;
            busqueda.Resultados = resultados;
            busqueda.Originales = resultadosUsuarios;
        }

        private Solicitud ObtenerSolicitudActual()
        {
            var resultado = new Solicitud
            {
                Agrupadores = new List<Propiedad> { Agrupador?.Propiedad },
                Columnas = Columnas.Todas.Where(_ => _.Activo).Select(_ => _.Nombre).Select(_ => (Propiedad)_).ToList(),
                Filtros = Filtros.Todos.Where(_ => _.Activo).Select(_ => _.Valor).Cast<Condicion>().ToList(),
                Ordenamientos = new List<Ordenamiento> { new Ordenamiento { Propiedad = Ordenamiento?.Propiedad, Direccion = Ordenamiento?.Direccion } }
            };

            resultado.Coercer();

            return resultado;
        }

        private IList<ResultadoUsuario> ObtenerUsuariosDesdeBaseDeDatos(Solicitud solicitud)
        {
            using (var baseDeDatos = Ambito.CreateReadOnly())
            {
                var tarifas = TarifasRepo.Datos.OrderBy(_ => _.FechaRegistro).ToArray();

                return EjecutorSolicitud.Ejecutar(
                    UsuariosRepo.Datos.AsQueryable(), solicitud,
                    (pagadoHasta, multiplicador) => Adeudos.Calcular(pagadoHasta, multiplicador, tarifas));
            }
        }

        private (IEnumerable<PuntoNavegacion>, IEnumerable) AgruparOrdenar(IEnumerable<ResultadoUsuario> resultadosUsuarios)
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
    }
}

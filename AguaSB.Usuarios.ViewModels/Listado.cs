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

        private EstadoBusqueda estado;
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

        public EstadoBusqueda Estado
        {
            get { return estado; }
            set { SetProperty(ref estado, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand DesactivarFiltrosComando { get; }
        public DelegateCommand MostrarColumnasTodasComando { get; }
        public AsyncDelegateCommand<ResultadoSolicitud> BuscarComando { get; }

        public DelegateCommand AgregarContratoComando { get; }
        public DelegateCommand EditarUsuarioComando { get; }
        #endregion

        #region Eventos
        public event EventHandler<IOrdenamiento> OrdenamientoCambiado;
        public event EventHandler Enfocar;
        #endregion

        #region Dependencias
        public IRepositorio<Usuario> UsuariosRepo { get; }
        public IRepositorio<Seccion> SeccionesRepo { get; }
        public IRepositorio<TipoContrato> TiposContratoRepo { get; }
        public IRepositorio<Tarifa> TarifasRepo { get; }

        private INavegador Navegador { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado(IRepositorio<Usuario> usuariosRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Tarifa> tarifasRepo, INavegador navegador)
        {
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            Nodo = new Nodo { PrimeraEntrada = Inicializar, Entrada = Entrar };

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            MostrarColumnasTodasComando = new DelegateCommand(MostrarColumnasTodas);
            BuscarComando = new AsyncDelegateCommand<ResultadoSolicitud>(Buscar, multipleExecutionSupported: true);

            AgregarContratoComando = new DelegateCommand(AgregarContrato);
            EditarUsuarioComando = new DelegateCommand(EditarUsuario);

            Columnas = new Columnas();

            Columnas.FechaRegistro.Activo = false;
            Columnas.UltimoPago.Activo = false;

            Filtros = new Filtros();

            Ordenamientos = new Ordenamientos();

            Estado = new EstadoBusqueda();

            RegistrarUniones();
        }

        private void RegistrarUniones()
        {
            var filtros = Filtros.ToObservableProperties().Select(_ => _.Args.PropertyName).Where(_ => _ != nameof(Filtros.NombreCompleto));

            filtros.Where(p => p == nameof(Filtros.Seccion))
                .ObserveOnDispatcher()
                .Subscribe(_ => ActualizarListadoDeCalles());

            filtros.Where(p => p == nameof(Filtros.ClaseContrato))
                .ObserveOnDispatcher()
                .Subscribe(_ => ActualizarListadoDeTiposContrato());

            var propiedades = this.ToObservableProperties()
                .Select(_ => _.Args.PropertyName)
                .Where(_ => _ == nameof(Agrupador) || _ == nameof(Ordenamiento));

            var textoBusqueda = this.ToObservableProperties()
                .Select(_ => _.Args.PropertyName)
                .Where(_ => _ == nameof(TextoBusqueda) && !string.IsNullOrEmpty(TextoBusqueda));

            new[] { filtros, propiedades, textoBusqueda }.Merge().Throttle(TiempoEsperaBusqueda)
                .Skip(1)
                .ObserveOnDispatcher()
                .Subscribe(_ => BuscarComando.Execute(null));
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

                if (BuscarComando.Execution == null)
                    BuscarComando.Execute(null);
            }
            else
            {
                BuscarComando.Execute(null);
                await InvocarEnfocar().ConfigureAwait(true);
            }
        }

        private async Task InvocarEnfocar()
        {
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private static readonly EjecutorSolicitud EjecutorSolicitud = new EjecutorSolicitud();

        private string SolicitudAnterior;
        private ResultadoSolicitud ResultadoAnterior;
        private bool forzar;

        public void EjecutarBusqueda()
        {
            forzar = true;
            BuscarComando.Execute(null);
        }

        private Task<ResultadoSolicitud> Buscar() => Task.Run(() =>
        {
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

            var solicitud = new Solicitud
            {
                Agrupadores = new List<Propiedad> { Agrupador?.Propiedad },
                Columnas = Columnas.Todas.Where(_ => _.Activo).Select(_ => _.Nombre).Select(_ => (Propiedad)_).ToList(),
                Filtros = Filtros.Todos.Where(_ => _.Activo).Select(_ => _.Valor).Cast<Condicion>().ToList(),
                Ordenamientos = new List<Ordenamiento> { new Ordenamiento { Propiedad = Ordenamiento?.Propiedad, Direccion = Ordenamiento?.Direccion } }
            };

            solicitud.Coercer();
            var cadenaSolicitud = solicitud.ToString();

            if (!forzar && cadenaSolicitud == SolicitudAnterior)
            {
                return ResultadoAnterior;
            }

            forzar = false;
            SolicitudAnterior = cadenaSolicitud;

            var estado = Estado = new EstadoBusqueda
            {
                Buscando = true
            };

            var resultadosUsuarios = EjecutorSolicitud.Ejecutar(
                UsuariosRepo.Datos.AsQueryable(), solicitud,
                (pagadoHasta, tipoContrato) => Adeudos.Calcular(pagadoHasta, tipoContrato, TarifasRepo.Datos.OrderBy(_ => _.FechaRegistro).ToArray()));

            estado.Buscando = false;
            estado.HayResultados = resultadosUsuarios.Count > 0;

            IEnumerable resultados;
            var puntosNavegacion = new List<PuntoNavegacion>();

            ResultadoUsuario Obtener(Grupo g) =>
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
                    resultados = resultadosAgrupados.SelectMany(g => Obtener(g).Concat(Ordenamiento.Ordenar(g.Valores).ToList()));
                else
                    resultados = resultadosAgrupados.SelectMany(g => Obtener(g).Concat(g.Valores));
            }
            else
            {
                if (Ordenamiento?.Direccion != null)
                    resultados = Ordenamiento.Ordenar(resultadosUsuarios).ToList();
                else
                    resultados = resultadosUsuarios;
            }

            ResultadoAnterior = new ResultadoSolicitud { Conteo = resultadosUsuarios.Count, Resultados = resultados, PuntosNavegacion = puntosNavegacion };

            return ResultadoAnterior;
        });
    }
}

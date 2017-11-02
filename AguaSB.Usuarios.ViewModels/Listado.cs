using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using System.Waf.Foundation;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Nucleo;
using System.Waf.Applications;
using GGUtils.MVVM.Async;
using MoreLinq;
using AguaSB.Utilerias;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Linq;
using AguaSB.Datos;
using AguaSB.Nucleo.Datos;

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Configuracion
        private const double TiempoEsperaBusqueda = 1.5;
        #endregion

        #region Campos
        private IEnumerable<Seccion> secciones;
        private IEnumerable<Calle> calles;
        private IEnumerable<ClaseContrato> clasesContrato;
        private IEnumerable<TipoContrato> tiposContrato;

        private IEnumerable<Agrupador> criteriosAgrupacion;
        private Solicitud solicitud;
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

        public IEnumerable<Agrupador> CriteriosAgrupacion
        {
            get { return criteriosAgrupacion; }
            set { SetProperty(ref criteriosAgrupacion, value); }
        }

        public Solicitud Solicitud
        {
            get { return solicitud; }
            set { SetProperty(ref solicitud, value); }
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
        #endregion

        #region Eventos
        public event EventHandler<Agrupador> AgrupadorCambiado;
        #endregion

        #region Servicios
        public IRepositorio<Usuario> UsuariosRepo { get; }
        public IRepositorio<Seccion> SeccionesRepo { get; }
        public IRepositorio<TipoContrato> TiposContratoRepo { get; }
        public IRepositorio<Tarifa> TarifasRepo { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado(IRepositorio<Usuario> usuariosRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Tarifa> tarifasRepo)
        {
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));

            Nodo = new Nodo { PrimeraEntrada = Inicializar };

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            MostrarColumnasTodasComando = new DelegateCommand(MostrarColumnasTodas);
            BuscarComando = new AsyncDelegateCommand<ResultadoSolicitud>(Buscar, multipleExecutionSupported: true);

            this.ToObservableProperties().Subscribe(_ => RegistrarUniones(_.Args));

            Solicitud = new Solicitud
            {
                Filtros = new Filtros(),
                Columnas = Columnas.Todas
            };
            Solicitud.Columnas.FechaRegistro = false;

            Estado = new EstadoBusqueda();

            Fill();
        }

        private IDictionary<Seccion, IList<Calle>> CallesAgrupadas;
        private IDictionary<ClaseContrato, IList<TipoContrato>> TiposContratoAgrupados;

        private async Task Inicializar()
        {
            var callesAgrupadasTarea = Task.Run(() => Domicilios.CallesAgrupadas(SeccionesRepo));
            var tiposContratoAgrupadosTarea = Task.Run(() => Contratos.TiposContratoAgrupados(TiposContratoRepo));

            CallesAgrupadas = await callesAgrupadasTarea.ConfigureAwait(continueOnCapturedContext: false);
            TiposContratoAgrupados = await tiposContratoAgrupadosTarea.ConfigureAwait(continueOnCapturedContext: false);

            Secciones = CallesAgrupadas.Keys.OrderBy(_ => _.Orden).ToList();
            ClasesContrato = TiposContratoAgrupados.Keys.ToList();

            if (Secciones.FirstOrDefault() is var seccion)
            {
                Solicitud.Filtros.Seccion.Valor = seccion;

                ActualizarListadoDeCalles();
            }

            if (ClasesContrato.FirstOrDefault() is var claseContrato)
            {
                Solicitud.Filtros.ClaseContrato.Valor = claseContrato;

                ActualizarListadoDeTiposContrato();
            }
        }

        private void ActualizarListadoDeCalles()
        {
            if (Solicitud.Filtros.Seccion.Valor is var seccion)
            {
                Calles = CallesAgrupadas[seccion];
                Solicitud.Filtros.Calle.Valor = Calles.FirstOrDefault();
            }
        }

        private void ActualizarListadoDeTiposContrato()
        {
            if (Solicitud.Filtros.ClaseContrato.Valor is ClaseContrato claseContrato)
            {
                TiposContrato = TiposContratoAgrupados[claseContrato];
                Solicitud.Filtros.TipoContrato.Valor = TiposContrato.FirstOrDefault();
            }
        }

        private IDisposable Propiedades;

        private void RegistrarUniones(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Solicitud))
            {
                Propiedades?.Dispose();

                var excepto = new HashSet<string>
                {
                    nameof(Solicitud.Agrupador), nameof(Solicitud.Columnas)
                };

                var props = new INotifyPropertyChanged[] { Solicitud, Solicitud.Filtros }
                .Concat(Solicitud.Filtros.Todos)
                .Select(_ => _.ToObservableProperties());

                Propiedades = props.Merge()
                    .Where(_ => !excepto.Contains(_.Args.PropertyName))
                    .Throttle(TimeSpan.FromSeconds(TiempoEsperaBusqueda))
                    .Skip(1)
                    .Subscribe(_ => BuscarComando.Execute(null));

                Solicitud.Filtros.Seccion.ToObservableProperties()
                    .Where(_ => _.Args.PropertyName == nameof(Solicitud.Filtros.Calle.Valor))
                    .Subscribe(_ => ActualizarListadoDeCalles());
            }
        }

        private void MostrarColumnasTodas() => Solicitud.Columnas = Columnas.Todas;

        private void DesactivarFiltros() => Solicitud.Filtros.Todos.ForEach(f => f.Activo = false);

        private async Task<ResultadoSolicitud> Buscar()
        {
            var estado = Estado = new EstadoBusqueda
            {
                Buscando = true
            };

            var resultados = await Task.Run(() =>
                Solicitud.Filtros.Aplicar(UsuariosRepo.Datos.AsQueryable(),
                (pagadoHasta, tipoContrato) => Adeudos.Calcular(pagadoHasta, tipoContrato, TarifasRepo.Datos.OrderBy(_ => _.FechaRegistro).ToArray())));

            var conteo = resultados.LongCount();

            estado.Buscando = false;
            estado.HayResultados = conteo > 0;

            return estado.HayResultados == true ? new ResultadoSolicitud { Resultados = resultados, Conteo = resultados.LongCount() } : null;
        }

        private async void Fill()
        {
            await Task.Delay(2000).ConfigureAwait(false);

            string ExtraerMes(object objeto) =>
                objeto is DateTime d
                ? Capitalizar(d.ToString("MMMM yyyy"))
                : "Desconocido";

            CriteriosAgrupacion = new[]
            {
                Agrupador.Ninguno,
                new Agrupador { Nombre = "Sección", Propiedad = "Domicilio.Calle.Seccion.Nombre" },
                new Agrupador { Nombre = "Calle", Propiedad = "Domicilio.Calle.Nombre" },
                new Agrupador { Nombre = "Adeudo", Descripcion = "En pesos", Propiedad = "Adeudo",
                    Conversor = x =>
                    {
                        if(x is decimal d)
                            return Clasificar(d);
                        else
                            return "Desconocido";
                    }
                },
                new Agrupador
                {
                    Nombre = "Pagado hasta",
                    Propiedad = "PagadoHasta",
                    Conversor = ExtraerMes
                },
                new Agrupador
                {
                    Nombre = "Fecha de registro",
                    Propiedad = "Usuario.FechaRegistro",
                    Conversor = ExtraerMes
                }
            };

            Solicitud.ToObservableProperties()
                .Where(_ => _.Args.PropertyName == nameof(Solicitud.Agrupador))
                .Subscribe(_ => AgrupadorCambiado?.Invoke(this, Solicitud.Agrupador));
        }

        private string Capitalizar(string s) => char.ToUpper(s[0]) + s.Substring(1);

        private string Clasificar(decimal d)
        {
            var residuo = d / 500;
            return $"{residuo * 500:C} - {(residuo + 1) * 500:C}";
        }
    }
}

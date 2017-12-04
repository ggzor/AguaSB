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
using System.Threading;
using Mehdime.Entity;
using AguaSB.Datos;
using System.Waf.Applications;
using System.Collections.Generic;
using MoreLinq;

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
        private ResultadosBusquedaUsuarios busqueda;
        private string textoBusqueda;
        private InformacionPago informacionPago =
            new InformacionPago(new Persona { Nombre = "Axel", ApellidoPaterno = "Suárez", ApellidoMaterno = "Pololo" },
                new[]
                {
                    new Contrato { Domicilio = new Domicilio { Numero = "34", Calle = new Calle { Nombre = "Dolores", Seccion = new Seccion { Nombre = "Primera" } } }, TipoContrato = new TipoContrato { Nombre ="Convencional", ClaseContrato = ClaseContrato.Doméstico } },
                    new Contrato { Domicilio = new Domicilio { Numero = "33", Calle = new Calle { Nombre = "Dominguez", Seccion = new Seccion { Nombre = "Segunda" } } }, TipoContrato = new TipoContrato { Nombre ="Tienda", ClaseContrato = ClaseContrato.Comercial } }
                }, GenerarListas());

        private static IEnumerable<(int ConteoInicio, int ConteoFin, IEnumerable<RangoPago> Rangos)> GenerarListas()
        {
            var inicio = new DateTime(2018, 01, 01);
            const decimal adeudoBase = 1200m;

            return from b in Enumerable.Range(0, 12).Batch(4)
                   let conteoInicio = b.First() + 1
                   let conteoFinal = b.Last() + 1
                   select (conteoInicio, conteoFinal,
                        from d in b
                        let mes = inicio.AddMonths(d)
                        select new RangoPago
                        {
                            AdeudoRestante = adeudoBase - (d * 60),
                            Hasta = mes,
                            Monto = d * 60
                        });
        }
        #endregion

        #region Propiedades
        public bool Buscando
        {
            get { return buscando; }
            set { SetProperty(ref buscando, value); }
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

        public InformacionPago InformacionPago
        {
            get { return informacionPago; }
            set { SetProperty(ref informacionPago, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand BuscarEnListadoComando { get; set; }
        #endregion

        #region Servicios
        private IDbContextScopeFactory Ambito { get; }
        private IRepositorio<Usuario> UsuariosRepo { get; }
        private INavegador Navegador { get; }
        #endregion

        public event EventHandler Enfocar;
        public INodo Nodo { get; }

        public Agregar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, INavegador navegador)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            Nodo = new Nodo { Entrada = Entrar };

            BuscarEnListadoComando = new DelegateCommand(() => Navegador.Navegar("Usuarios/Listado", TextoBusqueda));

            this.ToObservableProperties()
                .Where(p => p.Args.PropertyName == nameof(TextoBusqueda))
                .Throttle(TiempoEsperaBusqueda)
                .Where(_ => !string.IsNullOrWhiteSpace(TextoBusqueda))
                .Select(_ => TextoBusqueda.Trim())
                .DistinctUntilChanged()
                .ObserveOnDispatcher()
                .Subscribe(async texto => await ObtenerOpciones(texto).ConfigureAwait(true));
        }

        private async Task Entrar(object arg)
        {
            Busqueda = new ResultadosBusquedaUsuarios(CantidadOpciones);
            await Task.Delay(200).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private readonly object Token = new object();
        private int IdTareaActual;

        private async Task ObtenerOpciones(string nombreUsuario)
        {
            Task BuscarOpciones = Task.Run(() =>
            {
                var id = Interlocked.Increment(ref IdTareaActual);

                var busqueda = new ResultadosBusquedaUsuarios(CantidadOpciones);

                using (var baseDatos = Ambito.CreateReadOnly())
                    busqueda.Buscar(UsuariosRepo.Datos, nombreUsuario);

                if (IdTareaActual == id)
                {
                    lock (Token)
                    {
                        Busqueda = busqueda;
                    }
                }
            });

            Buscando = true;

            try
            {
                await Task.Delay(3000).ConfigureAwait(true);
                await BuscarOpciones.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                //TODO: Log
                Console.WriteLine("Excepcion: " + ex.Message);
            }

            if (Busqueda.TotalResultados == 1)
            {
                // Usuario unico
            }

            Buscando = false;
        }

        public void SeleccionarUsuario(Usuario usuario)
        {
            Console.WriteLine("Seleccionado: " + usuario.NombreCompleto);
        }
    }
}

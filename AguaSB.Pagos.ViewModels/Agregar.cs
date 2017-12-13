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
        private bool buscandoInformacionPago;
        private ResultadosBusquedaUsuarios busqueda;
        private string textoBusqueda;
        private InformacionPagoUsuario informacionPago =
            new InformacionPagoUsuario(new Persona { Nombre = "Axel", ApellidoPaterno = "Suárez", ApellidoMaterno = "Polo" }, GenerarContratos());
        private bool usuarioSeleccionado;

        private static readonly Random r = new Random();

        private static IEnumerable<InformacionPagoContrato> GenerarContratos()
        {
            var contratos = new[]
            {
                new Contrato { Domicilio = new Domicilio { Numero = "34", Calle = new Calle { Nombre = "Dolores", Seccion = new Seccion { Nombre = "Primera" } } }, TipoContrato = new TipoContrato { Nombre ="Convencional", ClaseContrato = ClaseContrato.Doméstico } },
                //new Contrato { Domicilio = new Domicilio { Numero = "33", Calle = new Calle { Nombre = "Dominguez", Seccion = new Seccion { Nombre = "Segunda" } } }, TipoContrato = new TipoContrato { Nombre ="Tienda", ClaseContrato = ClaseContrato.Comercial } }
            };

            int adeudo = r.Next(400, 1000);
            return contratos.Select(c => new InformacionPagoContrato(c, adeudo, GenerarListas(adeudo)));
        }

        private static IEnumerable<ColumnaRangosPago> GenerarListas(decimal adeudoBase)
        {
            var inicio = new DateTime(2018, 01, 01);

            return from b in Enumerable.Range(0, 12).Batch(4)
                   let conteoInicio = b.First() + 1
                   let conteoFinal = b.Last() + 1
                   select new ColumnaRangosPago(conteoInicio, conteoFinal,
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

        public InformacionPagoUsuario InformacionPago
        {
            get { return informacionPago; }
            set { SetProperty(ref informacionPago, value); }
        }

        public bool UsuarioSeleccionado
        {
            get { return usuarioSeleccionado; }
            set { SetProperty(ref usuarioSeleccionado, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand BuscarEnListadoComando { get; set; }
        #endregion

        #region Servicios
        private IDbContextScopeFactory Ambito { get; }
        private IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<Contrato> ContratosRepo { get; }
        private IRepositorio<Tarifa> TarifasRepo { get; set; }
        private INavegador Navegador { get; }
        #endregion

        public event EventHandler IniciandoBusqueda;
        public event EventHandler Enfocar;
        public event EventHandler EncontradoUsuarioUnico;
        public event EventHandler UsuarioCambiado;

        public INodo Nodo { get; }

        public Agregar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo,
            IRepositorio<Tarifa> tarifasRepo, INavegador navegador)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            ContratosRepo = contratosRepo ?? throw new ArgumentNullException(nameof(contratosRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            Nodo = new Nodo { Entrada = Entrar };

            BuscarEnListadoComando = new DelegateCommand(() => Navegador.Navegar("Usuarios/Listado", TextoBusqueda));

            ActivarEscuchaCambioTexto();
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
            IniciandoBusqueda?.Invoke(this, EventArgs.Empty);

            try
            {
                await BuscarOpciones.ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                //TODO: Log
                Console.WriteLine("Excepcion: " + ex.Message);
            }

            if (Busqueda.TotalResultados == 1)
            {
                EncontradoUsuarioUnico?.Invoke(this, EventArgs.Empty);
                await SeleccionarUsuario(Busqueda.Resultados.Single()).ConfigureAwait(false);
            }

            Buscando = false;
        }

        private static readonly Sincronizador ObtencionInformacionPagos = new Sincronizador();

        public async Task SeleccionarUsuario(Usuario usuario)
        {
            Task<InformacionPagoUsuario> ObtenerInformacion() => Task.Run(() =>
            {
                using (var baseDeDatos = Ambito.CreateReadOnly())
                {
                    var tarifas = TarifasRepo.Datos.OrderBy(t => t.FechaRegistro).ToArray();

                    var contratos = from Contrato in ContratosRepo.Datos
                                    where Contrato.Usuario.Id == usuario.Id
                                    let Tipo = Contrato.TipoContrato
                                    let ComponentesContrato = new { Contrato, Tipo }
                                    let Domicilio = Contrato.Domicilio
                                    let ComponentesDomicilio = new { Domicilio, Domicilio.Calle, Domicilio.Calle.Seccion }
                                    let Pagos = Contrato.Pagos
                                    select new { ComponentesContrato, ComponentesDomicilio, Pagos };

                    var contratosMaterializados = contratos.ToArray().Select(datos =>
                    {
                        var contrato = datos.ComponentesContrato.Contrato;
                        contrato.TipoContrato = datos.ComponentesContrato.Tipo;

                        return GenerarInformacionPagoContrato(contrato, tarifas);
                    }).ToArray();

                    return new InformacionPagoUsuario(usuario, contratosMaterializados);
                }
            });

            var id = ObtencionInformacionPagos.ObtenerId();

            UsuarioSeleccionado = false;
            BuscandoInformacionPago = true;

            var informacion = await ObtenerInformacion().ConfigureAwait(true);

            ObtencionInformacionPagos.Intentar(id, () => InformacionPago = informacion);

            BuscandoInformacionPago = false;
            UsuarioSeleccionado = true;

            // Invocar usuario cambiado
            await Task.Delay(200).ConfigureAwait(true);
            UsuarioCambiado?.Invoke(this, EventArgs.Empty);
        }

        private InformacionPagoContrato GenerarInformacionPagoContrato(Contrato contrato, Tarifa[] tarifas)
        {
            var pagos = contrato.Pagos.OrderBy(p => p.FechaRegistro).ToArray();
            var ultimoMesPagado = Fecha.MesDe(pagos.Last().Hasta);
            var adeudo = Adeudos.Calcular(ultimoMesPagado, contrato.TipoContrato.Multiplicador, tarifas);

            IEnumerable<RangoPago> GenerarRangosPago()
            {
                var tarifaActual = tarifas.Last();

                return Enumerable.Range(1, 12).Select(i =>
                {
                    var hasta = ultimoMesPagado.AddMonths(i);
                    var monto = tarifaActual.Monto * i;
                    var restante = Math.Max(adeudo - monto, 0);

                    return new RangoPago { Hasta = hasta, Monto = monto, AdeudoRestante = restante };
                });
            }

            var columnas = from b in GenerarRangosPago().ToArray().Batch(4).Index()
                           let indice = b.Key
                           let conteoInicio = (indice * 4) + 1
                           let conteoFin = (indice + 1) * 4
                           let rangos = b.Value
                           select new ColumnaRangosPago(conteoInicio, conteoFin, rangos.ToArray());

            return new InformacionPagoContrato(contrato, adeudo, columnas.ToArray());
        }

    }
}

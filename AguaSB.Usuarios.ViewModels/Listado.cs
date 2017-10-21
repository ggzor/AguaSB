using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AguaSB.Navegacion;
using System.Waf.Foundation;
using System.Collections;
using AguaSB.Usuarios.ViewModels.Dtos;
using AguaSB.Nucleo;
using System.Waf.Applications;
using GGUtils.MVVM.Async;
using MoreLinq;

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Campos
        private string textoBusqueda;

        private IEnumerable<string> criteriosOrdenamiento;
        private string criterioOrdenamiento;

        private bool agrupar;
        private IEnumerable<Agrupador> criteriosAgrupacion;

        private IEnumerable<Seccion> secciones;
        private IEnumerable<Calle> calles;

        private FiltroDomicilio filtroDomicilio = new FiltroDomicilio();
        private FiltroRango<decimal?> filtroAdeudo = new FiltroRango<decimal?>();
        private FiltroRango<DateTime> filtroFechaRegistro = new FiltroRango<DateTime>
        {
            Desde = new DateTime(2018, 01, 01),
            Hasta = DateTime.Today.AddDays(1).AddMinutes(-1)
        };

        private IEnumerable<ClaseContrato> clasesContrato;
        private IEnumerable<TipoContrato> tiposContrato;
        private IEnumerable<TipoContrato> tiposContratoSeleccionables;

        private FiltroTipoContrato filtroTipoContrato = new FiltroTipoContrato();
        private FiltroRango<DateTime> filtroFechaUltimoPago = new FiltroRango<DateTime>
        {
            Desde = DateTime.Today,
            Hasta = DateTime.Today.AddDays(1).AddMinutes(-1)
        };

        private bool? hayResultados;
        private bool? buscando;
        private bool? error;

        private IEnumerable<ResultadoUsuario> resultados;
        #endregion

        #region Propiedades
        public string TextoBusqueda
        {
            get { return textoBusqueda; }
            set { SetProperty(ref textoBusqueda, value); }
        }

        public IEnumerable<string> CriteriosOrdenamiento
        {
            get { return criteriosOrdenamiento; }
            set { SetProperty(ref criteriosOrdenamiento, value); }
        }

        public string CriterioOrdenamiento
        {
            get { return criterioOrdenamiento; }
            set { SetPropertyAndValidate(ref criterioOrdenamiento, value); }
        }

        public bool Agrupar
        {
            get { return agrupar; }
            set { SetProperty(ref agrupar, value); }
        }

        public IEnumerable<Agrupador> CriteriosAgrupacion
        {
            get { return criteriosAgrupacion; }
            set { SetProperty(ref criteriosAgrupacion, value); }
        }

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

        public FiltroDomicilio FiltroDomicilio
        {
            get { return filtroDomicilio; }
            set { SetProperty(ref filtroDomicilio, value); }
        }

        public FiltroRango<decimal?> FiltroAdeudo
        {
            get { return filtroAdeudo; }
            set { SetProperty(ref filtroAdeudo, value); }
        }

        public FiltroRango<DateTime> FiltroFechaRegistro
        {
            get { return filtroFechaRegistro; }
            set { SetProperty(ref filtroFechaRegistro, value); }
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

        public IEnumerable<TipoContrato> TiposContratoSeleccionables
        {
            get { return tiposContratoSeleccionables; }
            set { SetProperty(ref tiposContratoSeleccionables, value); }
        }

        public FiltroTipoContrato FiltroTipoContrato
        {
            get { return filtroTipoContrato; }
            set { SetProperty(ref filtroTipoContrato, value); }
        }

        public FiltroRango<DateTime> FiltroFechaUltimoPago
        {
            get { return filtroFechaUltimoPago; }
            set { SetProperty(ref filtroFechaUltimoPago, value); }
        }

        public bool? HayResultados
        {
            get { return hayResultados; }
            set { SetProperty(ref hayResultados, value); RaisePropertyChanged(nameof(NoHayResultados)); }
        }

        public bool? NoHayResultados => HayResultados == false;

        public bool? Buscando
        {
            get { return buscando; }
            set { SetProperty(ref buscando, value); }
        }

        public bool? Error
        {
            get { return error; }
            set { SetProperty(ref error, value); }
        }

        public long? CantidadResultados => Resultados?.LongCount();

        public IEnumerable<ResultadoUsuario> Resultados
        {
            get { return resultados; }
            set
            {
                SetProperty(ref resultados, value);
                RaisePropertyChanged(nameof(CantidadResultados));
            }
        }
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; }
        public AsyncDelegateCommand<int> BuscarComando { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado()
        {
            Nodo = new Nodo();

            ReestablecerComando = new DelegateCommand(Reestablecer);
            BuscarComando = new AsyncDelegateCommand<int>(Buscar);

            Fill();
        }

        private void Reestablecer()
        {
            Console.WriteLine("Reest");
        }

        private async Task<int> Buscar()
        {
            Resultados = null;
            HayResultados = null;
            Buscando = true;
            await Task.Delay(10).ConfigureAwait(false);

            var telefono = new TipoContacto()
            {
                Nombre = "Teléfono",
                ExpresionRegular = "."
            };

            var seccion = new Seccion
            {
                Nombre = "Primera",
                Orden = 0
            };

            var calle = new Calle
            {
                Seccion = seccion,
                Nombre = "Tlaxcala"
            };

            var tipoContrato = new TipoContrato
            {
                Nombre = "Convencional",
                ClaseContrato = ClaseContrato.Doméstico,
                Multiplicador = 0.5m
            };

            var tipoContrato2 = new TipoContrato
            {
                Nombre = "Comercial con nombre muy largo",
                ClaseContrato = ClaseContrato.Comercial,
                Multiplicador = 0.5m
            };

            Resultados = await Task.Run(() => new ResultadoUsuario[]
            {
                new ResultadoUsuario
                {
                    Usuario = new Persona
                    {
                        Id = 0,
                        Nombre = "Axel",
                        ApellidoPaterno = "Suárez",
                        ApellidoMaterno = "Polo",
                        FechaRegistro = DateTime.Today.AddMonths(-3),
                        Contactos = new List<Contacto>
                        {
                            new Contacto
                            {
                                TipoContacto = telefono,
                                Informacion = "241 245 32 12"
                            }
                        }
                    },
                    Adeudo = 0m,
                    UltimoPago = DateTime.Now,
                    Contratos = new List<ResultadoContrato>
                    {
                        new ResultadoContrato
                        {
                            Contrato = new Contrato
                            {
                                Domicilio = new Domicilio
                                {
                                    Numero = "19",
                                    Calle = calle
                                },
                                AdeudoInicial = 400,
                                MedidaToma = "1/2",
                                TipoContrato = tipoContrato
                            },
                            Adeudo = 400m
                        }
                    }
                },
                new ResultadoUsuario
                {
                    Usuario = new Negocio
                    {
                        Id = 1,
                        Nombre = "AguaSB",
                        FechaRegistro = DateTime.Today,
                        Contactos = new List<Contacto>
                        {
                            new Contacto
                            {
                                TipoContacto = telefono,
                                Informacion = "241 245 32 12"
                            }
                        }
                    },
                    Adeudo = 1000m,
                    Contratos = new List<ResultadoContrato>
                    {
                        new ResultadoContrato
                        {
                            Contrato = new Contrato
                            {
                                Domicilio = new Domicilio
                                {
                                    Numero = "20",
                                    Calle = calle
                                },
                                AdeudoInicial = 400,
                                MedidaToma = "1/2",
                                TipoContrato = tipoContrato2
                            },
                            Adeudo = 400m
                        },
                        new ResultadoContrato
                        {
                            Contrato = new Contrato
                            {
                                Domicilio = new Domicilio
                                {
                                    Numero = "1",
                                    Calle = calle
                                },
                                AdeudoInicial = 400,
                                MedidaToma = "1/2",
                                TipoContrato = tipoContrato
                            },
                            Adeudo = 400m
                        }
                    }
                },
            }.Repeat(1));

            Buscando = false;
            HayResultados = true;
            return 0;
        }

        private async void Fill()
        {
            await Task.Delay(2000).ConfigureAwait(false);

            CriteriosOrdenamiento = new[] { "Mayor adeudo", "Menor adeudo", "Pago reciente", "Registro reciente", "Calle" };
            CriteriosAgrupacion = new[]
            {
                new Agrupador { Nombre = "Sección" },
                new Agrupador { Nombre = "Calle" },
                new Agrupador { Nombre = "Deudor", Descripcion = "¿Es deudor o no?" },
                new Agrupador { Nombre = "Adeudo", Descripcion = "En pesos" },
                new Agrupador { Nombre = "Fecha de registro" }
            };
        }
    }
}

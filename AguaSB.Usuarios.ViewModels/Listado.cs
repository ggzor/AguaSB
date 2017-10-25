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

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Campos
        private IEnumerable<Filtro<Seccion>> secciones;
        private IEnumerable<Filtro<Calle>> calles;
        private IEnumerable<Filtro<ClaseContrato>> clasesContrato;
        private IEnumerable<Filtro<TipoContrato>> tiposContrato;

        private IEnumerable<Agrupador> criteriosAgrupacion;
        private Solicitud solicitud;
        private bool? buscando;
        private bool? hayResultados;
        private IEnumerable<ResultadoUsuario> resultados;
        #endregion

        #region Propiedades
        public IEnumerable<Filtro<Seccion>> Secciones
        {
            get { return secciones; }
            set { SetProperty(ref secciones, value); }
        }

        public IEnumerable<Filtro<Calle>> Calles
        {
            get { return calles; }
            set { SetProperty(ref calles, value); }
        }

        public IEnumerable<Filtro<ClaseContrato>> ClasesContrato
        {
            get { return clasesContrato; }
            set { SetProperty(ref clasesContrato, value); }
        }

        public IEnumerable<Filtro<TipoContrato>> TiposContrato
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

        public bool? Buscando
        {
            get { return buscando; }
            set { SetProperty(ref buscando, value); }
        }

        public bool? HayResultados
        {
            get { return hayResultados; }
            set { SetProperty(ref hayResultados, value); }
        }

        public IEnumerable<ResultadoUsuario> Resultados
        {
            get { return resultados; }
            set { SetProperty(ref resultados, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand DesactivarFiltrosComando { get; }
        public DelegateCommand ReestablecerComando { get; }
        public AsyncDelegateCommand<int> BuscarComando { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado()
        {
            Nodo = new Nodo();

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            ReestablecerComando = new DelegateCommand(Reestablecer);
            BuscarComando = new AsyncDelegateCommand<int>(Buscar, multipleExecutionSupported: true);

            Solicitud = new Solicitud
            {
                Filtros = new Filtros()
            };

            Fill();

            var props = Solicitud.ToObservableProperties();
            (from prop in props
             where prop.Args.PropertyName == nameof(Solicitud.Texto)
             select Solicitud.Texto)
             .Throttle(TimeSpan.FromSeconds(1))
             .Subscribe(_ => BuscarComando.Execute(null));
        }

        private void DesactivarFiltros() => Solicitud.Filtros.Todos.ForEach(f => f.Activo = false);

        private void Reestablecer()
        {
            Console.WriteLine("Reest");
        }

        private async Task<int> Buscar()
        {
            Resultados = null;
            HayResultados = null;
            Buscando = true;
            await Task.Delay(2000).ConfigureAwait(false);

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
            }.Repeat(1)).ConfigureAwait(false);

            Buscando = false;
            HayResultados = true;
            return 0;
        }

        private async void Fill()
        {
            await Task.Delay(2000).ConfigureAwait(false);

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

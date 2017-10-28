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

namespace AguaSB.Usuarios.ViewModels
{
    public class Listado : ValidatableModel, IViewModel
    {
        #region Configuracion
        private const double TiempoEsperaBusqueda = 1.5;
        #endregion

        #region Campos
        private IEnumerable<Filtro<Seccion>> secciones;
        private IEnumerable<Filtro<Calle>> calles;
        private IEnumerable<Filtro<ClaseContrato>> clasesContrato;
        private IEnumerable<Filtro<TipoContrato>> tiposContrato;

        private IEnumerable<Agrupador> criteriosAgrupacion;
        private Solicitud solicitud;
        private EstadoBusqueda estado;
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

        public EstadoBusqueda Estado
        {
            get { return estado; }
            set { SetProperty(ref estado, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand DesactivarFiltrosComando { get; }
        public DelegateCommand MostrarColumnasTodasComando { get; }
        public AsyncDelegateCommand<IEnumerable<ResultadoUsuario>> BuscarComando { get; }
        #endregion

        public INodo Nodo { get; }

        public Listado()
        {
            Nodo = new Nodo();

            DesactivarFiltrosComando = new DelegateCommand(DesactivarFiltros);
            MostrarColumnasTodasComando = new DelegateCommand(MostrarColumnasTodas);
            BuscarComando = new AsyncDelegateCommand<IEnumerable<ResultadoUsuario>>(Buscar, multipleExecutionSupported: true);

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

        private IDisposable Propiedades;

        private void RegistrarUniones(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Solicitud))
            {
                Propiedades?.Dispose();

                var props = new[] {
                    Solicitud.ToObservableProperties().Where(p => p.Args.PropertyName != nameof(Columnas) && p.Args.PropertyName != nameof(Solicitud.Texto)),
                    Solicitud.ToObservableProperties().Where(_ => _.Args.PropertyName == nameof(Solicitud.Texto)).Throttle(TimeSpan.FromSeconds(TiempoEsperaBusqueda)),
                    Solicitud.Filtros.ToObservableProperties()
                }.Concat(Solicitud.Filtros.Todos.Select(_ => _.ToObservableProperties()));

                Propiedades = props.Merge().Subscribe(_ => BuscarComando.Execute(null));
            }
        }

        private void MostrarColumnasTodas() => Solicitud.Columnas = Columnas.Todas;

        private void DesactivarFiltros() => Solicitud.Filtros.Todos.ForEach(f => f.Activo = false);

        private async Task<IEnumerable<ResultadoUsuario>> Buscar()
        {
            var estado = Estado = new EstadoBusqueda
            {
                Buscando = true
            };

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

            var resultados = await Task.Run(() => new ResultadoUsuario[]
            {
                new ResultadoUsuario
                {
                    Usuario = new Persona
                    {
                        Id = 10000,
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
                    Domicilio = new Domicilio
                    {
                        Numero = "19",
                        Calle = calle
                    },
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
                    Domicilio = new Domicilio
                    {
                        Numero = "19",
                        Calle = calle
                    },
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

            estado.Buscando = false;
            estado.HayResultados = true;

            return resultados;
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

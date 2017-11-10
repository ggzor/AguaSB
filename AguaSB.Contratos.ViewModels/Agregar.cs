using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;
using System.Windows.Input;

using GGUtils.MVVM.Async;
using MoreLinq;

using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.ViewModels;
using AguaSB.Notificaciones;
using AguaSB.Nucleo.Datos;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : ValidatableModel, IViewModel
    {
        #region Campos
        private bool mostrarProgreso;
        private string textoProgreso;

        private bool mostrarMensajeError = true;
        private bool puedeReestablecer = true;

        private Contrato contrato;

        private DateTime pagadoHasta;

        private IEnumerable<TipoContrato> tiposContrato;
        private IDictionary<Seccion, IList<Calle>> callesAgrupadas;
        #endregion

        #region Propiedades
        public bool MostrarProgreso
        {
            get { return mostrarProgreso; }
            set { SetProperty(ref mostrarProgreso, value); }
        }

        public string TextoProgreso
        {
            get { return textoProgreso; }
            set { SetProperty(ref textoProgreso, value); }
        }

        public bool MostrarMensajeError
        {
            get { return mostrarMensajeError; }
            set { SetProperty(ref mostrarMensajeError, value); }
        }

        public bool PuedeReestablecer
        {
            get { return puedeReestablecer; }
            set
            {
                SetProperty(ref puedeReestablecer, value);
                ReestablecerComando.RaiseCanExecuteChanged();
            }
        }

        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }

        public DateTime PagadoHasta
        {
            get { return pagadoHasta; }
            set { SetProperty(ref pagadoHasta, value); }
        }

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return tiposContrato; }
            set { SetProperty(ref tiposContrato, value); }
        }

        public IEnumerable<string> SugerenciasMedidasToma { get; } = new[] { "1/2", "1", "1 1/2", "2" };

        public IDictionary<Seccion, IList<Calle>> CallesAgrupadas
        {
            get { return callesAgrupadas; }
            set { SetProperty(ref callesAgrupadas, value); }
        }
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; }

        public AsyncDelegateCommand<int> AgregarContratoComando { get; }

        public DelegateCommand NavegarA { get; }
        #endregion

        #region Dependencias
        private IRepositorio<Usuario> UsuariosRepo { get; }
        private IRepositorio<Contrato> ContratosRepo { get; }
        private IRepositorio<TipoContrato> TiposContratoRepo { get; }
        private IRepositorio<Seccion> SeccionesRepo { get; }
        private IRepositorio<Ajustador> AjustadoresRepo { get; }
        private IRepositorio<Tarifa> TarifasRepo { get; }

        private IAdministradorNotificaciones Notificaciones { get; }
        private INavegador Navegador { get; }
        #endregion

        public event EventHandler Enfocar;

        public INodo Nodo { get; }

        public Agregar(
            IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Seccion> seccionesRepo, IRepositorio<Ajustador> ajustadoresRepo, IRepositorio<Tarifa> tarifasRepo,
            IAdministradorNotificaciones notificaciones, INavegador navegador)
        {
            UsuariosRepo = usuariosRepo ?? throw new ArgumentNullException(nameof(usuariosRepo));
            ContratosRepo = contratosRepo ?? throw new ArgumentNullException(nameof(contratosRepo));
            TiposContratoRepo = tiposContratoRepo ?? throw new ArgumentNullException(nameof(tiposContratoRepo));
            SeccionesRepo = seccionesRepo ?? throw new ArgumentNullException(nameof(seccionesRepo));
            AjustadoresRepo = ajustadoresRepo ?? throw new ArgumentNullException(nameof(ajustadoresRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));

            Notificaciones = notificaciones ?? throw new ArgumentNullException(nameof(notificaciones));
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));

            AgregarContratoComando = new AsyncDelegateCommand<int>(AgregarContrato, PuedeAgregarContrato);
            ReestablecerComando = new DelegateCommand(Reestablecer, () => PuedeReestablecer);
            NavegarA = new DelegateCommand(o => Navegador.Navegar((string)o, null));

            Nodo = new Nodo() { PrimeraEntrada = Inicializar, Entrada = Entrar };

            Reestablecer();

            new VerificadorPropiedades(this,
                () => new INotifyDataErrorInfo[]
                {
                    this, Contrato, Contrato.Domicilio
                },
                Enumerable.Empty<INotifyPropertyChanged>,
                () => new ICommand[]
                {
                    AgregarContratoComando, ReestablecerComando
                });
        }

        private Task Inicializar() => Task.Run(() =>
        {
            TextoProgreso = "Cargando información de secciones y calles...";
            MostrarProgreso = true;

            CallesAgrupadas = Domicilios.CallesAgrupadas(SeccionesRepo);

            TiposContrato = (from tipo in TiposContratoRepo.Datos
                             orderby tipo.Nombre
                             select tipo).ToList();

            MostrarProgreso = false;
        });

        private async Task Entrar(object arg)
        {
            if (arg is int id)
            {
                MostrarProgreso = true;
                TextoProgreso = "Cargando datos de usuario...";

                var buscarUsuario = Task.Run(() => UsuariosRepo.Datos.SingleOrDefault(u => u.Id == id));

                if (await buscarUsuario is Usuario usuario)
                {
                    Contrato.Usuario = usuario;
                }
                else
                {
                    Notificaciones.Lanzar(new NotificacionError()
                    {
                        Titulo = "Error",
                        Clase = "Base de datos",
                        Descripcion = $"No se encontró al usuario con Id = {id}."
                    });
                }

                MostrarProgreso = false;
            }

            await Task.Delay(100).ConfigureAwait(true);
            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private void Reestablecer()
        {
            MostrarMensajeError = false;
            Contrato = new Contrato()
            {
                Domicilio = new Domicilio()
            };

            PagadoHasta = Fecha.MesDe(Fecha.Ahora);
        }

        private bool PuedeAgregarContrato() =>
            UtileriasErrores.NingunoTieneErrores(this, Contrato, Contrato.Domicilio)
            && !Contrato.TieneCamposRequeridosVacios
            && !Contrato.Domicilio.TieneCamposRequeridosVacios
            && Contrato.Usuario != null;

        private async Task<int> AgregarContrato(IProgress<(double, string)> progreso)
        {
            MostrarMensajeError = true;
            PuedeReestablecer = false;

            try
            {
                var resultado = await EjecutarAgregarContrato(progreso).ConfigureAwait(true);

                var _ = Navegador.Navegar("Usuarios/Listado", Contrato.Usuario.NombreCompleto);

                PuedeReestablecer = true;
                Reestablecer();

                return resultado.Id;
            }
            finally
            {
                PuedeReestablecer = true;
            }
        }

        private Task<Contrato> EjecutarAgregarContrato(IProgress<(double, string)> progreso) => Task.Run(() =>
        {
            progreso.Report((20.0, "Registrando pago inicial..."));

            var ajustadorRegistro = AjustadoresRepo.Datos.FirstOrDefault(a => a.Nombre == "Registro");

            if (ajustadorRegistro == null)
                throw new Exception("No se ha establecido el ajustador para el registro inicial. Registre en la sección \"Editar ajustadores\" un ajustador con el nombre \"Registro\"");

            var pago = new Pago
            {
                Ajustador = ajustadorRegistro,
                Contrato = Contrato,
                Desde = Fecha.MesDe(PagadoHasta),
                Hasta = Fecha.MesDe(PagadoHasta),
                FechaRegistro = Fecha.Ahora
            };

            pago.Coercer();

            Contrato.Pagos.Add(pago);

            progreso.Report((50.0, "Agregando contrato..."));

            var resultado = ContratosRepo.Agregar(Contrato).Result;
            // TODO: Probablemente remover con EF
            Contrato.Usuario.Contratos.Add(Contrato);

            progreso.Report((100.0, "Completado."));
            return resultado;
        });
    }
}

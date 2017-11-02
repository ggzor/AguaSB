using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        private TipoContrato tipoContrato;
        private Seccion seccion;
        private Calle calle;

        private IEnumerable<TipoContrato> tiposContrato = Enumerable.Empty<TipoContrato>();
        private IEnumerable<Seccion> secciones = Enumerable.Empty<Seccion>();
        private IEnumerable<Calle> calles = Enumerable.Empty<Calle>();
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
                puedeReestablecer = value;
                ReestablecerComando.RaiseCanExecuteChanged();
            }
        }

        public Contrato Contrato
        {
            get { return contrato; }
            set { SetProperty(ref contrato, value); }
        }

        [Required(ErrorMessage = "Debe seleccionar un tipo de contrato existente.")]
        public TipoContrato TipoContrato
        {
            get { return tipoContrato; }
            set { SetPropertyAndValidate(ref tipoContrato, value); }
        }

        [Required(ErrorMessage = "Debe seleccionar una sección existente.")]
        public Seccion Seccion
        {
            get { return seccion; }
            set
            {
                SetPropertyAndValidate(ref seccion, value);
                if (value != null)
                {
                    Calles = callesAgrupadas[value];
                    Calle = Calles.FirstOrDefault();
                }
                else
                {
                    Calle = null;
                    Calles = null;
                }
            }
        }

        [Required(ErrorMessage = "Debe seleccionar una calle registrada.")]
        public Calle Calle
        {
            get { return calle; }
            set { SetPropertyAndValidate(ref calle, value); }
        }

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return tiposContrato; }
            set { SetProperty(ref tiposContrato, value); }
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

        public IEnumerable<string> SugerenciasMedidasToma { get; } = new[] { "1/2", "1", "1 1/2", "2" };
        #endregion

        #region Comandos
        public DelegateCommand ReestablecerComando { get; }

        public AsyncDelegateCommand<int> AgregarContratoComando { get; }

        public DelegateCommand NavegarA { get; }
        #endregion

        #region Dependencias
        private IRepositorio<Usuario> Usuarios { get; }
        private IRepositorio<Contrato> Contratos { get; }
        private IRepositorio<TipoContrato> TiposContratoRepo { get; }
        private IRepositorio<Seccion> SeccionesRepo { get; }

        private IAdministradorNotificaciones Notificaciones { get; }
        private INavegador Navegador { get; }
        #endregion

        public event EventHandler Enfocar;

        public INodo Nodo { get; }

        public Agregar(
            IRepositorio<Usuario> usuarios, IRepositorio<Contrato> contratos, IRepositorio<TipoContrato> tiposContrato,
            IRepositorio<Seccion> secciones, IAdministradorNotificaciones notificaciones, INavegador navegador)
        {
            Usuarios = usuarios ?? throw new ArgumentNullException(nameof(usuarios));
            Contratos = contratos ?? throw new ArgumentNullException(nameof(contratos));
            TiposContratoRepo = tiposContrato ?? throw new ArgumentNullException(nameof(tiposContrato));
            SeccionesRepo = secciones ?? throw new ArgumentNullException(nameof(secciones));
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
                () => Enumerable.Empty<INotifyPropertyChanged>(),
                () => new ICommand[]
                {
                    AgregarContratoComando, ReestablecerComando
                });
        }

        private IDictionary<Seccion, IList<Calle>> callesAgrupadas;

        private async Task Inicializar()
        {
            TextoProgreso = "Cargando información de secciones y calles...";
            MostrarProgreso = true;

            var callesAgrupadasTarea = Task.Run(() => Domicilios.CallesAgrupadas(SeccionesRepo));

            var tiposContratoTarea = Task.Run(() =>
            {
                return (from tipo in TiposContratoRepo.Datos
                        orderby tipo.Nombre
                        select tipo).ToList();
            });

            callesAgrupadas = await callesAgrupadasTarea;
            TiposContrato = await tiposContratoTarea;

            Secciones = callesAgrupadas.Keys;

            ReestablecerTipoContratoYCalles();
            MostrarProgreso = false;
        }

        private async Task Entrar(object arg)
        {
            if (arg is int id)
            {
                MostrarProgreso = true;
                TextoProgreso = "Cargando datos de usuario...";

                var buscarUsuario = Task.Run(() => Usuarios.Datos.SingleOrDefault(u => u.Id == id));

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

            Enfocar?.Invoke(this, EventArgs.Empty);
        }

        private void Reestablecer()
        {
            MostrarMensajeError = false;
            Contrato = new Contrato()
            {
                Domicilio = new Domicilio()
            };

            ReestablecerTipoContratoYCalles();
        }

        private void ReestablecerTipoContratoYCalles()
        {
            TipoContrato = TiposContrato.FirstOrDefault();
            Seccion = Secciones.FirstOrDefault();
        }

        private bool PuedeAgregarContrato() =>
            UtileriasErrores.NingunoTieneErrores(this, Contrato, Contrato.Domicilio)
            && !Contrato.TieneCamposRequeridosVacios
            && !Contrato.Domicilio.TieneCamposRequeridosVacios
            && Contrato.Usuario != null;

        private async Task<int> AgregarContrato(IProgress<(double, string)> progreso)
        {
            Contrato.TipoContrato = TipoContrato;
            Contrato.Domicilio.Calle = Calle;

            MostrarMensajeError = true;
            PuedeReestablecer = false;

            try
            {
                progreso.Report((0.0, "Agregando contrato..."));
                var resultado = await Contratos.Agregar(Contrato).ConfigureAwait(false);

                progreso.Report((100.0, "Completado."));

                PuedeReestablecer = true;
                Reestablecer();

                await Navegador.Navegar("Usuarios/Listado", Contrato.Usuario.NombreCompleto);

                return resultado.Id;
            }
            finally
            {
                PuedeReestablecer = true;
            }
        }
    }
}

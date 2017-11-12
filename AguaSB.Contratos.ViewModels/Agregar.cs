using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using GGUtils.MVVM.Async;

using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
using AguaSB.Notificaciones;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : ModificarContratoBase
    {
        #region Comandos
        public AsyncDelegateCommand<int> AgregarContratoComando { get; }
        #endregion

        #region Dependencias
        private IRepositorio<Ajustador> AjustadoresRepo { get; }
        private IRepositorio<Tarifa> TarifasRepo { get; }
        #endregion

        public Agregar(
            IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Seccion> seccionesRepo, IRepositorio<Ajustador> ajustadoresRepo, IRepositorio<Tarifa> tarifasRepo,
            IAdministradorNotificaciones notificaciones, INavegador navegador) : base(usuariosRepo, contratosRepo, tiposContratoRepo, seccionesRepo, notificaciones, navegador)
        {
            AjustadoresRepo = ajustadoresRepo ?? throw new ArgumentNullException(nameof(ajustadoresRepo));
            TarifasRepo = tarifasRepo ?? throw new ArgumentNullException(nameof(tarifasRepo));

            AgregarContratoComando = new AsyncDelegateCommand<int>(AgregarContrato, TodosCamposValidos);

            ConfigurarVerificador(() => new ICommand[] { AgregarContratoComando, ReestablecerComando });
        }

        protected override async Task Entrar(object arg)
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

            InvocarEnfocar();
        }

        private Task<int> AgregarContrato(IProgress<(double, string)> progreso) =>
            EjecutarAccion(EjecutarAgregarContrato, progreso);

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

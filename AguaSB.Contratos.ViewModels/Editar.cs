using AguaSB.Datos;
using AguaSB.Navegacion;
using AguaSB.Notificaciones;
using AguaSB.Nucleo;
using GGUtils.MVVM.Async;
using Mehdime.Entity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;

namespace AguaSB.Contratos.ViewModels
{
    public class Editar : ModificarContratoBase
    {
        public DelegateCommand CancelarComando { get; }
        public AsyncDelegateCommand<int> EditarContratoComando { get; }

        public Editar(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Seccion> seccionesRepo, IRepositorio<Calle> callesRepo, IAdministradorNotificaciones notificaciones, INavegador navegador)
            : base(ambito, usuariosRepo, contratosRepo, tiposContratoRepo, seccionesRepo, callesRepo, notificaciones, navegador)
        {
            CancelarComando = new DelegateCommand(Cancelar);
            EditarContratoComando = new AsyncDelegateCommand<int>(EditarContrato, TodosCamposValidos);

            ConfigurarVerificador(() => new ICommand[] { EditarContratoComando, ReestablecerComando });
        }

        protected override async Task Entrar(object arg)
        {
            if (arg is int id)
            {
                MostrarProgreso = true;
                TextoProgreso = "Cargando datos de contrato...";

                var buscarContrato = Task.Run(() => ContratosRepo.Datos.SingleOrDefault(c => c.Id == id));

                if (await buscarContrato is Contrato contrato)
                {
                    Contrato = contrato;

                    InvocarEnfocar();
                }
                else
                {
                    Notificaciones.Lanzar(new NotificacionError()
                    {
                        Titulo = "Error",
                        Clase = "Base de datos",
                        Descripcion = $"No se encontró el contrato con Id = {id}."
                    });
                    var _ = Navegador.Navegar("Usuarios/Listado", null);
                }

                MostrarProgreso = false;
            }
        }

        private void Cancelar()
        {
            var _ = Navegador.Navegar("Usuarios/Listado", Contrato?.Usuario.NombreCompleto);
            Contrato = null;
        }

        private Task<int> EditarContrato(IProgress<(double, string)> progreso) =>
            EjecutarAccion(EjecutarEditarContrato, progreso);

        private Task<Contrato> EjecutarEditarContrato(IProgress<(double, string)> progreso) => Task.Run(() =>
        {
            progreso.Report((0.0, "Guardando cambios..."));

            var resultado = ContratosRepo.Actualizar(Contrato);

            progreso.Report((100.0, "Completado"));

            return resultado;
        });
    }
}

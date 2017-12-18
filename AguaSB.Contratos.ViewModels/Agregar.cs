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
using Mehdime.Entity;

namespace AguaSB.Contratos.ViewModels
{
    public class Agregar : ModificarContratoBase
    {
        #region Comandos
        public AsyncDelegateCommand<int> AgregarContratoComando { get; }
        #endregion

        #region Dependencias
        private IRepositorio<Tarifa> TarifasRepo { get; }
        #endregion

        public Agregar(IDbContextScopeFactory ambito,
            IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Seccion> seccionesRepo, IRepositorio<Calle> callesRepo, IRepositorio<Tarifa> tarifasRepo,
            IAdministradorNotificaciones notificaciones, INavegador navegador) : base(ambito, usuariosRepo, contratosRepo, tiposContratoRepo, seccionesRepo, callesRepo, notificaciones, navegador)
        {
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

                await Task.Run(() =>
                {
                    using (var baseDeDatos = Ambito.CreateReadOnly())
                    {
                        var usuarioBuscado = UsuariosRepo.Datos.SingleOrDefault(u => u.Id == id);

                        if (usuarioBuscado is Usuario usuario)
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
                    }
                }).ConfigureAwait(true);

                MostrarProgreso = false;
            }

            InvocarEnfocar();
        }

        private Task<int> AgregarContrato(IProgress<(double, string)> progreso) =>
            EjecutarAccion(EjecutarAgregarContrato, progreso);

        private Task<Contrato> EjecutarAgregarContrato(IProgress<(double, string)> progreso) => Task.Run(() =>
        {
            using (var baseDeDatos = Ambito.Create())
            {
                progreso.Report((20.0, "Registrando pago inicial..."));

                var pago = new Pago
                {
                    Contrato = Contrato,
                    Desde = Fecha.MesDe(PagadoHasta),
                    Hasta = Fecha.MesDe(PagadoHasta),
                    FechaRegistro = Fecha.Ahora
                };

                pago.Coercer();

                progreso.Report((50.0, "Agregando contrato..."));

                Contrato.Pagos.Add(pago);

                NormalizarReferencias.Contrato(Contrato, UsuariosRepo, TiposContratoRepo, CallesRepo);

                var resultado = ContratosRepo.Agregar(Contrato);

                baseDeDatos.SaveChanges();

                return resultado;
            }
        });
    }
}

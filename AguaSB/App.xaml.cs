using System.Windows;

using Castle.Windsor;
using Castle.Windsor.Installer;

using AguaSB.Inicializadores;
using AguaSB.Notificaciones;

namespace AguaSB
{
    public partial class App : Application
    {
        private IWindsorContainer contenedor;

        protected override void OnStartup(StartupEventArgs e)
        {
            contenedor = new WindsorContainer();

            contenedor.Install(FromAssembly.This());

            ResolverServiciosFueraDeArbolPrincipal();

            var ventanaPrincipal = contenedor.Resolve<IVentanaPrincipal>();
            ventanaPrincipal.Mostrar();
        }

        private void ResolverServiciosFueraDeArbolPrincipal()
        {
            contenedor.Resolve<IInicializador>();

            contenedor.Resolve<ITransformadorNotificaciones>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            contenedor.Dispose();
        }
    }
}

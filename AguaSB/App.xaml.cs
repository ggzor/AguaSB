using System.Windows;

using Castle.Windsor;
using Castle.Windsor.Installer;

using AguaSB.Inicializadores;

namespace AguaSB
{
    public partial class App : Application
    {
        private IWindsorContainer contenedor;

        protected override void OnStartup(StartupEventArgs e)
        {
            contenedor = new WindsorContainer();

            contenedor.Install(FromAssembly.This());

            // TODO: Remover en release
            contenedor.Resolve<IInicializador>();

            var ventanaPrincipal = contenedor.Resolve<IVentanaPrincipal>();
            ventanaPrincipal.Mostrar();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            contenedor.Dispose();
        }
    }
}

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using System.Windows;

namespace AguaSB
{
    public partial class App : Application
    {

        private IWindsorContainer contenedor;

        protected override void OnStartup(StartupEventArgs e)
        {
            contenedor = new WindsorContainer();

            contenedor.Kernel.Resolver.AddSubResolver(new CollectionResolver(contenedor.Kernel, allowEmptyCollections: true));

            contenedor.Register(Component.For<VentanaPrincipalViewModel>());
            contenedor.Register(Component.For<VentanaPrincipal>());

            var ventanaPrincipal = contenedor.Resolve<VentanaPrincipal>();

            ventanaPrincipal.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            contenedor.Dispose();
        }
    }
}

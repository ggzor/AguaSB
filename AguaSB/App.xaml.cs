using System;
using System.Reflection;
using System.Windows;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

using AguaSB.Datos;
using AguaSB.Extensiones;
using AguaSB.ViewModels;
using AguaSB.Views;

namespace AguaSB
{
    public partial class App : Application
    {

        private IWindsorContainer contenedor;

        protected override void OnStartup(StartupEventArgs e)
        {
            contenedor = new WindsorContainer();

            RegistrarResoluciónDeExtensiones();

            contenedor.Register(Component.For(typeof(IRepositorio<>))
                .ImplementedBy(typeof(RepositorioEnMemoria<>)));

            contenedor.Register(Component.For<VentanaPrincipalViewModel>());
            contenedor.Register(Component.For<VentanaPrincipal>());

            var ventanaPrincipal = contenedor.Resolve<VentanaPrincipal>();

            ventanaPrincipal.Show();
        }

        private void RegistrarResoluciónDeExtensiones()
        {
            const string DirectorioViews = "";
            Predicate<AssemblyName> FiltroViews = nombre => nombre.Name.StartsWith(nameof(AguaSB)) && nombre.Name.EndsWith(nameof(Views));

            const string DirectorioViewModels = "";
            Predicate<AssemblyName> FiltroViewModels = nombre => nombre.Name.StartsWith(nameof(AguaSB)) && nombre.Name.EndsWith(nameof(ViewModels));

            // Esto permite que se registren multiples extensiones
            contenedor.Kernel.Resolver
                .AddSubResolver(new CollectionResolver(contenedor.Kernel, allowEmptyCollections: true));

            contenedor.Register(
                Classes.FromAssemblyInDirectory(new AssemblyFilter(DirectorioViews).FilterByName(FiltroViews))
                .BasedOn<IView>().WithService.Self());

            contenedor.Register(
                Classes.FromAssemblyInDirectory(new AssemblyFilter(DirectorioViewModels).FilterByName(FiltroViewModels))
                .BasedOn<IViewModel>().WithService.Self());

            contenedor.Register(
                Classes.FromAssemblyInDirectory(new AssemblyFilter(DirectorioViews).FilterByName(FiltroViews))
                .BasedOn<IExtension>().WithService.Base());
        }

        protected override void OnExit(ExitEventArgs e)
        {
            contenedor.Dispose();
        }
    }
}

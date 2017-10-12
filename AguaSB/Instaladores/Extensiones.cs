using System;
using System.Reflection;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.ViewModels;
using AguaSB.Views;
using AguaSB.Extensiones;

namespace AguaSB.Instaladores
{
    public class Extensiones : IWindsorInstaller
    {
        private const string DirectorioViews = "";
        private static readonly Predicate<AssemblyName> FiltroViews = nombre =>
            nombre.Name.StartsWith(nameof(AguaSB)) && nombre.Name.EndsWith(nameof(Views));

        const string DirectorioViewModels = "";
        private static readonly Predicate<AssemblyName> FiltroViewModels = nombre =>
            nombre.Name.StartsWith(nameof(AguaSB)) && nombre.Name.EndsWith(nameof(ViewModels));

        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
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
    }
}

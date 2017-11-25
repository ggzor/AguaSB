using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

using AguaSB.Inicializadores;

namespace AguaSB.Instaladores
{
    public class Inicializadores : IWindsorInstaller, IInicializador
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Resolver
                .AddSubResolver(new CollectionResolver(container.Kernel, allowEmptyCollections: true));

            container.Register(Component.For<IInicializador>().ImplementedBy<Inicializador>());
            container.Register(Classes.FromThisAssembly().BasedOn<IInicializador>().WithService.FirstInterface());
        }
    }
}

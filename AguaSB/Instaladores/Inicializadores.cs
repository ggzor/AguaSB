using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;

namespace AguaSB.Instaladores
{
    public class Inicializadores : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Kernel.Resolver
                .AddSubResolver(new CollectionResolver(contenedor.Kernel, allowEmptyCollections: true));

            contenedor.Register(Classes.FromThisAssembly().BasedOn<IInicializador>().WithService.FirstInterface());
        }
    }
}

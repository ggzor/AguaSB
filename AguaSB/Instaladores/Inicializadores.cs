using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;

using AguaSB.Inicializadores;

namespace AguaSB.Instaladores
{
    public class Inicializadores : IWindsorInstaller, IInicializador
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Kernel.Resolver
                .AddSubResolver(new CollectionResolver(contenedor.Kernel, allowEmptyCollections: true));

            contenedor.Register(Classes.FromThisAssembly().BasedOn<IInicializador>().WithService.FirstInterface());
        }
    }
}

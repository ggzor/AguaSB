using AguaSB.Operaciones;
using AguaSB.Operaciones.Entity.Ambitos;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Mehdime.Entity;

namespace AguaSB.Instaladores
{
    public class BaseDeDatos : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IAmbientDbContextLocator>().ImplementedBy<AmbientDbContextLocator>());
            container.Register(Component.For<IDbContextScopeFactory>().ImplementedBy<DbContextScopeFactory>());
            container.Register(Component.For<IProveedorAmbito>().ImplementedBy<ProveedorAmbito>());
        }
    }
}

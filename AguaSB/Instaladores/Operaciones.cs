using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using AguaSB.Operaciones.Datos;
using AguaSB.Operaciones.Datos.Entity;

namespace AguaSB.Instaladores
{
    public class Operaciones : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IOperacionesPagos>().ImplementedBy<OperacionesPagos>());
        }
    }
}

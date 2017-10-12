using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Datos.Decoradores;
using AguaSB.Datos;

namespace AguaSB.Instaladores
{
    public class Repositorios : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Register(Component.For(typeof(IRepositorio<>))
                .ImplementedBy(typeof(RepositorioNotificador<>)));

            contenedor.Register(Component.For(typeof(IRepositorio<>))
                .ImplementedBy(typeof(RepositorioEnMemoria<>)));
        }
    }
}

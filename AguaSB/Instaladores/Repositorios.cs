using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Datos;
using AguaSB.Datos.Decoradores;
using AguaSB.Nucleo;

namespace AguaSB.Instaladores
{
    public class Repositorios : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            RegistrarRepositorioNotificador<Usuario>(contenedor);
            RegistrarRepositorioNotificador<Contrato>(contenedor);

            contenedor.Register(Component.For(typeof(IRepositorio<>))
                .ImplementedBy(typeof(RepositorioEnMemoria<>)));
        }

        private static void RegistrarRepositorioNotificador<T>(IWindsorContainer contenedor) where T : IEntidad =>
            contenedor.Register(Component.For<IRepositorio<T>>()
                .ImplementedBy<RepositorioNotificador<T>>());
    }
}

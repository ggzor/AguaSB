using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Datos;
using AguaSB.Datos.Decoradores;
using AguaSB.Nucleo;
using AguaSB.Datos.Entity;

namespace AguaSB.Instaladores
{
    public class Repositorios : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            RegistrarRepositorioNotificador<Usuario>(container);
            RegistrarRepositorioNotificador<Contrato>(container);

            container.Register(Component.For<IRepositorio<Contacto>>()
                .ImplementedBy<RepositorioContactos>());

            container.Register(Component.For(typeof(IRepositorio<>))
                .ImplementedBy(typeof(RepositorioEntity<>)));
        }

        private static void RegistrarRepositorioNotificador<T>(IWindsorContainer contenedor) where T : class, IEntidad =>
            contenedor.Register(Component.For<IRepositorio<T>>()
                .ImplementedBy<RepositorioNotificador<T>>());
    }
}

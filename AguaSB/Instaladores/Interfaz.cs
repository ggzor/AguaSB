using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Notificaciones;

namespace AguaSB.Instaladores
{
    public class Interfaz : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Register(Component.For<ITransformadorNotificaciones>().ImplementedBy<TransformadorNotificaciones>());
            contenedor.Register(Component.For<VentanaPrincipalViewModel>());

            contenedor.Register(Component.For<IVentanaPrincipal>().ImplementedBy<VentanaPrincipal>());
        }
    }
}

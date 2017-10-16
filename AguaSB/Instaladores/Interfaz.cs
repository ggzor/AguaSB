using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Extensiones;
using AguaSB.Navegacion;
using AguaSB.Notificaciones;

namespace AguaSB.Instaladores
{
    public class Interfaz : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Register(Component.For<ITransformadorExtensiones>()
                .ImplementedBy<TransformadorExtensiones>());

            contenedor.Register(Component.For<ITransformadorNotificaciones>()
                .ImplementedBy<TransformadorNotificaciones>());

            contenedor.Register(Component.For<ManejadorNavegacion<Operacion>>());

            contenedor.Register(Component.For<INavegador, NavegadorDirectorio<Operacion>>()
                .ImplementedBy<NavegadorDirectorio<Operacion>>());

            contenedor.Register(Component.For<VentanaPrincipalViewModel>());

            contenedor.Register(Component.For<IVentanaPrincipal>()
                .ImplementedBy<VentanaPrincipal>());
        }
    }
}

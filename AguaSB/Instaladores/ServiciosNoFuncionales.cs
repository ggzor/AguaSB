using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Notificaciones;

namespace AguaSB.Instaladores
{
    public class ServiciosNoFuncionales : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Register(Component.For<IAdministradorNotificaciones, IProveedorNotificaciones>()
                .ImplementedBy<ManejadorNotificaciones>());
        }
    }
}

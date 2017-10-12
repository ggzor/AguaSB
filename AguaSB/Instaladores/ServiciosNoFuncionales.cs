using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using AguaSB.Notificaciones;

namespace AguaSB.Instaladores
{
    public class ServiciosNoFuncionales : IWindsorInstaller
    {
        public void Install(IWindsorContainer contenedor, IConfigurationStore store)
        {
            contenedor.Register(Component.For<IManejadorNotificaciones>()
                .ImplementedBy<ManejadorNotificaciones>());
        }
    }
}

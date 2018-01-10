using Castle.MicroKernel.Registration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AguaSB.Instaladores
{
    public class OperacionesEntity : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Localizadores
            container.Register(
                Classes.FromAssemblyNamed("AguaSB.Operaciones.Entity")
                .Where(t => t.GetInterfaces().Any(i => i.Name.Contains("Localizador")))
                .WithServiceAllInterfaces());

            // Operaciones
            container.Register(
                Classes.FromAssemblyNamed("AguaSB.Operaciones.Entity")
                .Where(t => t.GetInterfaces().Any(i => i.Name.Contains("Operaciones")))
                .WithServiceAllInterfaces());
        }
    }
}

using Castle.MicroKernel.Registration;
using System.Linq;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AguaSB.Instaladores
{
    public class Calculadores : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Classes.FromAssemblyNamed("AguaSB.Operaciones.Implementacion")
                .Where(t =>
                {
                    var interfaces = t.GetInterfaces();
                    return interfaces.Any(i => i.Name.Contains("Calculador"));
                })
                .WithServiceAllInterfaces());
        }
    }
}

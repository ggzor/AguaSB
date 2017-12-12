using Castle.MicroKernel.Registration;
using System;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using AguaSB.Reportes;
using AguaSB.Reportes.Excel;
using System.IO;

namespace AguaSB.Instaladores
{
    public class Tablas : IWindsorInstaller
    {
        public static readonly string DirectorioTablas = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AguaSB", "Tablas");

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            Directory.CreateDirectory(DirectorioTablas);

            container.Register(
                Component.For<IGeneradorTablas>()
                .ImplementedBy<GeneradorTablasExcel>()
                .DependsOn(Parameter.ForKey("direccionTablas")
                .Eq(DirectorioTablas)));
        }
    }
}

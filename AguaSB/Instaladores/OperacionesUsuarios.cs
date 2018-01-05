﻿using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Operaciones.Usuarios;
using AguaSB.Operaciones.Usuarios.Entity;
using AguaSB.Operaciones.Usuarios.Implementacion;
using AguaSB.Operaciones.Usuarios.Legado;
using AguaSB.Operaciones.Usuarios.Utilerias;

using AguaSB.Operaciones.Notas;
using AguaSB.Operaciones.Notas.Entity;

namespace AguaSB.Instaladores
{
    public class OperacionesUsuarios : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILocalizadorUsuarios>().ImplementedBy<LocalizadorUsuarios>());
            container.Register(Component.For<ILocalizadorNotas>().ImplementedBy<LocalizadorNotas>());

            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));

            container.Register(Component.For<IProveedorSugerenciasUsuarios>().ImplementedBy<ProveedorSugerenciasRelevable>());
            container.Register(Component.For<IProveedorSugerenciasUsuarios>().ImplementedBy<SugerenciasPorNota>());
            container.Register(Component.For<IProveedorSugerenciasUsuarios>().ImplementedBy<SugerenciasPorNombre>());
        }
    }
}

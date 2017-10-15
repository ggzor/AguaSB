using System;
using System.Reactive.Linq;

using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using AguaSB.Extensiones;
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
                .ImplementedBy<TransformadorNotificaciones>()
                .OnCreate(RegistrarTransformador));

            contenedor.Register(Component.For<VentanaPrincipalViewModel>());
            contenedor.Register(Component.For<IVentanaPrincipal>().ImplementedBy<VentanaPrincipal>());
        }

        public void RegistrarTransformador(IKernel kernel, ITransformadorNotificaciones transformador)
        {
            var manejadorNotificaciones = kernel.Resolve<IManejadorNotificaciones>();

            var notificaciones = manejadorNotificaciones.Notificaciones
                .Select(transformador.Transformar);

            kernel.Register(Component.For<IObservable<NotificacionView>>()
                .Instance(notificaciones));
        }
    }
}

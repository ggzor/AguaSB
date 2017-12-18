using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Datos.Decoradores;
using AguaSB.Estilos;
using AguaSB.Nucleo;

namespace AguaSB.Notificaciones
{
    public class TransformadorNotificaciones : ITransformadorNotificaciones
    {
        private static Control Preparar(Control elem)
        {
            elem.Width = 40;
            elem.Height = 40;
            elem.Foreground = Brushes.White;

            return elem;
        }

        private static Func<FrameworkElement> IconoModern(PackIconModernKind kind) => () =>
            Preparar(new PackIconModern() { Kind = kind });

        private static Func<FrameworkElement> IconoMaterial(PackIconMaterialKind kind) => () =>
            Preparar(new PackIconMaterial() { Kind = kind });

        private static readonly Dictionary<Type, (Func<FrameworkElement> Icono, Brush Fondo)> Diccionario = new Dictionary<Type, (Func<FrameworkElement> Icono, Brush Fondo)>()
        {
            [typeof(EntidadAgregada<Usuario>)] = (IconoModern(PackIconModernKind.User), Temas.Azul.BrochaSolidaWPF),
            [typeof(EntidadActualizada<Usuario>)] = (IconoMaterial(PackIconMaterialKind.AccountCheck), Temas.Azul.BrochaSolidaWPF),
            [typeof(EntidadAgregada<Contrato>)] = (IconoModern(PackIconModernKind.AlignJustify), Temas.Naranja.BrochaSolidaWPF),
            [typeof(EntidadActualizada<Contrato>)] = (IconoMaterial(PackIconMaterialKind.AccountCardDetails), Temas.Naranja.BrochaSolidaWPF),
            [typeof(EntidadAgregada<Pago>)] = (IconoModern(PackIconModernKind.Money), Temas.Verde.BrochaSolidaWPF),
            [typeof(NotificacionError)] = (IconoModern(PackIconModernKind.Close), Temas.Rojo.BrochaSolidaWPF)
        };

        public NotificacionView Transformar(Notificacion notificacion)
        {
            (var Icono, var Fondo) = ObtenerConfiguracion(notificacion);

            return new NotificacionView()
            {
                Titulo = notificacion.Titulo,
                Contenido = notificacion.Descripcion,
                Clase = notificacion.Clase,
                Fecha = notificacion.Fecha,
                Icono = Icono(),
                Background = Fondo
            };
        }

        private static (Func<FrameworkElement> Icono, Brush Fondo) ObtenerConfiguracion(Notificacion notificacion)
        {
            var tipoNotificacion = notificacion.GetType();

            var resultado = Diccionario.Keys
                .FirstOrDefault(t => t.IsAssignableFrom(tipoNotificacion));

            if (resultado is Type tipo)
                return Diccionario[tipo];
            else
                return (IconoModern(PackIconModernKind.NotificationMultiple), Temas.Azul.BrochaSolidaWPF);
        }
    }
}

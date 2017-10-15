using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using AguaSB.Datos.Decoradores;
using AguaSB.Estilos;
using AguaSB.Nucleo;
using MahApps.Metro.IconPacks;

namespace AguaSB.Notificaciones
{
    public class TransformadorNotificaciones : ITransformadorNotificaciones
    {
        private static Func<FrameworkElement> IconoDe(PackIconModernKind kind) => () =>
            new PackIconModern()
            {
                Kind = kind,
                Width = 40,
                Height = 40,
                Foreground = Brushes.White
            };

        private static readonly (Func<FrameworkElement>, Brush) ConfiguracionUsuarios = (IconoDe(PackIconModernKind.User), Temas.Azul.BrochaSolidaWPF);

        private static readonly Dictionary<Type, (Func<FrameworkElement> Icono, Brush Fondo)> Diccionario = new Dictionary<Type, (Func<FrameworkElement> Icono, Brush Fondo)>()
        {
            [typeof(EntidadAgregada<Usuario>)] = ConfiguracionUsuarios,
            [typeof(EntidadAgregada<Contrato>)] = (IconoDe(PackIconModernKind.AlignJustify), Temas.Naranja.BrochaSolidaWPF)
        };


        public NotificacionView Transformar(Notificacion notificacion)
        {
            (var Icono, var Fondo) = ObtenerConfiguracion(notificacion);

            var notificacionView = new NotificacionView()
            {
                Titulo = notificacion.Titulo,
                Contenido = notificacion.Descripcion,
                Clase = notificacion.Clase,
                Fecha = notificacion.Fecha,
                Icono = Icono(),
                Background = Fondo
            };

            return notificacionView;
        }

        private static (Func<FrameworkElement> Icono, Brush Fondo) ObtenerConfiguracion(Notificacion notificacion)
        {
            var tipo = notificacion.GetType();
            if (Diccionario.ContainsKey(tipo))
                return Diccionario[tipo];
            else
                return (IconoDe(PackIconModernKind.NotificationMultiple), Temas.Azul.BrochaSolidaWPF);
        }
    }
}

using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace AguaSB.Extensiones
{
    public class TransformadorExtensiones : ITransformadorExtensiones
    {
        public ExtensionView Transformar(IExtension extension)
        {
            var view = new ExtensionView()
            {
                Titulo = extension.Nombre,
                Descripcion = extension.Descripcion,
                Elementos = extension.Operaciones,
                Icono = extension.Icono,
                FondoIcono = extension.Tema.BrochaSolidaWPF,
            };

            var icono = extension.Icono;

            if (icono is PackIconModern i)
                i.Foreground = Brushes.White;

            icono.Width = 80;
            icono.Height = 80;

            return view;
        }
    }
}

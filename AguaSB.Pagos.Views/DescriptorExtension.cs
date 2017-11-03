using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Estilos;
using AguaSB.Extensiones;

namespace AguaSB.Pagos.Views
{
    public class DescriptorExtension : IExtension
    {
        public DescriptorExtension()
        {
        }

        public string Nombre => nameof(Pagos);

        public string Version => "v0.1.0";

        public string Descripcion =>
            "Administrar y realizar pagos de los contratos en el sistema.";

        public FrameworkElement Icono { get; } = new PackIconModern()
        {
            Kind = PackIconModernKind.Money,
            Foreground = Brushes.White
        };

        public Tema Tema { get; } = Temas.Verde;

        public IEnumerable<Operacion> Operaciones => new Operacion[]
        {
        };
    }
}

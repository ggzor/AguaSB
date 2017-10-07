using System.Collections.Generic;
using System.Windows;

using AguaSB.Estilos;
using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace AguaSB.Contratos.Views
{
    public class DescriptorExtension : IExtension
    {
        public string Nombre => nameof(Contratos);

        public string Version => "v0.1.0";

        public string Descripcion =>
            "Administrar y ver contratos de los usuarios.";

        public FrameworkElement Icono { get; } = new PackIconModern()
        {
            Kind = PackIconModernKind.AlignJustify,
            Foreground = Brushes.White
        };

        public Estilos.Color Tema { get; } = Colores.Naranja;

        public Agregar Agregar { get; set; }

        public IEnumerable<Operacion> Operaciones => new[]
        {
            new Operacion(this, "Agregar contrato", Agregar)
        };
    }
}
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;
using AguaSB.Navegacion;

namespace AguaSB
{
    public class ExtensionPrueba : IExtension
    {
        public string Nombre => "Extensión";

        public string Version => "v1.0.0";

        public string Descripcion =>
            "Este módulo permite la ejecución de cosas geniales, cosas que solo son posibles en este gran programa. ;)";

        public Lazy<FrameworkElement> Icono => new Lazy<FrameworkElement>(() => new PackIconMaterial()
        {
            Width = 80,
            Height = 80,
            Foreground = Brushes.MediumPurple,
            Kind = PackIconMaterialKind.Earth
        });

        public IEnumerable<Operacion> Operaciones => new[] {
            new Operacion("Agregar nueva extensión", () => new StackPanel(){ Background = Brushes.White }, new NodoHoja()),
            new Operacion("Modificar configuración", () => new StackPanel(), new NodoHoja()),
            new Operacion("Buscar nuevas extensiónes", () => new StackPanel(), new NodoHoja()),
            new Operacion("Obtener ayuda", () => new StackPanel(), new NodoHoja())
        };
    }
}

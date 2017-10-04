using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;
using AguaSB.Navegacion;
using AguaSB.ViewModels;

namespace AguaSB
{
    public class ExtensionPrueba : IExtension
    {
        public string Nombre => "Extensión";

        public string Version => "v1.0.0";

        public string Descripcion =>
            "Este módulo permite la ejecución de cosas geniales, cosas que solo son posibles en este gran programa. ;)";

        public FrameworkElement Icono => new PackIconMaterial()
        {
            Kind = PackIconMaterialKind.Earth
        };

        public Estilos.Color Tema { get; } = Estilos.Colores.Azul;

        public IEnumerable<Operacion> Operaciones => new[] {
            new Operacion("Agregar nueva extensión", new StackPanel(){ Background = Brushes.White }),
            new Operacion("Modificar configuración",  new StackPanel()),
            new Operacion("Buscar nuevas extensiónes",  new StackPanel()),
            new Operacion("Obtener ayuda",  new StackPanel())
        };
    }

    public class FakeViewModel : IViewModel
    {
        public INodo<IProveedorServicios> Nodo => new NodoHoja<IProveedorServicios>();
    }
}

using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Estilos;
using AguaSB.Extensiones;

namespace AguaSB.Usuarios.Views
{
    public class DescriptorExtension : IExtension
    {
        public string Nombre => nameof(Usuarios);

        public string Version => "v0.1.0";

        public string Descripcion =>
            "Agregar, actualizar o inhabilitar usuarios de la base de datos.";

        public FrameworkElement Icono { get; } = new PackIconModern()
        {
            Kind = PackIconModernKind.People,
            Foreground = Brushes.White
        };

        public Tema Tema { get; } = Temas.Azul;

        public Agregar Agregar { get; set; }

        public IEnumerable<Operacion> Operaciones => new[]
        {
            new Operacion(this, "Agregar usuario", Agregar, Agregar.ViewModel)
        };
    }
}

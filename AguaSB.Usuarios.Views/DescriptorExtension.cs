using System.Collections.Generic;
using System.Windows;

using AguaSB.Estilos;
using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;
using System.Windows.Media;

namespace AguaSB.Usuarios.Views
{
    public class DescriptorExtension : IExtension
    {
        public string Nombre => nameof(Usuarios);

        public string Version => "v1.0.0";

        public string Descripcion =>
            "Agregar, actualizar o inhabilitar usuarios de la base de datos.";

        #region Operaciones
        public Agregar Agregar { get; set; }
        #endregion

        public FrameworkElement Icono { get; } = new PackIconModern()
        {
            Kind = PackIconModernKind.People,
            Foreground = Brushes.White
        };

        public Estilos.Color Tema { get; } = Colores.Azul;

        public IEnumerable<Operacion> Operaciones => new[]
        {
            new Operacion("Agregar usuario", Agregar)
        };
    }
}

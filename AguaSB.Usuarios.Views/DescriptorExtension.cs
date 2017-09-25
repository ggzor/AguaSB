using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

using AguaSB.Estilos;
using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;

namespace AguaSB.Usuarios.Views
{
    public class DescriptorExtension : IExtension
    {
        public string Nombre => nameof(Usuarios);

        public string Version => "v1.0.0";

        public string Descripcion =>
            "Agregar, actualizar o inhabilitar usuarios de la base de datos. Así como ver estadísticas sobre los adeudos de los usuarios.";

        public IEnumerable<Operacion> Operaciones => Enumerable.Empty<Operacion>();

        public Lazy<FrameworkElement> Icono => new Lazy<FrameworkElement>(() => new PackIconModern()
        {
            Width = 80,
            Height = 80,
            Kind = PackIconModernKind.People,
            Foreground = Colores.Azul.BrochaWPF
        });
    }
}

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Estilos;
using AguaSB.Extensiones;

namespace AguaSB.Contratos.Views
{
    public class DescriptorExtension : IExtension
    {
        public Agregar Agregar { get; }

        public IEnumerable<Operacion> Operaciones { get; }

        public IEnumerable<Operacion> OperacionesMenuPrincipal { get; }

        public DescriptorExtension(Agregar agregar)
        {
            Agregar = agregar ?? throw new ArgumentNullException(nameof(agregar));

            OperacionesMenuPrincipal = new[]
            {
                new Operacion(this, "Agregar contrato", Agregar, Agregar, Agregar.ViewModel)
            };

            Operaciones = OperacionesMenuPrincipal;
        }

        public string Nombre => nameof(Contratos);

        public string Version => "v0.1.0";

        public string Descripcion =>
            "Administrar los contratos de los usuarios y las tarifas en el sistema.";

        public FrameworkElement Icono { get; } = new PackIconModern()
        {
            Kind = PackIconModernKind.AlignJustify,
            Foreground = Brushes.White
        };

        public Tema Tema { get; } = Temas.Naranja;
    }
}
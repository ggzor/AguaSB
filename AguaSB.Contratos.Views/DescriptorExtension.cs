using System;
using System.Collections.Generic;
using System.Linq;
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

        public Editar Editar { get; }

        public IEnumerable<Operacion> Operaciones { get; }

        public IEnumerable<Operacion> OperacionesMenuPrincipal { get; }

        public DescriptorExtension(Agregar agregar, Editar editar)
        {
            Agregar = agregar ?? throw new ArgumentNullException(nameof(agregar));
            Editar = editar ?? throw new ArgumentNullException(nameof(editar));

            OperacionesMenuPrincipal = new[]
            {
                new Operacion(this, "Agregar contrato", Agregar, Agregar, Agregar.ViewModel)
            };

            Operaciones = OperacionesMenuPrincipal.Concat(new[] {
                new Operacion(this, "Editar contrato", Editar, Editar, Editar.ViewModel)
            });
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
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
        public Agregar Agregar { get; }

        public Listado Listado { get; }

        public Pagar Pagar { get; }

        public IEnumerable<Operacion> Operaciones { get; }

        public IEnumerable<Operacion> OperacionesMenuPrincipal { get; }

        public DescriptorExtension(Agregar agregar, Listado listado, Pagar pagar)
        {
            Agregar = agregar ?? throw new ArgumentNullException(nameof(agregar));
            Listado = listado ?? throw new ArgumentNullException(nameof(listado));
            Pagar = pagar ?? throw new ArgumentNullException(nameof(pagar));

            OperacionesMenuPrincipal = new[]
            {
                new Operacion(this, "Hacer pago", Agregar, Agregar, Agregar.ViewModel),
                new Operacion(this, "Listado de pagos", Listado, Listado, Listado.ViewModel),
                new Operacion(this, "Pagar", Pagar, Pagar, Pagar.ViewModel)
            };

            Operaciones = OperacionesMenuPrincipal;
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
    }
}

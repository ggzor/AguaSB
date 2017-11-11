using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Estilos;
using AguaSB.Extensiones;

namespace AguaSB.Usuarios.Views
{
    public class DescriptorExtension : IExtension
    {
        public Agregar Agregar { get; }

        public Listado Listado { get; }

        public Editar Editar { get; }

        public IEnumerable<Operacion> Operaciones { get; }

        public IEnumerable<Operacion> OperacionesMenuPrincipal { get; }

        public DescriptorExtension(Agregar agregar, Editar editar, Listado listado)
        {
            Agregar = agregar ?? throw new ArgumentNullException(nameof(agregar));
            Editar = editar ?? throw new ArgumentNullException(nameof(editar));
            Listado = listado ?? throw new ArgumentNullException(nameof(listado));

            OperacionesMenuPrincipal = new[]
            {
                new Operacion(this, "Agregar usuario", Agregar, Agregar, Agregar.ViewModel),
                new Operacion(this, "Listado de usuarios", Listado, Listado, Listado.ViewModel)
            };

            Operaciones = OperacionesMenuPrincipal.Concat(new[] {
                new Operacion(this, "Editar usuario", Editar, Editar, Editar.ViewModel)
            });
        }

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
    }
}

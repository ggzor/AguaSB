using System;
using System.Windows;

using AguaSB.ViewModels;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public IExtension Extension { get; }

        public string Nombre { get; }

        public FrameworkElement View { get; }

        public IViewModel ViewModel { get; }

        public Operacion(IExtension extension, string nombre, FrameworkElement view, IViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            Extension = extension ?? throw new ArgumentNullException(nameof(extension));
            Nombre = nombre;
            View = view ?? throw new ArgumentNullException(nameof(view));
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));
        }

        public override string ToString() => Nombre;
    }
}

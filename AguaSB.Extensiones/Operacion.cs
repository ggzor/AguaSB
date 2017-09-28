using System;
using System.Windows;

using AguaSB.ViewModels;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public string Nombre { get; }

        public Lazy<FrameworkElement> View { get; }

        public IViewModel ViewModel { get; set; }

        public Operacion(string nombre, Func<IViewModel, FrameworkElement> generadorView, IViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            Nombre = nombre;
            View = new Lazy<FrameworkElement>(() => generadorView(viewModel) ?? throw new ArgumentNullException(nameof(generadorView)));
            ViewModel = viewModel;
        }
    }
}

using System;
using System.Windows;

using AguaSB.ViewModels;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public string Nombre { get; }

        public Lazy<FrameworkElement> View { get; }

        public Lazy<IViewModel> ViewModel { get; set; }

        public Operacion(string nombre, Func<IViewModel, FrameworkElement> generadorView, Func<IViewModel> generadorViewModel)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            if (generadorView == null)
                throw new ArgumentNullException(nameof(generadorView));

            if (generadorViewModel == null)
                throw new ArgumentNullException(nameof(generadorViewModel));

            Nombre = nombre;
            View = new Lazy<FrameworkElement>(() => generadorView(ViewModel.Value));
            ViewModel = new Lazy<IViewModel>(generadorViewModel);
        }
    }
}

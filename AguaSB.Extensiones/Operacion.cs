using System;
using System.Windows;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public string Nombre { get; }

        public FrameworkElement View { get; }

        public Operacion(string nombre, FrameworkElement view)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            Nombre = nombre;
            View = view ?? throw new ArgumentNullException(nameof(view));
        }

        public override string ToString() => Nombre;
    }
}

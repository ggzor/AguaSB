using System;
using System.Windows;

using AguaSB.Navegacion;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public string Nombre { get; }

        public Lazy<FrameworkElement> Visualizacion { get; }

        public INodo Nodo { get; }

        public Operacion(string nombre, Func<FrameworkElement> visualizacion, INodo nodo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            Nombre = nombre;
            Visualizacion = new Lazy<FrameworkElement>(visualizacion ?? throw new ArgumentNullException(nameof(visualizacion)));
            Nodo = nodo ?? throw new ArgumentNullException(nameof(nodo));
        }
    }
}

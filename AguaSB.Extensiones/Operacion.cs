using System;
using System.Windows;

using AguaSB.Navegacion;

namespace AguaSB.Extensiones
{
    public class Operacion
    {
        public string Nombre { get; }

        public FrameworkElement Visualizacion { get; }

        public INodo Nodo { get; }

        public Operacion(string nombre, FrameworkElement visualizacion, INodo nodo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre de la operación debe tener al menos un caracter");

            Nombre = nombre;
            Visualizacion = visualizacion ?? throw new ArgumentNullException(nameof(visualizacion));
            Nodo = nodo ?? throw new ArgumentNullException(nameof(nodo));
        }
    }
}

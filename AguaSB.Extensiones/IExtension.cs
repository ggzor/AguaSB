using System;
using System.Collections.Generic;
using System.Windows;

namespace AguaSB.Extensiones
{
    public interface IExtension
    {
        string Nombre { get; }

        string Version { get; }

        string Descripcion { get; }

        Lazy<FrameworkElement> Icono { get; }

        IEnumerable<Operacion> Operaciones { get; }
    }
}

using System.Collections.Generic;
using System.Windows;

namespace AguaSB.Extensiones
{
    public interface IExtension
    {
        string Nombre { get; }

        string Version { get; }

        FrameworkElement CrearIcono();

        IEnumerable<Operacion> Operaciones { get; }
    }
}

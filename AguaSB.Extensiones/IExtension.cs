using System.Collections.Generic;
using System.Windows;

using AguaSB.Estilos;

namespace AguaSB.Extensiones
{
    public interface IExtension
    {
        string Nombre { get; }

        string Version { get; }

        string Descripcion { get; }

        FrameworkElement Icono { get; }

        Tema Tema { get; }

        IEnumerable<Operacion> Operaciones { get; }
    }
}

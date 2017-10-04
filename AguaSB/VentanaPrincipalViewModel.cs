using System;
using System.Collections.Generic;

using AguaSB.Extensiones;

namespace AguaSB
{
    public class VentanaPrincipalViewModel
    {
        public IEnumerable<IExtension> Extensiones { get; }

        public VentanaPrincipalViewModel(IEnumerable<IExtension> extensiones)
        {
            Extensiones = extensiones ?? throw new ArgumentNullException(nameof(extensiones));
        }

    }
}

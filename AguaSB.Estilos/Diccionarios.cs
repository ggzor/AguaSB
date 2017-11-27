using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Windows;

namespace AguaSB.Estilos
{
    public static class Diccionarios
    {
        public static readonly IDictionary<string, Func<FrameworkElement>> TiposContacto = new Dictionary<string, Func<FrameworkElement>>
        {
            ["Default"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Contacts },
            ["Teléfono"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Phone },
            ["Email"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Email }
        };
    }
}

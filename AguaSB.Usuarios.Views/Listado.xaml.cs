using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

using AguaSB.Views;
using AguaSB.Views.Utilerias;

namespace AguaSB.Usuarios.Views
{
    public partial class Listado : UserControl, IView
    {
        public ViewModels.Listado ViewModel { get; }

        public Listado(ViewModels.Listado viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            if (FindResource("Iconos") is DictionaryConverter conversor)
            {
                conversor.Dictionary = Iconos;
            }
        }

        public IDictionary Iconos { get; } = new Dictionary<string, UIElement>
        {
            ["Id"] = new PackIconEntypo { Kind = PackIconEntypoKind.Fingerprint },
            ["Nombre"] = new PackIconMaterial { Kind = PackIconMaterialKind.Account },
            ["Adeudo"] = new PackIconModern { Kind = PackIconModernKind.CurrencyDollar },
            ["Fecha de registro"] = new PackIconMaterial { Kind = PackIconMaterialKind.CalendarPlus },
            ["Contratos"] = new PackIconModern { Kind = PackIconModernKind.AlignJustify },
            ["Último pago"] = new PackIconModern { Kind = PackIconModernKind.CalendarDollar },
            ["Sección"] = new PackIconMaterial { Kind = PackIconMaterialKind.ViewGrid },
            ["Calle"] = new PackIconEntypo { Kind = PackIconEntypoKind.Address },
            ["Número"] = new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.Hashtag }
        };

        private void MostrarFiltros(object sender, RoutedEventArgs e) => Filtros.IsOpen = true;

        private void MostrarFiltrosColumnas(object sender, RoutedEventArgs e) => FiltrosColumnas.IsOpen = true;
    }
}

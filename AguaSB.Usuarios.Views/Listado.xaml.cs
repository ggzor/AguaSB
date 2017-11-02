using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

using MahApps.Metro.IconPacks;

using AguaSB.Views;
using AguaSB.Views.Utilerias;
using AguaSB.Usuarios.ViewModels.Dtos;
using System.Windows.Data;

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

            ViewModel.AgrupadorCambiado += AgrupadorCambiado;
        }

        private void AgrupadorCambiado(object sender, Agrupador e)
        {
            ListaResultados.Items.GroupDescriptions.Clear();
            if (e != null && e != Agrupador.Ninguno)
            {
                var descriptor = new PropertyGroupDescription(e.Propiedad);

                if (e.Ordenador != null)
                    descriptor.CustomSort = new FuncComparer(e.Ordenador);

                if (e.Conversor != null)
                    descriptor.Converter = new FuncValueConverter(e.Conversor);

                ListaResultados.Items.GroupDescriptions.Add(descriptor);
            }
        }

        private GridViewColumnHeader columna;
        private SortAdorner adornador;

        private void Columna_Seleccionada(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader nuevaColumna = (sender as GridViewColumnHeader);
            string sortBy = nuevaColumna.Tag.ToString();
            if (columna != null)
            {
                AdornerLayer.GetAdornerLayer(columna).Remove(adornador);
                ListaResultados.Items.SortDescriptions.Clear();
            }

            var newDir = ListSortDirection.Ascending;
            if (columna == nuevaColumna && adornador.Direction == newDir)
                newDir = ListSortDirection.Descending;

            columna = nuevaColumna;
            adornador = new SortAdorner(columna, newDir);
            AdornerLayer.GetAdornerLayer(columna).Add(adornador);
            ListaResultados.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }

        public IDictionary Iconos { get; } = new Dictionary<string, UIElement>
        {
            ["Id"] = new PackIconEntypo { Kind = PackIconEntypoKind.Fingerprint },
            ["Nombre"] = new PackIconMaterial { Kind = PackIconMaterialKind.Account },
            ["Adeudo"] = new PackIconModern { Kind = PackIconModernKind.CurrencyDollar },
            ["Fecha de registro"] = new PackIconMaterial { Kind = PackIconMaterialKind.CalendarPlus },
            ["Contratos"] = new PackIconModern { Kind = PackIconModernKind.AlignJustify },
            ["Último pago"] = new PackIconModern { Kind = PackIconModernKind.CalendarDollar },
            ["Pagado hasta"] = new PackIconMaterial { Kind = PackIconMaterialKind.CalendarClock },
            ["Sección"] = new PackIconMaterial { Kind = PackIconMaterialKind.ViewGrid },
            ["Calle"] = new PackIconEntypo { Kind = PackIconEntypoKind.Address },
            ["Número"] = new PackIconFontAwesome { Kind = PackIconFontAwesomeKind.Hashtag }
        };

        private void MostrarFiltros(object sender, RoutedEventArgs e) => Filtros.IsOpen = true;

        private void MostrarFiltrosColumnas(object sender, RoutedEventArgs e) => FiltrosColumnas.IsOpen = true;
    }

    public class SortAdorner : Adorner
    {
        private static readonly Geometry ascGeometry =
            Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static readonly Geometry descGeometry =
            Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; }

        public SortAdorner(UIElement element, ListSortDirection dir) : base(element)
        {
            Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            var transform = new TranslateTransform(
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2
            );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }
}

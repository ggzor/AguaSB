using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using MahApps.Metro.IconPacks;
using MoreLinq;

using AguaSB.Views;
using AguaSB.Views.Utilerias;
using AguaSB.Usuarios.ViewModels.Dtos;

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
            ViewModel.Enfocar += (_, __) => Busqueda.Focus();
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

        private GridViewColumnHeader columnaActual;
        private SortAdorner adornadorActual;

        private void Columna_Seleccionada(object sender, RoutedEventArgs e)
        {
            if (sender is GridViewColumnHeader nuevaColumna)
            {
                RemoverOrdenamientoActual();

                if (columnaActual == nuevaColumna)
                {
                    if (adornadorActual.Direction == ListSortDirection.Ascending)
                    {
                        EstablecerOrdenamiento(columnaActual, ListSortDirection.Descending);
                    }
                    else
                    {
                        columnaActual = null;
                        adornadorActual = null;
                    }
                }
                else
                {
                    EstablecerOrdenamiento(nuevaColumna, ListSortDirection.Ascending);
                }
            }
        }

        private void EstablecerOrdenamiento(GridViewColumnHeader columna, ListSortDirection direccion)
        {
            var criterioOrdenamiento = columna.Tag.ToString();
            columnaActual = columna;
            adornadorActual = new SortAdorner(columna, direccion);
            var ordenamiento = new SortDescription(criterioOrdenamiento, direccion);

            AdornerLayer.GetAdornerLayer(columnaActual).Add(adornadorActual);
            ListaResultados.Items.SortDescriptions.Add(ordenamiento);
        }

        private void RemoverOrdenamientoActual()
        {
            if (columnaActual != null && adornadorActual != null)
            {
                AdornerLayer.GetAdornerLayer(columnaActual).Remove(adornadorActual);
                ListaResultados.Items.SortDescriptions.Clear();
            }
        }

        private void MostrarFiltros(object sender, RoutedEventArgs e) => Filtros.IsOpen = true;

        private void MostrarFiltrosColumnas(object sender, RoutedEventArgs e) => FiltrosColumnas.IsOpen = true;

        private void ManejarTeclas(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != 0)
            {
                switch (e.Key)
                {
                    case Key.A:
                        AjustarColumnas();
                        break;
                    case Key.C:
                        if (ListaResultados.SelectedItems.Count == 1)
                            EjecutarAgregarContrato();
                        break;
                    case Key.E:
                        if (ListaResultados.SelectedItems.Count == 1)
                            EjecutarEditarUsuario();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.F5:
                        ViewModel.BuscarComando.Execute(null);
                        break;
                }
            }
        }

        private void AjustarColumnas()
        {
            if (ListaResultados.View is GridView view)
            {
                view.Columns
                    .Where(c => c.GetValue(AguaSB.Views.Utilerias.Columnas.EsVisibleProperty) is bool b && b)
                    .ForEach(c =>
                    {
                        if (double.IsNaN(c.Width))
                            c.Width = ActualWidth;

                        c.Width = double.NaN;
                    });
            }
        }

        private void AjustarColumnas_Click(object sender, RoutedEventArgs e) => AjustarColumnas();

        private void Actualizar_Click(object sender, RoutedEventArgs e) => ViewModel.BuscarComando.Execute(null);

        private void SeleccionCambiada(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoDeBotonesDependientesDeSeleccion();
        }

        private void ActualizarEstadoDeBotonesDependientesDeSeleccion()
        {
            new[] { AgregarContrato, EditarUsuario }
                    .ForEach(_ => _.IsEnabled = ListaResultados.SelectedItems.Count == 1);
        }

        private void AgregarContrato_Click(object sender, RoutedEventArgs e) =>
            EjecutarAgregarContrato();

        private void EjecutarAgregarContrato() => ViewModel.AgregarContratoComando.Execute(ListaResultados.SelectedItem);

        private void EditarUsuario_Click(object sender, RoutedEventArgs e) => EjecutarEditarUsuario();

        private void EjecutarEditarUsuario() => ViewModel.EditarUsuarioComando.Execute(ListaResultados.SelectedItem);
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

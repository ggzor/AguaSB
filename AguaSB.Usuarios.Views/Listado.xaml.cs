using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MahApps.Metro.IconPacks;
using MoreLinq;
using AguaSB.Views.Utilerias;
using AguaSB.Usuarios.Views.Utilerias;
using AguaSB.Usuarios.ViewModels.Dtos;
using System.Windows.Controls.Primitives;

namespace AguaSB.Usuarios.Views
{
    public partial class Listado : UserControl, AguaSB.Views.IView
    {
        public ViewModels.Listado ViewModel { get; }

        public DelegateCommand UsuarioAnteriorComando { get; set; }

        public DelegateCommand UsuarioSiguienteComando { get; set; }

        private ManejadorOrdenamientoColumnas ManejadorOrdenamiento { get; }

        public Listado(ViewModels.Listado viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            if (FindResource("Iconos") is DictionaryConverter conversor)
            {
                conversor.Dictionary = Iconos;
            }

            if (FindResource("TiposContacto") is DictionaryConverter conversorTiposContacto && FindResource("IconoLateral") is Style estiloIcono)
            {
                object Procesar(object value, object parametro, object posibleValor)
                {
                    if (posibleValor is Func<FrameworkElement> generador)
                    {
                        var icono = generador();

                        icono.Style = estiloIcono;

                        return icono;
                    }
                    else
                    {
                        return null;
                    }
                }

                conversorTiposContacto.Dictionary = (IDictionary)Estilos.Diccionarios.TiposContacto;
                conversorTiposContacto.PostProcessCallback = Procesar;
            }

            ViewModel.Enfocar += (_, __) => Busqueda.Focus();

            ManejadorOrdenamiento = new ManejadorOrdenamientoColumnas(ListaResultados);
            ViewModel.OrdenamientoCambiado += (_, ordenamiento) => ManejadorOrdenamiento.SeleccionarOrdenamiento(ordenamiento);

            ConfigurarComandos();
            new ManejadorTeclas(PanelResultados, ManejarTeclas, ManejarTeclasConControl);
        }

        private void ConfigurarComandos()
        {
            IEnumerable<ResultadoUsuario> UsuariosAntesDelSeleccionado() =>
                ListaResultados.Items.OfType<ResultadoUsuario>()
                .Take(ListaResultados.SelectedIndex)
                .Where(_ => _.NoEsTitulo);

            IEnumerable<ResultadoUsuario> UsuariosDespuesDelSeleccionado() =>
                ListaResultados.Items.OfType<ResultadoUsuario>()
                .Skip(ListaResultados.SelectedIndex + 1)
                .Where(_ => _.NoEsTitulo);

            bool HayUsuarioAntesDelSeleccionado() =>
                ListaResultados.SelectedIndex > 0
                && UsuariosAntesDelSeleccionado().Any();

            bool HayUsuarioDespuesDelSeleccionado() =>
                ListaResultados.SelectedIndex >= 0 && ListaResultados.SelectedIndex + 1 < ListaResultados.Items.Count
                && UsuariosDespuesDelSeleccionado().Any();

            void SeleccionarUsuarioAnterior()
            {
                ListaResultados.SelectedItem = UsuariosAntesDelSeleccionado().LastOrDefault();
                EnfocarElementoActual();
            }

            void SeleccionarUsuarioSiguiente()
            {
                ListaResultados.SelectedItem = UsuariosDespuesDelSeleccionado().FirstOrDefault();
                EnfocarElementoActual();
            }

            UsuarioAnteriorComando = new DelegateCommand(SeleccionarUsuarioAnterior, HayUsuarioAntesDelSeleccionado);
            UsuarioSiguienteComando = new DelegateCommand(SeleccionarUsuarioSiguiente, HayUsuarioDespuesDelSeleccionado);

            void ManejarFlechas(Key k)
            {
                switch (k)
                {
                    case Key.Left:
                        if (HayUsuarioAntesDelSeleccionado())
                            SeleccionarUsuarioAnterior();
                        break;
                    case Key.Right:
                        if (HayUsuarioDespuesDelSeleccionado())
                            SeleccionarUsuarioSiguiente();
                        break;
                    case Key.Escape:
                        DetallesUsuario.IsOpen = false;
                        break;
                }
            }

            DetallesUsuario.Loaded += (_, __) =>
            {
                var t = DetallesUsuario.Template;
                var p = (Popup)t.FindName("PART_Popup", DetallesUsuario);
                new ManejadorTeclas(p, ManejarFlechas);
            };
        }

        private void EnfocarElementoActual() =>
            ListaResultados.ScrollIntoView(ListaResultados.SelectedItem);

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

        private void Columna_Seleccionada(object sender, RoutedEventArgs e)
        {
            var ordenamiento = (IOrdenamiento)((GridViewColumnHeader)sender).Tag;

            ordenamiento.Cambiar();

            ViewModel.Ordenamiento = ordenamiento;
        }

        private void MostrarFiltros(object sender, RoutedEventArgs e) => Filtros.IsOpen = true;

        private void MostrarFiltrosColumnas(object sender, RoutedEventArgs e) => FiltrosColumnas.IsOpen = true;

        private void ManejarTeclas(Key tecla)
        {
            switch (tecla)
            {
                case Key.F5:
                    ViewModel.Buscar(ViewModels.Listado.ModoBusqueda.Forzar);
                    break;
            }
        }

        private void ManejarTeclasConControl(Key tecla)
        {
            switch (tecla)
            {
                case Key.C:
                    if (EstaSeleccionadoUnUsuario())
                        EjecutarAgregarContrato();
                    break;
                case Key.D:
                    if (EstaSeleccionadoUnUsuario())
                        VerDetalles.Command?.Execute(null);
                    break;
                case Key.E:
                    if (EstaSeleccionadoUnUsuario())
                        EjecutarEditarUsuario();
                    break;
            }
        }

        private void Actualizar_Click(object sender, RoutedEventArgs e) => ViewModel.Buscar(ViewModels.Listado.ModoBusqueda.Forzar);

        private void SeleccionCambiada(object sender, SelectionChangedEventArgs e)
        {
            ActualizarEstadoDeBotonesDependientesDeSeleccion();

            new[] { UsuarioAnteriorComando, UsuarioSiguienteComando }.ForEach(_ => _.RaiseCanExecuteChanged());
        }

        private void ActualizarEstadoDeBotonesDependientesDeSeleccion()
        {
            (new[] { AgregarContrato, EditarUsuario, VerDetalles })
                    .ForEach(_ => _.IsEnabled = EstaSeleccionadoUnUsuario());
        }

        private bool EstaSeleccionadoUnUsuario() =>
            ListaResultados.SelectedItems.Count == 1 && ListaResultados.SelectedItem is ResultadoUsuario resultado && resultado.NoEsTitulo;

        private void AgregarContrato_Click(object sender, RoutedEventArgs e) =>
            EjecutarAgregarContrato();

        private void EditarUsuario_Click(object sender, RoutedEventArgs e) => EjecutarEditarUsuario();

        private void EjecutarAgregarContrato() => ViewModel.AgregarContratoComando.Execute(ListaResultados.SelectedItem);
        private void EjecutarEditarUsuario() => ViewModel.EditarUsuarioComando.Execute(ListaResultados.SelectedItem);

        private void NavegadorResultados_SeleccionCambiada(object sender, SelectionChangedEventArgs e)
        {
            if (NavegadorResultados.SelectedItem is PuntoNavegacion p && p.Indice >= 0 && p.Indice < ListaResultados.Items.Count)
            {
                ListaResultados.SelectedIndex = p.Indice;
                SeleccionarElemento(p.Indice);

                EnfocarElementoActual();
            }
        }

        private void SeleccionarElemento(int indice)
        {
            var deslizador = VisualTreeUtils.FindVisualChild<ScrollViewer>(ListaResultados);

            deslizador.ScrollToVerticalOffset(indice);
        }

        private void AbriendoDetallesDeUsuario(object sender, MaterialDesignThemes.Wpf.DialogOpenedEventArgs eventArgs)
        {
        }

        private void CerrandoDetallesDeUsuario(object sender, MaterialDesignThemes.Wpf.DialogClosingEventArgs eventArgs) => EnfocarElementoActual();
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using MahApps.Metro.IconPacks;
using MoreLinq;

using AguaSB.Estilos;
using AguaSB.Nucleo;
using AguaSB.Views.Utilerias;
using System.Waf.Applications;
using System.Collections.Specialized;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class ContactosView : UserControl, IEnfocable
    {
        public DelegateCommand AgregarContactoComando { get; }

        public DelegateCommand BorrarContactoComando { get; }

        public ContactosView()
        {
            AgregarContactoComando = new DelegateCommand(AgregarContacto);
            BorrarContactoComando = new DelegateCommand(BorrarContacto);

            InitializeComponent();

            BuscarYUnirRecursos();

            ((INotifyCollectionChanged)Lista.Items).CollectionChanged += ContactosCambiados;

            Contactos = new ObservableCollection<Contacto>();
        }

        private void BuscarYUnirRecursos()
        {
            if (FindResource("Iconos") is CallbackConverter conversor)
            {
                conversor.Callback = (arg, param) =>
                {
                    FrameworkElement icono;
                    if (arg is string clave && Iconos.ContainsKey(clave))
                    {
                        icono = Iconos[clave]();
                    }
                    else
                    {
                        icono = Iconos["Default"]();
                    }

                    if (param is "N")
                        icono.Margin = new Thickness(16, 0, 0, 0);

                    return icono;
                };
            }

            EstiloIcono = (Style)FindResource("IconoLateral");
        }

        private async void ContactosCambiados(object sender, NotifyCollectionChangedEventArgs e)
        {
            await Task.Delay(20).ConfigureAwait(true);

            if (Lista.Items.Count > 1)
            {
                ObtenerCajasDeContactos()
                    .Windowed(2)
                    .ForEach(w => w.Fold((t1, t2) =>
                    {
                        t1.SetValue(Foco.SiguienteFocoProperty, t2);
                        return 0;
                    }));
            }

            if (Lista.HasItems)
                ObtenerCajasDeContactos().LastOrDefault()?.SetValue(Foco.SiguienteFocoProperty, GetValue(Foco.SiguienteFocoProperty));

            if (e.Action == NotifyCollectionChangedAction.Add)
                ObtenerCajasDeContactos().LastOrDefault()?.Focus();
        }

        private void BotonAgregar_Click(object sender, RoutedEventArgs e) => OpcionesTiposContacto.IsOpen = true;

        private void AgregarContacto(object param)
        {
            if (param is TipoContacto contacto)
            {
                var nuevoContacto = new Contacto { TipoContacto = contacto };

                Contactos.Add(nuevoContacto);

                OpcionesTiposContacto.IsOpen = false;
            }
        }

        private void BorrarContacto(object param)
        {
            if (param is Contacto c)
                Contactos.Remove(c);
        }

        public void Enfocar()
        {
            if (Lista.HasItems)
                ObtenerCajasDeContactos().FirstOrDefault()?.Focus();
            else
                Foco.EnfocarSiguiente(this);
        }

        private IEnumerable<TextBox> ObtenerCajasDeContactos() =>
            Enumerable.Range(0, Lista.Items.Count)
                .Select(Lista.ItemContainerGenerator.ContainerFromIndex)
                .Where(i => i != null)
                .Cast<ListViewItem>()
                .Select(VisualTreeUtils.FindVisualChild<ContentPresenter>)
                .Select(c => Lista.ItemTemplate.FindName("Box", c))
                .Cast<TextBox>();

        public static Style EstiloIcono { get; set; }

        public Dictionary<string, Func<FrameworkElement>> Iconos { get; } = new Dictionary<string, Func<FrameworkElement>>
        {
            ["Default"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Contacts, Style = EstiloIcono },
            ["Teléfono"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Phone, Style = EstiloIcono },
            ["Email"] = () => new PackIconMaterial { Kind = PackIconMaterialKind.Email, Style = EstiloIcono }
        };

        public IEnumerable<TipoContacto> TiposContacto
        {
            get { return (IEnumerable<TipoContacto>)GetValue(TiposContactoProperty); }
            set { SetValue(TiposContactoProperty, value); }
        }

        public ObservableCollection<Contacto> Contactos
        {
            get { return (ObservableCollection<Contacto>)GetValue(ContactosProperty); }
            set { SetValue(ContactosProperty, value); }
        }

        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }

        #region DP´s
        public static readonly DependencyProperty TiposContactoProperty =
            DependencyProperty.Register(nameof(TiposContacto), typeof(IEnumerable<TipoContacto>), typeof(ContactosView), new PropertyMetadata(Enumerable.Empty<TipoContacto>()));

        public static readonly DependencyProperty ContactosProperty =
            DependencyProperty.Register(nameof(Contactos), typeof(ObservableCollection<Contacto>), typeof(ContactosView), new PropertyMetadata(null));

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(ContactosView), new PropertyMetadata(true));
        #endregion

        #region Sincronizar icono de caja de texto
        public static readonly DependencyProperty UnirConProperty =
            DependencyProperty.RegisterAttached("UnirCon", typeof(TextBox), typeof(ContactosView), new PropertyMetadata(null, Unir));

        private static void Unir(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is TextBox box && d is ContentControl elem && elem.Content is FrameworkElement contenido)
            {
                var b = new Binding
                {
                    Source = box,
                    Path = new PropertyPath(nameof(IsKeyboardFocusWithin))
                };

                contenido.SetBinding(Icono.EnfocadoProperty, b);
                contenido.SetValue(TagProperty, box);
            }
        }

        public static void SetUnirCon(ContentControl elem, TextBox box) => elem.SetValue(UnirConProperty, box);
        public static TextBox GetUnirCon(ContentControl elem) => (TextBox)elem.GetValue(UnirConProperty);
        #endregion
    }
}

using System;
using System.Collections;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GGUtils.WPF.Controles
{
    public enum AutocompleteMode { Search, Filter }

    public partial class AutocompleteTextBox : UserControl
    {
        public AutocompleteTextBox()
        {
            InitializeComponent();

            var ourWindow = Application.Current.MainWindow;
            Mouse.AddPreviewMouseDownHandler(ourWindow, Cerrar);
        }

        #region Properties
        public IEnumerable Items
        {
            get { return (IEnumerable)GetValue(ItemsProperty); }
            set { SetValue(ItemsProperty, value); }
        }

        public IEnumerable ItemsShown
        {
            get { return (IEnumerable)GetValue(ItemsShownProperty); }
            private set { SetValue(ItemsShownPropertyKey, value); }
        }

        public Func<object, bool> Filter
        {
            get { return (Func<object, bool>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        public AutocompleteMode AutocompleteMode
        {
            get { return (AutocompleteMode)GetValue(AutocompleteModeProperty); }
            set { SetValue(AutocompleteModeProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }



        public Style ItemContainerStyle
        {
            get { return (Style)GetValue(ItemContainerStyleProperty); }
            set { SetValue(ItemContainerStyleProperty, value); }
        }

        public Style TextBoxStyle
        {
            get { return (Style)GetValue(TextBoxStyleProperty); }
            set { SetValue(TextBoxStyleProperty, value); }
        }

        public Style ScrollViewerStyle
        {
            get { return (Style)GetValue(ScrollViewerStyleProperty); }
            set { SetValue(ScrollViewerStyleProperty, value); }
        }

        public Style BorderStyle
        {
            get { return (Style)GetValue(BorderStyleProperty); }
            set { SetValue(BorderStyleProperty, value); }
        }

        public UIElement NoItemMatchingControl
        {
            get { return (UIElement)GetValue(NoItemMatchingControlProperty); }
            set { SetValue(NoItemMatchingControlProperty, value); }
        }
        #endregion

        #region DP´s
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(AutocompleteTextBox), new PropertyMetadata(null));


        public static readonly DependencyPropertyKey ItemsShownPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ItemsShown), typeof(IEnumerable), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemsShownProperty = ItemsShownPropertyKey.DependencyProperty;


        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(Func<object, bool>), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty AutocompleteModeProperty =
            DependencyProperty.Register(nameof(AutocompleteMode), typeof(AutocompleteMode), typeof(AutocompleteTextBox), new PropertyMetadata(AutocompleteMode.Search));

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty TextBoxStyleProperty =
            DependencyProperty.Register(nameof(TextBoxStyle), typeof(Style), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty ScrollViewerStyleProperty =
            DependencyProperty.Register(nameof(ScrollViewerStyle), typeof(Style), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty BorderStyleProperty =
            DependencyProperty.Register(nameof(BorderStyle), typeof(Style), typeof(AutocompleteTextBox), new PropertyMetadata(null));

        public static readonly DependencyProperty NoItemMatchingControlProperty =
            DependencyProperty.Register(nameof(NoItemMatchingControl), typeof(UIElement), typeof(AutocompleteTextBox), new PropertyMetadata(null));
        #endregion


        private async void Autocomplete_SelectedByClick(object sender, EventArgs e)
        {
            await Task.Delay(50);
            ignoreOnce = true;
            TextBox.Focus();
        }

        private void Cerrar(object sender, MouseButtonEventArgs e)
        {
            bool DentroElemento(Point p, FrameworkElement element) =>
                p.X >= 0 && p.Y >= 0
                && p.X <= element.Width && p.Y <= element.Height;

            Point posicionRelativoAPopup = e.GetPosition(Popup.Child);
            Point posicionRelativoATextBox = e.GetPosition(this);

            if (!(DentroElemento(posicionRelativoAPopup, Popup) || DentroElemento(posicionRelativoATextBox, this)))
                Popup.IsOpen = false;
        }

        private bool ignoreOnce = false;

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (ignoreOnce)
                ignoreOnce = false;
            else
                ShowPopup();
        }

        private void TextBox_MouseDown(object sender, MouseButtonEventArgs e) =>
            ShowPopup();

        private void ShowPopup()
        {
            if (!Popup.IsOpen)
            {
                Popup.IsOpen = true;
            }
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Popup.IsOpen = false;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
                RollDown();
            if (e.Key == Key.Up)
                RollUp();
            if (e.Key == Key.Enter)
                Popup.IsOpen = false;
        }

        internal void RollDown() => ApplyOffset(1);

        internal void RollUp() => ApplyOffset(-1);

        private void ApplyOffset(int offset)
        {
            if (List.HasItems)
            {
                var current = List.SelectedIndex;
                var next = current + offset;

                if (next >= 0 && next < List.Items.Count)
                {
                    List.SelectedIndex = next;
                    List.ScrollIntoView(List.SelectedItem);
                }
                else if (current == -1)
                {
                    List.SelectedIndex = 0;
                    List.ScrollIntoView(List.SelectedIndex);
                }
            }
        }
    }
}

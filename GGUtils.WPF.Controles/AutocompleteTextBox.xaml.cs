using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GGUtils.WPF.Controles
{
    /// <summary>
    /// Lógica de interacción para AutocompleteTextBox.xaml
    /// </summary>
    public partial class AutocompleteTextBox : UserControl
    {
        public AutocompleteTextBox()
        {
            InitializeComponent();

            var ourWindow = Application.Current.MainWindow;
            Mouse.AddPreviewMouseDownHandler(ourWindow, Cerrar);

            Autocomplete.SelectedByClick += Autocomplete_SelectedByClick;
        }

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

            Console.WriteLine("\n" + GetHashCode());
            Console.WriteLine("Popup: " + posicionRelativoAPopup);
            Console.WriteLine("TextBox: " + posicionRelativoATextBox);

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
                Autocomplete.SelectFirst();
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
                Autocomplete.RollDown();
            if (e.Key == Key.Up)
                Autocomplete.RollUp();
        }
    }
}

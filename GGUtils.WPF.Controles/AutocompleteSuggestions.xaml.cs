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
    /// Lógica de interacción para AutocompletePopup.xaml
    /// </summary>
    public partial class AutocompleteSuggestions : UserControl
    {
        public AutocompleteSuggestions()
        {
            InitializeComponent();
        }

        public event EventHandler SelectedByClick;

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
            }
        }

        private void List_MouseDown(object sender, MouseButtonEventArgs e) =>
            SelectedByClick?.Invoke(null, EventArgs.Empty);

        internal void SelectFirst()
        {
            if (List.HasItems)
            {
                List.SelectedIndex = 0;
                List.ScrollIntoView(List.SelectedItem);
            }
        }
    }
}

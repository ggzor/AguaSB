using AguaSB.Views;
using System;
using System.Windows.Controls;

namespace AguaSB.Pagos.Views
{
    public partial class Pagar : UserControl, IView
    {
        public ViewModels.Pagar ViewModel { get; }

        public Pagar(ViewModels.Pagar pagar)
        {
            DataContext = ViewModel = pagar ?? throw new ArgumentNullException(nameof(pagar));
            InitializeComponent();
        }
    }
}

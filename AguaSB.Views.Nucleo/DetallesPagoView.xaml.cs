using AguaSB.Nucleo.Pagos;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AguaSB.Views.Nucleo
{
    public partial class DetallesPagoView : UserControl
    {
        public DetallesPagoView()
        {
            InitializeComponent();
        }

        public IEnumerable<IDetallePago> DetallesPago
        {
            get { return (IEnumerable<IDetallePago>)GetValue(DetallesPagoProperty); }
            set { SetValue(DetallesPagoProperty, value); }
        }

        public static readonly DependencyProperty DetallesPagoProperty =
            DependencyProperty.Register(nameof(DetallesPago), typeof(IEnumerable<IDetallePago>), typeof(DetallesPagoView), new PropertyMetadata(Enumerable.Empty<IDetallePago>()));
    }
}

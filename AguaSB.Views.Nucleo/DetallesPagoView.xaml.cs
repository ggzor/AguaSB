using AguaSB.Nucleo.Pagos;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

        public Brush ColorMontos
        {
            get { return (Brush)GetValue(ColorMontosProperty); }
            set { SetValue(ColorMontosProperty, value); }
        }

        public static readonly DependencyProperty ColorMontosProperty =
            DependencyProperty.Register(nameof(ColorMontos), typeof(Brush), typeof(DetallesPagoView), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50))));
    }
}

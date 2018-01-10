using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using AguaSB.Operaciones.Montos;

namespace AguaSB.Views.Operaciones
{
    public partial class DetallesPagoView : UserControl
    {
        public DetallesPagoView()
        {
            InitializeComponent();
        }

        public IEnumerable<IDetalleMonto> DetallesPago
        {
            get { return (IEnumerable<IDetalleMonto>)GetValue(DetallesPagoProperty); }
            set { SetValue(DetallesPagoProperty, value); }
        }

        public static readonly DependencyProperty DetallesPagoProperty =
            DependencyProperty.Register(nameof(DetallesPago), typeof(IEnumerable<IDetalleMonto>), typeof(DetallesPagoView), new PropertyMetadata(Enumerable.Empty<IDetalleMonto>()));

        public Brush ColorMontos
        {
            get { return (Brush)GetValue(ColorMontosProperty); }
            set { SetValue(ColorMontosProperty, value); }
        }

        public static readonly DependencyProperty ColorMontosProperty =
            DependencyProperty.Register(nameof(ColorMontos), typeof(Brush), typeof(DetallesPagoView), new PropertyMetadata(new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50))));
    }
}

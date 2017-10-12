using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using MoreLinq;

namespace AguaSB.Notificaciones
{
    public class PanelNotificaciones : UserControl
    {
        private Grid Contenido;

        public PanelNotificaciones()
        {
            Contenido = new Grid()
            {
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Content = Contenido;
        }

        private Queue<NotificacionView> notificaciones = new Queue<NotificacionView>();

        public void AgregarNotificacion(NotificacionView notificacion)
        {
            notificaciones.Enqueue(notificacion);
            IntentarAgregar();
        }

        private bool[] espaciosOcupados = new bool[1];
        private NotificacionView[] espacios = new NotificacionView[1];

        private void IntentarAgregar()
        {
            if (notificaciones.Any())
            {
                var espaciosVacios = espaciosOcupados.Index().Where(kv => kv.Value == false).Select(kv => kv.Key);

                if (espaciosVacios.Any())
                {
                    var espacioVacioIndice = espaciosVacios.First();

                    if (espacios[espacioVacioIndice] != null)
                        Contenido.Children.Remove(espacios[espacioVacioIndice]);

                    var notificacion = notificaciones.Dequeue();

                    notificacion.NotificacionCerrada += (src, args) => Desocupar(espacioVacioIndice);

                    Grid.SetRow(notificacion, espacioVacioIndice * 2);

                    Contenido.Children.Add(notificacion);
                    espaciosOcupados[espacioVacioIndice] = true;
                    espacios[espacioVacioIndice] = notificacion;
                }
            }
        }

        private void Desocupar(int indice)
        {
            espaciosOcupados[indice] = false;
            IntentarAgregar();
        }

        #region Cálculo de espacios
        /// <summary>
        /// No modificar los espacios en tiempo de ejecución.
        /// </summary>
        public int Espacios
        {
            get { return (int)GetValue(EspaciosProperty); }
            set { SetValue(EspaciosProperty, value); }
        }

        public static readonly DependencyProperty EspaciosProperty =
            DependencyProperty.Register(nameof(Espacios), typeof(int), typeof(PanelNotificaciones), new PropertyMetadata(1, ActualizarEspacios));

        private const int Separacion = 10;
        private static readonly GridLengthConverter converter = new GridLengthConverter();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
        private static void ActualizarEspacios(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var obj = d as PanelNotificaciones;
            var contenido = obj.Contenido;
            var espaciosNuevos = (int)e.NewValue;

            contenido.RowDefinitions.Clear();

            for (int i = 0; i < espaciosNuevos; i++)
            {
                contenido.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                contenido.RowDefinitions.Add(new RowDefinition() { Height = (GridLength)converter.ConvertFromString(Separacion.ToString()) });
            }
            contenido.Children.Clear();

            obj.espacios = new NotificacionView[espaciosNuevos];
            obj.espaciosOcupados = new bool[espaciosNuevos];
        }
        #endregion
    }
}

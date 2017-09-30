using System;
using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace AguaSB.Notificaciones
{
    public partial class NotificacionView : UserControl, IDisposable
    {

        #region Sonido
        private static SoundPlayer SonidoNotificacion { get; }

        static NotificacionView()
        {
            try
            {
                SonidoNotificacion = new SoundPlayer("Notificaciones/Notificacion.wav");
                SonidoNotificacion.Load();
            }
            catch
            {
                Console.WriteLine("No se cargó el sonido.");
            }
        }
        #endregion

        #region Propiedades
        public string Titulo
        {
            get { return (string)GetValue(TituloProperty); }
            set { SetValue(TituloProperty, value); }
        }

        public FrameworkElement Icono
        {
            get { return (FrameworkElement)GetValue(IconoProperty); }
            set { SetValue(IconoProperty, value); }
        }

        public string Contenido
        {
            get { return (string)GetValue(ContenidoProperty); }
            set { SetValue(ContenidoProperty, value); }
        }

        public string Clase
        {
            get { return (string)GetValue(ClaseProperty); }
            set { SetValue(ClaseProperty, value); }
        }

        public DateTime Fecha
        {
            get { return (DateTime)GetValue(FechaProperty); }
            set { SetValue(FechaProperty, value); }
        }

        public ICommand Command { get; set; }

        public event EventHandler NotificacionCerrada;
        #endregion


        public NotificacionView()
        {
            InitializeComponent();
            Loaded += (_, __) => SonidoNotificacion?.Play();
        }

        private void Cerrado(object sender, EventArgs e) => NotificacionCerrada?.Invoke(this, EventArgs.Empty);

        private bool CerradoPorBoton = false;

        private void Abierto(object sender, EventArgs e)
        {
            IniciarCerradura(this, null);
        }

        private void Cerrar(object sender, MouseButtonEventArgs e)
        {
            if (!UsandoCerradura)
                CancelarCerradura.Cancel();
            CerradoPorBoton = true;
        }

        private void EjecutarAccion(object sender, MouseButtonEventArgs e)
        {
            if (!UsandoCerradura)
                CancelarCerradura.Cancel();
            if (!CerradoPorBoton)
                Command?.Execute(null);
        }

        #region Manejo de animaciones
        private CancellationTokenSource CancelarCerradura = new CancellationTokenSource();
        private bool UsandoCerradura = false;
        private bool EsperandoCerradura = false;

        private static readonly TimeSpan TiempoEsperaCerradura = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan DuracionCerradura = TimeSpan.FromMilliseconds(300);
        private static readonly CircleEase Ease = new CircleEase();

        private async void IniciarCerradura(object sender, MouseEventArgs e)
        {
            if (!EsperandoCerradura)
            {
                try
                {
                    EsperandoCerradura = true;
                    await Task.Delay(TiempoEsperaCerradura, CancelarCerradura.Token);
                    UsandoCerradura = true;

                    DoubleAnimation animation = new DoubleAnimation(ActualWidth, DuracionCerradura)
                    {
                        EasingFunction = Ease
                    };
                    animation.Completed += Cerrado;
                    RenderTransform.BeginAnimation(TranslateTransform.XProperty, animation);
                }
                catch (TaskCanceledException)
                {
                    CancelarCerradura.Dispose();
                    CancelarCerradura = new CancellationTokenSource();
                }
                finally
                {
                    EsperandoCerradura = false;
                }
            }
        }

        private void DetenerCerradura(object sender, MouseEventArgs e)
        {
            CancelarCerradura.Cancel();
            CancelarCerradura.Dispose();
            CancelarCerradura = new CancellationTokenSource();
        }
        #endregion

        #region DP´s
        public static readonly DependencyProperty TituloProperty =
            DependencyProperty.Register(nameof(Titulo), typeof(string), typeof(NotificacionView), new PropertyMetadata(""));

        public static readonly DependencyProperty IconoProperty =
            DependencyProperty.Register(nameof(Icono), typeof(FrameworkElement), typeof(NotificacionView), new PropertyMetadata(null));


        public static readonly DependencyProperty ContenidoProperty =
            DependencyProperty.Register(nameof(Contenido), typeof(string), typeof(NotificacionView), new PropertyMetadata(""));


        public static readonly DependencyProperty ClaseProperty =
            DependencyProperty.Register(nameof(Clase), typeof(string), typeof(NotificacionView), new PropertyMetadata(""));

        public static readonly DependencyProperty FechaProperty =
            DependencyProperty.Register(nameof(Fecha), typeof(DateTime), typeof(NotificacionView), new PropertyMetadata(DateTime.Now));
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CancelarCerradura.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

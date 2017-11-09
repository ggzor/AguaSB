using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using AguaSB.Estilos;
using AguaSB.Nucleo;
using AguaSB.Utilerias;

namespace AguaSB.Contratos.Views.Controles
{
    public partial class DomicilioView : UserControl, INotifyPropertyChanged, INotifyDataErrorInfo, IEnfocable
    {
        public DomicilioView()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            InitializeComponent();
        }

        public void Enfocar() => CajaSecciones.Focus();

        private void DomicilioCambiado()
        {
            if (Domicilio != null)
            {
                if (Domicilio.Calle?.Seccion is Seccion seccion && Secciones.Contains(seccion))
                {
                    Seccion = seccion;
                    SeleccionarCallesDeSeccion(seccion);

                    if (Calles.Contains(Domicilio.Calle))
                        Calle = Domicilio.Calle;
                    else
                        SeleccionarPrimeraCalle();
                }
                else
                {
                    ActualizarCalles();
                }
            }
        }

        private void CallesCambiadas()
        {
            Secciones = CallesAgrupadas?.Keys.OrderBy(_ => _.Orden).ToList() ?? Enumerable.Empty<Seccion>();

            ActualizarCalles();
        }

        private void ActualizarCalles()
        {
            if (Secciones.FirstOrDefault() is Seccion seccion)
            {
                Seccion = seccion;

                SeleccionarCallesDeSeccion(seccion);
                SeleccionarPrimeraCalle();
            }
        }

        private void SeleccionarPrimeraCalle()
        {
            if (Calles.FirstOrDefault() is Calle calle)
                Calle = calle;
        }

        private void SeleccionarCallesDeSeccion(Seccion seccion) =>
            Calles = CallesAgrupadas[seccion].OrderBy(_ => _.Nombre).ToList();

        #region Props
        public Domicilio Domicilio
        {
            get { return (Domicilio)GetValue(DomicilioProperty); }
            set { SetValue(DomicilioProperty, value); }
        }

        public IEnumerable<Seccion> Secciones
        {
            get { return (IEnumerable<Seccion>)GetValue(SeccionesProperty); }
            private set { SetValue(SeccionesPropertyKey, value); }
        }

        public IEnumerable<Calle> Calles
        {
            get { return (IEnumerable<Calle>)GetValue(CallesProperty); }
            private set { SetValue(CallesPropertyKey, value); }
        }

        public IDictionary<Seccion, IList<Calle>> CallesAgrupadas
        {
            get { return (IDictionary<Seccion, IList<Calle>>)GetValue(CallesAgrupadasProperty); }
            set { SetValue(CallesAgrupadasProperty, value); }
        }

        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }

        private const string MensajeRequerido = "Seleccione un valor que aparezca en la lista";

        private Seccion seccion;

        [Required(ErrorMessage = MensajeRequerido)]
        public Seccion Seccion
        {
            get { return seccion; }
            set
            {
                N.Validate(ref seccion, value);
                if (value != null)
                {
                    SeleccionarCallesDeSeccion(seccion);
                    SeleccionarPrimeraCalle();
                }
            }
        }

        private Calle calle;

        [Required(ErrorMessage = MensajeRequerido)]
        public Calle Calle
        {
            get { return calle; }
            set
            {
                N.Validate(ref calle, value);
                Domicilio.Calle = calle;
            }
        }
        #endregion

        #region DP´s
        public static readonly DependencyProperty DomicilioProperty =
            DependencyProperty.Register(nameof(Domicilio), typeof(Domicilio), typeof(DomicilioView), new PropertyMetadata(null, DomicilioCambiado));

        private static void DomicilioCambiado(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((DomicilioView)d).DomicilioCambiado();

        private static readonly DependencyPropertyKey SeccionesPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Secciones), typeof(IEnumerable<Seccion>), typeof(DomicilioView), new PropertyMetadata(Enumerable.Empty<Seccion>()));

        public static readonly DependencyProperty SeccionesProperty = SeccionesPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey CallesPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Calles), typeof(IEnumerable<Calle>), typeof(DomicilioView), new PropertyMetadata(Enumerable.Empty<Calle>()));

        public static readonly DependencyProperty CallesProperty = CallesPropertyKey.DependencyProperty;

        public static readonly DependencyProperty CallesAgrupadasProperty =
            DependencyProperty.Register(nameof(CallesAgrupadas), typeof(IDictionary<Seccion, IList<Calle>>), typeof(DomicilioView), new PropertyMetadata(null, CallesCambiadas));

        private static void CallesCambiadas(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((DomicilioView)d).CallesCambiadas();

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(DomicilioView), new PropertyMetadata(true));
        #endregion

        #region PropertyChanged y DataErrorInfo
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public bool HasErrors => N.TieneErrores;
        public IEnumerable GetErrors(string propertyName) => N.Errores(propertyName);

        private readonly Lazy<Notificador> notificador;
        protected Notificador N => notificador.Value;
        #endregion
    }
}

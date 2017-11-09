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
    public partial class ContratoView : UserControl, INotifyPropertyChanged, INotifyDataErrorInfo, IEnfocable
    {
        public ContratoView()
        {
            notificador = new Lazy<Notificador>(() =>
                new Notificador(this,
                    (src, args) => PropertyChanged?.Invoke(src, args),
                    (src, args) => ErrorsChanged?.Invoke(src, args)));

            InitializeComponent();
        }

        public void Enfocar() => CajaTipoContrato.Focus();

        private void EstablecerContrato()
        {
            if (Contrato != null)
            {
                if (Contrato.TipoContrato is TipoContrato tipo && TiposContrato.Contains(tipo))
                    TipoContrato = tipo;
                else
                    TipoContrato = TiposContrato.FirstOrDefault();
            }
        }

        private void EstablecerTiposContrato() => TipoContrato = TiposContrato?.FirstOrDefault();

        private TipoContrato tipoContrato;

        [Required(ErrorMessage = "Debe seleccionar un tipo de contrato existente")]
        public TipoContrato TipoContrato
        {
            get { return tipoContrato; }
            set { N.Validate(ref tipoContrato, value); if (Contrato != null) Contrato.TipoContrato = value; }
        }

        public Contrato Contrato
        {
            get { return (Contrato)GetValue(ContratoProperty); }
            set { SetValue(ContratoProperty, value); }
        }

        public DateTime PagadoHasta
        {
            get { return (DateTime)GetValue(PagadoHastaProperty); }
            set { SetValue(PagadoHastaProperty, value); }
        }

        public static readonly DependencyProperty PagadoHastaProperty =
            DependencyProperty.Register(nameof(PagadoHasta), typeof(DateTime), typeof(ContratoView), new PropertyMetadata(Fecha.MesDe(DateTime.Today)));

        public static readonly DependencyProperty ContratoProperty =
            DependencyProperty.Register(nameof(Contrato), typeof(Contrato), typeof(ContratoView), new PropertyMetadata(null, EstablecerContrato));

        private static void EstablecerContrato(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ContratoView)d).EstablecerContrato();

        public IEnumerable<TipoContrato> TiposContrato
        {
            get { return (IEnumerable<TipoContrato>)GetValue(TiposContratoProperty); }
            set { SetValue(TiposContratoProperty, value); }
        }

        public static readonly DependencyProperty TiposContratoProperty =
            DependencyProperty.Register(nameof(TiposContrato), typeof(IEnumerable<TipoContrato>), typeof(ContratoView), new PropertyMetadata(Enumerable.Empty<TipoContrato>(), EstablecerTiposContratos));

        private static void EstablecerTiposContratos(DependencyObject d, DependencyPropertyChangedEventArgs e) => ((ContratoView)d).EstablecerTiposContrato();

        public IEnumerable<string> SugerenciasMedidasTomas
        {
            get { return (IEnumerable<string>)GetValue(SugerenciasTomasProperty); }
            set { SetValue(SugerenciasTomasProperty, value); }
        }

        public static readonly DependencyProperty SugerenciasTomasProperty =
            DependencyProperty.Register(nameof(SugerenciasMedidasTomas), typeof(IEnumerable<string>), typeof(ContratoView), new PropertyMetadata(Enumerable.Empty<string>()));

        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(ContratoView), new PropertyMetadata(true));

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

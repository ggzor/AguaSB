using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using AguaSB.Estilos;
using AguaSB.Nucleo;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class PersonaView : UserControl, IEnfocable
    {
        public PersonaView()
        {
            InitializeComponent();
        }

        public void Enfocar() => Nombre.Focus();

        #region Props
        public Persona Persona
        {
            get { return (Persona)GetValue(PersonaProperty); }
            set { SetValue(PersonaProperty, value); }
        }

        public IEnumerable<string> SugerenciasNombres
        {
            get { return (IEnumerable<string>)GetValue(SugerenciasNombresProperty); }
            set { SetValue(SugerenciasNombresProperty, value); }
        }

        public IEnumerable<string> SugerenciasApellidos
        {
            get { return (IEnumerable<string>)GetValue(SugerenciasApellidosProperty); }
            set { SetValue(SugerenciasApellidosProperty, value); }
        }

        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }
        #endregion

        #region DP´s
        public static readonly DependencyProperty PersonaProperty =
            DependencyProperty.Register(nameof(Persona), typeof(Persona), typeof(PersonaView), new PropertyMetadata(new Persona()));

        public static readonly DependencyProperty SugerenciasNombresProperty =
            DependencyProperty.Register(nameof(SugerenciasNombres), typeof(IEnumerable<string>), typeof(PersonaView), new PropertyMetadata(Enumerable.Empty<string>()));

        public static readonly DependencyProperty SugerenciasApellidosProperty =
            DependencyProperty.Register(nameof(SugerenciasApellidos), typeof(IEnumerable<string>), typeof(PersonaView), new PropertyMetadata(Enumerable.Empty<string>()));

        public static readonly DependencyProperty EditableProperty =
            DependencyProperty.Register(nameof(Editable), typeof(bool), typeof(PersonaView), new PropertyMetadata(true));
        #endregion
    }
}

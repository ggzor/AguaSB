﻿using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using AguaSB.Nucleo;

namespace AguaSB.Usuarios.Views.Controles
{
    public partial class PersonaView : UserControl
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
        #endregion

        #region DP´s
        public static readonly DependencyProperty PersonaProperty =
            DependencyProperty.Register(nameof(Persona), typeof(Persona), typeof(PersonaView), new PropertyMetadata(new Persona()));

        public static readonly DependencyProperty SugerenciasNombresProperty =
            DependencyProperty.Register(nameof(SugerenciasNombres), typeof(IEnumerable<string>), typeof(PersonaView), new PropertyMetadata(Enumerable.Empty<string>()));

        public static readonly DependencyProperty SugerenciasApellidosProperty =
            DependencyProperty.Register(nameof(SugerenciasApellidos), typeof(IEnumerable<string>), typeof(PersonaView), new PropertyMetadata(Enumerable.Empty<string>()));
        #endregion
    }
}

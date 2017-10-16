﻿using System;
using System.Windows.Controls;

using AguaSB.Views;
using AguaSB.Navegacion;

namespace AguaSB.Contratos.Views
{
    public partial class Agregar : UserControl, IView
    {
        public INavegador Navegador { get; }

        public ViewModels.Agregar ViewModel { get; }

        public Agregar(ViewModels.Agregar viewModel, INavegador navegador)
        {
            Navegador = navegador ?? throw new ArgumentNullException(nameof(navegador));
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            ViewModel.Enfocar += (_, __) => TipoContrato.Focus();
        }

        private void AgregarUsuario_Click(object sender, System.Windows.RoutedEventArgs e) => Navegador.Navegar("Usuarios/Agregar", null);

        private void BuscarUsuario_Click(object sender, System.Windows.RoutedEventArgs e) => Navegador.Navegar("Usuarios/Listado", null);
    }
}

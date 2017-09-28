﻿using System;
using System.Windows.Controls;

namespace AguaSB.Usuarios.Views
{
    public partial class Agregar : UserControl
    {
        public ViewModels.Agregar ViewModel { get; }

        public Agregar(ViewModels.Agregar viewModel)
        {
            ViewModel = viewModel ?? throw new ArgumentNullException(nameof(viewModel));

            InitializeComponent();

            Principal.DataContext = ViewModel;
        }
    }
}

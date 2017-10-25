using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using MahApps.Metro.IconPacks;

using AguaSB.Views;
using AguaSB.Views.Utilerias;

namespace AguaSB.Usuarios.Views
{
    public partial class ListadoAnterior : UserControl, IView
    {
        public ViewModels.Listado ViewModel { get; }

        public ListadoAnterior(ViewModels.Listado viewModel)
        {
            DataContext = ViewModel = viewModel ?? throw new ArgumentNullException(nameof(ViewModel));

            InitializeComponent();
        }


    }
}

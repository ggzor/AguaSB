﻿using System;
using System.Collections.Generic;
using System.Windows;

using AguaSB.Estilos;
using AguaSB.Extensiones;
using MahApps.Metro.IconPacks;

namespace AguaSB.Usuarios.Views
{
    public class DescriptorExtension : IExtension
    {
        public string Nombre => nameof(Usuarios);

        public string Version => "v1.0.0";

        public string Descripcion =>
            "Agregar, actualizar o inhabilitar usuarios de la base de datos. Así como ver estadísticas sobre los adeudos de los usuarios.";

        #region Operaciones
        public Agregar Agregar { get; set; }
        #endregion

        public IEnumerable<Operacion> Operaciones => new[]
        {
            new Operacion(nameof(Agregar), Agregar)
        };

        public Lazy<FrameworkElement> Icono { get; } = new Lazy<FrameworkElement>(() => new PackIconModern()
        {
            Width = 80,
            Height = 80,
            Kind = PackIconModernKind.People,
            Foreground = Colores.Azul.BrochaWPF
        });
    }
}

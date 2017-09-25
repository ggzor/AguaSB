﻿using AguaSB.Extensiones;
using GGUtils.MVVM.Async;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

namespace AguaSB
{
    public class MainWindowViewModel : Model
    {
        private static readonly string DirectorioExtensiones = Path.Combine(Directory.GetCurrentDirectory(), "Extensiones");
        private const string PatronExtensiones = @"AguaSB\.[A-Za-z]+\.Views\.dll";

        public AsyncProperty<IEnumerable<IExtension>> Extensiones { get; }

        public DelegateCommand EjecutarOperacionComando { get; }

        public IAdministradorViews Views { get; }

        public MainWindowViewModel(IAdministradorViews administradorViews)
        {
            Extensiones = new AsyncProperty<IEnumerable<IExtension>>(CargarExtensiones());
            EjecutarOperacionComando = new DelegateCommand(EjecutarOperacion);
            Views = administradorViews ?? throw new ArgumentNullException(nameof(administradorViews));
        }

        private Task<IEnumerable<IExtension>> CargarExtensiones() => Task.Run(() =>
        {
            var extensiones = UtileriasExtensiones.En(DirectorioExtensiones, s => Regex.IsMatch(s, PatronExtensiones));
            var extensionesCargadas = new List<IExtension>();

            foreach (var extension in extensiones)
                if (!extension.Extension.IsFaulted)
                    extensionesCargadas.Add(extension.Extension.Value);
                else
                    Console.WriteLine($"No se pudo cargar \"{extension.Archivo}\": {extension.Extension.Exception.Message}");

            return (IEnumerable<IExtension>)extensionesCargadas;
        });

        private void EjecutarOperacion(object parametro)
        {
            if (parametro is Operacion operacion)
            {
                Views.TraerAlFrente(operacion.Visualizacion.Value);
                operacion.Nodo.Entrar(null);
            }
        }
    }
}
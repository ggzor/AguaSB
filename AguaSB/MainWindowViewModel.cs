using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;

using AguaSB.Extensiones;

namespace AguaSB
{
    public class MainWindowViewModel : Model
    {
        #region Configuracion
        private static readonly string DirectorioExtensiones = Path.Combine(Directory.GetCurrentDirectory());
        private const string PatronExtensiones = @"AguaSB\.[A-Za-z]+\.Views\.dll";
        #endregion

        #region Propiedades
        public AsyncProperty<IEnumerable<IExtension>> Extensiones { get; }

        public IAdministradorViews Views { get; }
        #endregion

        #region Comandos
        public DelegateCommand EjecutarOperacionComando { get; }
        #endregion

        private ProveedorServicios ProveedorServicios { get; }

        public MainWindowViewModel(IAdministradorViews administradorViews)
        {
            Extensiones = new AsyncProperty<IEnumerable<IExtension>>(CargarExtensiones());
            Views = administradorViews ?? throw new ArgumentNullException(nameof(administradorViews));

            EjecutarOperacionComando = new DelegateCommand(EjecutarOperacion);

            ProveedorServicios = new ProveedorServicios();
        }

        private async Task<IEnumerable<IExtension>> CargarExtensiones()
        {
            var extensionesARegistrar = await Task.Run(() =>
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

            foreach (var extension in extensionesARegistrar)
                foreach (var operacion in extension.Operaciones)
                    await operacion.ViewModel.Nodo.Inicializar(ProveedorServicios);

            return extensionesARegistrar;
        }

        private void EjecutarOperacion(object parametro)
        {
            if (parametro is Operacion operacion)
            {
                Views.TraerAlFrente(operacion.View.Value);

                operacion.ViewModel.Nodo.Entrar(null);
            }
        }
    }
}

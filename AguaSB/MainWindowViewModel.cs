using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Waf.Foundation;

using GGUtils.MVVM.Async;
using AguaSB.Extensiones;
using AguaSB.Proveedores;

namespace AguaSB
{
    public class MainWindowViewModel : Model
    {
        #region Configuracion
        private static readonly string DirectorioExtensiones = Path.Combine(Directory.GetCurrentDirectory());
        private const string PatronExtensiones = @"AguaSB\.[A-Za-z]+\.Views\.dll";
        #endregion

        public AsyncProperty<IEnumerable<IExtension>> Extensiones { get; }


        #region Comandos
        public DelegateCommand EjecutarOperacionComando { get; }
        #endregion

        private ProveedorServicios ProveedorServicios { get; }

        public MainWindowViewModel()
        {
            Extensiones = new AsyncProperty<IEnumerable<IExtension>>(CargarExtensiones());


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
            

            return extensionesARegistrar;
        }
    }
}

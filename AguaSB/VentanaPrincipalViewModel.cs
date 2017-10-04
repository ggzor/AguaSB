using System;
using System.Collections.Generic;
using System.Waf.Applications;

using AguaSB.Extensiones;

namespace AguaSB
{
    public class VentanaPrincipalViewModel
    {
        public IEnumerable<IExtension> Extensiones { get; }

        public DelegateCommand EjecutarOperacionComando { get; }

        public VentanaPrincipalViewModel(IEnumerable<IExtension> extensiones)
        {
            Extensiones = extensiones ?? throw new ArgumentNullException(nameof(extensiones));
            EjecutarOperacionComando = new DelegateCommand(EjecutarOperacion);
        }

        private void EjecutarOperacion(object operacion)
        {
            Console.WriteLine(operacion);
        }
    }
}

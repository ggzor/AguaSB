using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AguaSB.Extensiones.Tests.LibreriaBase;

namespace AguaSB.Extensiones.Tests.Cliente
{
    public class ClienteInterfaz : IInterfaz1
    {
        public void HacerAlgo()
        {
            Console.WriteLine("Estoy solo en este modulo... Soy clienteinterfaz");
        }
    }
}

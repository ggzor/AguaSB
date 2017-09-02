using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class Navegador : INavegador
    {
        
        public INodo NodoPrincipal { get; }

        public async Task<bool> NavegarA(string direccion)
        {
            return true;
        }
    }
}

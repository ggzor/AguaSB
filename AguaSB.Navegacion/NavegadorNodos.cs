using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class NavegadorNodos<T> : Navegador
    {

        public INodo<T> NodoPrincipal { get; }

        public override async Task<bool> NavegarA(ColaNavegacion direccion)
        {
            return true;
        }
    }
}

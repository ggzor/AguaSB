using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AguaSB.Navegacion;

namespace AguaSB.Pagos.ViewModels
{
    public class Listado : IViewModel
    {
        public INodo Nodo { get; }

        public Listado()
        {
            Nodo = new Nodo { };
        }
    }
}

using ReactiveUI;

using AguaSB.ViewModels;
using AguaSB.Navegacion;

namespace AguaSB.Pagos.ViewModels
{
    public class Pagar : ReactiveObject, IViewModel
    {
        public INodo Nodo { get; }

        public Pagar()
        {
            Nodo = new Nodo { };
        }
    }
}

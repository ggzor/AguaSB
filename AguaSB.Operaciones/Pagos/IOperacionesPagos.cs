using AguaSB.Nucleo;
using System.Linq;

namespace AguaSB.Operaciones.Pagos
{
    public interface IOperacionesPagos : IAmbitoDependiente
    {
        IQueryable<Pago> Todos { get; }

        void Hacer(Pago pago);
        void Deshacer(Pago pago);
    }
}

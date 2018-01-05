using System.Linq;

using Mehdime.Entity;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Pagos.Entity
{
    public class OperacionesPagos : OperacionesEntity, IOperacionesPagos
    {
        public OperacionesPagos(IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public IQueryable<Pago> Todos => BaseDeDatos.Pagos;

        public void Hacer(Pago pago)
        {

        }

        public void Deshacer(Pago pago)
        {

        }
    }
}

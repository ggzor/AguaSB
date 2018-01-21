using System.Linq;

using Mehdime.Entity;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;
using AguaSB.Utilerias;

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
            var pagoNuevo = new Pago
            {
                CantidadPagada = pago.CantidadPagada,
                Contrato = BaseDeDatos.Contratos.Find(pago.Contrato.Id),
                Desde = pago.Desde,
                FechaPago = pago.FechaPago,
                FechaRegistro = Fecha.Ahora,
                Hasta = pago.Hasta,
                Monto = pago.Monto
            };

            BaseDeDatos.Pagos.Add(pagoNuevo);
        }

        public void Deshacer(Pago pago)
        {
            // TODO: Implementar
        }
    }
}

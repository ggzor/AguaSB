using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Pagos.Entity
{
    public class LocalizadorPagos : OperacionesEntity, ILocalizadorPagos
    {
        public LocalizadorPagos(Mehdime.Entity.IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public Pago UltimoDe(Contrato contrato) =>
            (from pago in BaseDeDatos.Pagos
             where pago.Contrato.Id == contrato.Id
             orderby pago.FechaPago descending
             select pago)
            .FirstOrDefault();
    }
}

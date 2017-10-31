using AguaSB.Datos;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Nucleo.Datos
{
    public static class Contratos
    {
        public static IDictionary<ClaseContrato, IList<TipoContrato>> TiposContratoAgrupados(IRepositorio<TipoContrato> tiposContratoRepo) =>
            (from contrato in tiposContratoRepo.Datos
             orderby contrato.Nombre
             group contrato by contrato.ClaseContrato into g
             orderby g.Key
             select g)
            .ToDictionary(g => g.Key, g => g.ToList() as IList<TipoContrato>);
    }
}

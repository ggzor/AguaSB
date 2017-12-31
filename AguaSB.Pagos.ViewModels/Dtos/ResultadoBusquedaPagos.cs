using AguaSB.Nucleo;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class ResultadoBusquedaPagos
    {
        public IEnumerable<Pago> Resultados { get; }

        public ResultadoBusquedaPagos(IEnumerable<Pago> resultados)
        {
            Resultados = resultados;
        }

        public bool? HayResultados => Resultados?.Any();
        public bool? NoHayResultados => !HayResultados;

        public int? Conteo => Resultados?.Count();
        public decimal? Total => Resultados?.Select(p => p.CantidadPagada).Sum();
    }
}

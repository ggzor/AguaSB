using System.Collections.Generic;

namespace AguaSB.Reportes
{
    public interface IEscritorTabla
    {
        RGB ColorEncabezado { get; set; }
        RGB ColorTextoEncabezado { get; set; }

        void EscribirEncabezado(IEnumerable<string> encabezado);
    }
}

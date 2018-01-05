using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public sealed class ResultadoBusquedaUsuariosConContrato
    {
        public static readonly ResultadoBusquedaUsuariosConContrato Vacio = new ResultadoBusquedaUsuariosConContrato();

        public IReadOnlyCollection<Usuario> Resultados { get; }
        public int Total { get; }
        public int CantidadOpciones { get; }

        public bool? HayResultados => Resultados?.Count > 0;
        public bool? NoHayResultados => Resultados?.Count == 0;

        public int CantidadOpcionesAdicionales => Math.Max(Total - CantidadOpciones, 0);
        public bool HayMasOpciones => CantidadOpcionesAdicionales > 0;

        public int CoincidentesSinContrato { get; }
        public bool HayCoincidentesSinContrato => CoincidentesSinContrato > 0 && !(HayMasOpciones == true);

        private ResultadoBusquedaUsuariosConContrato()
        {
        }

        public ResultadoBusquedaUsuariosConContrato(IQueryable<Usuario> todasLasCoincidencias, int cantidadOpciones)
        {
            if (todasLasCoincidencias == null)
                throw new ArgumentNullException(nameof(todasLasCoincidencias));

            if (cantidadOpciones < 1)
                throw new ArgumentOutOfRangeException(nameof(cantidadOpciones), "Debe ser mayor a 0.");

            CantidadOpciones = cantidadOpciones;

            Resultados = todasLasCoincidencias.Where(u => u.Contratos.Any()).Take(CantidadOpciones).ToArray();
            Total = todasLasCoincidencias.Count();

            if (Resultados.Count == 0)
                CoincidentesSinContrato = todasLasCoincidencias.Where(u => !u.Contratos.Any()).Count();
        }
    }
}

using AguaSB.Nucleo;
using AguaSB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class InformacionPagoContrato : Activable
    {
        public Contrato Contrato { get; set; }

        public ColumnaRangosPago[] Columnas { get; set; }

        public InformacionPagoContrato(Contrato contrato, IEnumerable<ColumnaRangosPago> columnas)
        {
            Contrato = contrato ?? throw new ArgumentNullException(nameof(contrato));

            if (columnas == null)
                throw new ArgumentNullException(nameof(columnas));

            Columnas = columnas.ToArray();

            if (Columnas.Length == 0)
                throw new ArgumentException("Debe haber al menos una columna.", nameof(columnas));
        }
    }
}

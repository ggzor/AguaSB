using System;
using System.Transactions;

using AguaSB.Legado.Nucleo;
using AguaSB.Servicios.Office;

namespace AguaSB.Legado
{
    internal sealed class EscrituraFechasPago : OperacionControlArchivo
    {
        public string Hoja { get; }
        public SolicitudPago Solicitud { get; }

        public EscrituraFechasPago(ControlArchivo controlArchivo, string hoja, SolicitudPago solicitud) : base(controlArchivo)
        {
            Hoja = hoja ?? throw new ArgumentNullException(nameof(hoja));
            Solicitud = solicitud ?? throw new ArgumentNullException(nameof(solicitud));
        }

        public override void Prepare(PreparingEnlistment preparingEnlistment)
        {
            lock (Lock)
            {
                var fila = new EditorFilaExcel(ControlArchivo.Archivo.Workbook.Worksheets[Hoja], Solicitud.Fila);

                fila.Establecer<DateTime>(10, Solicitud.FechaPago);
                fila.Establecer<DateTime>(12, Solicitud.MesFin);

                preparingEnlistment.Prepared();
            }
        }
    }
}

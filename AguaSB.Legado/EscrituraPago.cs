using System;
using System.Transactions;

using AguaSB.Legado.Nucleo;
using AguaSB.Servicios.Office;

namespace AguaSB.Legado
{
    internal sealed class EscrituraPago : OperacionControlArchivo
    {
        public string Hoja { get; }
        public int Fila { get; }
        public int NumeroUsuario { get; }
        public SolicitudPago Solicitud { get; }

        public EscrituraPago(ControlArchivo controlArchivo, string hoja, int fila, int numeroUsuario, SolicitudPago solicitudPago) : base(controlArchivo)
        {
            Hoja = hoja ?? throw new ArgumentNullException(nameof(hoja));
            Fila = fila;
            NumeroUsuario = numeroUsuario;
            Solicitud = solicitudPago ?? throw new ArgumentNullException(nameof(solicitudPago));
        }


        public override void Prepare(PreparingEnlistment preparingEnlistment)
        {
            lock (Lock)
            {
                var fila = new EditorFilaExcel(ControlArchivo.Archivo.Workbook.Worksheets[Hoja], Fila);

                fila.Establecer(3, Solicitud.FechaPago);
                fila.Establecer(4, NumeroUsuario);
                fila.Establecer(10, Solicitud.Meses);
                fila.Establecer(13, Solicitud.CantidadMeses);
                fila.Establecer(14, Solicitud.Monto);

                preparingEnlistment.Prepared();
            }
        }
    }
}

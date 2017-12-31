using AguaSB.Legado.Nucleo;
using AguaSB.Servicios.Office;
using OfficeOpenXml;
using System;
using System.IO;
using System.Linq;

namespace AguaSB.Legado
{
    public class EjecutorSolicitudPago
    {
        public FileInfo Archivo { get; }

        public EjecutorSolicitudPago(FileInfo archivo) =>
            Archivo = archivo ?? throw new ArgumentNullException(nameof(archivo));

        private const string NombreHojaUsuarios = "CONCENTRADO";
        private const string NombreHojaPagos = "PAGOS 2018";

        public Pago Ejecutar(SolicitudPago solicitudPago)
        {
            var resultado = new Pago { FechaPago = solicitudPago.FechaPago };

            using (var documento = new ExcelPackage(Archivo))
            {
                var hojaUsuarios = documento.Workbook.Worksheets[NombreHojaUsuarios];
                var hojaPagos = documento.Workbook.Worksheets[NombreHojaPagos];

                ProcesarHojaUsuarios(solicitudPago, resultado, hojaUsuarios);
                ProcesarHojaPagos(solicitudPago, resultado, hojaPagos);

                documento.Save();
            }

            return resultado;
        }

        private void ProcesarHojaUsuarios(SolicitudPago solicitudPago, Pago resultado, ExcelWorksheet hojaUsuarios)
        {
            var filaUsuario = new EditorFilaExcel(hojaUsuarios, solicitudPago.Fila);

            resultado.NumeroUsuario = filaUsuario.Obtener<int>(2);
            resultado.Contrato = filaUsuario.Obtener<string>(3);
            resultado.NombreUsuario = filaUsuario.Obtener<string>(6);
            resultado.Seccion = filaUsuario.Obtener<string>(7);
            resultado.Domicilio = filaUsuario.Obtener<string>(8);
            resultado.TipoContrato = filaUsuario.Obtener<string>(11);

            filaUsuario.Establecer<DateTime>(10, solicitudPago.FechaPago);
            filaUsuario.Establecer<DateTime>(12, solicitudPago.MesFin);

            resultado.Cantidad = solicitudPago.Monto;
            resultado.Meses = solicitudPago.Meses;
        }

        private const int InicioPagos = 2;
        private const int ConteoPagos = 3000;

        private void ProcesarHojaPagos(SolicitudPago solicitudPago, Pago resultado, ExcelWorksheet hojaPagos)
        {
            int ObtenerSiguienteFilaPagos() => Enumerable.Range(InicioPagos, ConteoPagos)
                .Select(i => (Indice: i, Valor: hojaPagos.Cells[i, 4].GetValue<int?>()))
                .First(i => i.Valor == null)
                .Indice;

            var filaPagos = new EditorFilaExcel(hojaPagos, ObtenerSiguienteFilaPagos());

            resultado.Folio = filaPagos.Obtener<int>(1);
            filaPagos.Establecer(1, filaPagos.Fila - 1);
            filaPagos.Establecer(2, filaPagos.Fila - 1);
            filaPagos.Establecer(3, solicitudPago.FechaPago);
            filaPagos.Establecer(4, resultado.NumeroUsuario);
            filaPagos.Establecer(10, resultado.Meses);
            filaPagos.Establecer(13, solicitudPago.CantidadMeses);
        }
    }
}

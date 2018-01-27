using System;
using System.IO;
using System.Transactions;

using OfficeOpenXml;

using AguaSB.Legado.Nucleo;
using AguaSB.Nucleo.Utilerias;

namespace AguaSB.Legado
{
    public class EjecutorSolicitudPago
    {
        public FileInfo Archivo { get; }

        public EjecutorSolicitudPago(FileInfo archivo) =>
            Archivo = archivo ?? throw new ArgumentNullException(nameof(archivo));

        private readonly Guid Id = Guid.NewGuid();
        private const string NombreHojaUsuarios = "CONCENTRADO";
        private const string NombreHojaPagos = "PAGOS 2018";
        private const int InicioBusquedaFolios = 900;

        public Pago Ejecutar(SolicitudPago solicitudPago)
        {
            if (Transaction.Current is Transaction t)
            {
                var control = new ControlArchivo(Archivo);
                var hojas = control.Archivo.Workbook.Worksheets;

                var (fila, folio) = LecturaFolioPago.Obtener(hojas[NombreHojaPagos], InicioBusquedaFolios, 4, 2);

                var resultado = new Pago
                {
                    Folio = folio,
                    FechaPago = solicitudPago.FechaPago,
                    Cantidad = solicitudPago.Monto,
                    CantidadLetra = CantidadALetra.Convertir(solicitudPago.Monto),
                    Meses = solicitudPago.Meses,
                    Usuario = LecturaUsuario.Leer(hojas[NombreHojaUsuarios], solicitudPago.Fila)
                };

                t.EnlistDurable(Guid.NewGuid(), new EscrituraFechasPago(control, NombreHojaUsuarios, solicitudPago), EnlistmentOptions.None);
                t.EnlistDurable(Guid.NewGuid(), new EscrituraPago(control, NombreHojaPagos, fila, resultado.Usuario.Numero, solicitudPago), EnlistmentOptions.None);

                return resultado;
            }
            else
            {
                throw new InvalidOperationException("Esta operacion debe ejecutarse bajo una transacción.");
            }
        }
    }
}

using System;
using System.Linq;

using AguaSB.Datos;
using AguaSB.Legado.Nucleo;
using AguaSB.Servicios;

namespace AguaSB.Legado
{
    public class TransformadorPago : IInformador<AguaSB.Nucleo.Pago>
    {
        public IInformador<Pago> InformadorPagos { get; }
        public IRepositorio<AguaSB.Nucleo.TipoNota> TiposNotaRepo { get; }
        public EjecutorSolicitudPago EjecutorSolicitud { get; set; }

        public TransformadorPago(IInformador<Pago> informadorPagos, IRepositorio<AguaSB.Nucleo.TipoNota> tiposNotaRepo, EjecutorSolicitudPago ejecutorSolicitud)
        {
            InformadorPagos = informadorPagos ?? throw new ArgumentNullException(nameof(informadorPagos));
            TiposNotaRepo = tiposNotaRepo ?? throw new ArgumentNullException(nameof(tiposNotaRepo));
            EjecutorSolicitud = ejecutorSolicitud ?? throw new ArgumentNullException(nameof(ejecutorSolicitud));
        }

        public void Informar(AguaSB.Nucleo.Pago informacion)
        {
            int idContrato = informacion.Contrato.Id;
            var nota = (from tipoNota in TiposNotaRepo.Datos
                        where tipoNota.Nombre == "_Contrato_Fila"
                        from n in tipoNota.Notas
                        where n.Referencia == idContrato
                        select n)
                        .SingleOrDefault();

            if (nota == null)
            {
                //TODO: Log
                Console.WriteLine($"No se encontró la nota para el contrato {idContrato}");
                return;
            }

            if (int.TryParse(nota.Informacion, out int fila))
            {
                var solicitudPago = new SolicitudPago
                {
                    Fila = fila,
                    FechaPago = informacion.FechaPago,
                    MesInicio = informacion.Desde,
                    MesFin = informacion.Hasta,
                    Monto = (int)informacion.CantidadPagada
                };

                var resultadoSolicitud = EjecutorSolicitud.Ejecutar(solicitudPago);

                InformadorPagos.Informar(resultadoSolicitud);
            }
            else
            {
                //TODO: Log
                Console.WriteLine($"La información establecida para la fila {nota.Id} es invalida: {nota.Informacion}");
            }
        }
    }
}

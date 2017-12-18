using AguaSB.Nucleo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public sealed class OpcionesPago
    {
        public PagoPorRangos PagoPorRangos { get; }

        public PagoPorPropiedades PagoPorPropiedades { get; set; }

        public OpcionesPago(Usuario usuario, IReadOnlyCollection<InformacionContrato> contratos, Tarifa[] tarifas)
        {
            PagoPorRangos = new PagoPorRangos(usuario, contratos, tarifas);
            PagoPorPropiedades = new PagoPorPropiedades(usuario, contratos, tarifas);
        }


        public static OpcionesPago Para(Usuario usuario, Tarifa[] tarifas)
        {
            var solicitud = from Contrato in usuario.Contratos
                            let TipoContrato = Contrato.TipoContrato
                            let DatosContrato = new { Contrato, TipoContrato }
                            let Domicilio = Contrato.Domicilio
                            let DatosDomicilio = new { Domicilio, Domicilio.Calle, Domicilio.Calle.Seccion }
                            let Pagos = Contrato.Pagos.OrderBy(p => p.FechaPago)
                            orderby DatosDomicilio.Seccion.Orden, DatosDomicilio.Calle.Nombre, Domicilio.Numero
                            select new { DatosContrato, DatosDomicilio, Pagos };

            var informacionContratos = solicitud.ToArray().Select(datos =>
            {
                var contrato = datos.DatosContrato.Contrato;
                contrato.TipoContrato = datos.DatosContrato.TipoContrato;

                var domicilio = datos.DatosDomicilio.Domicilio;
                domicilio.Calle = datos.DatosDomicilio.Calle;
                domicilio.Calle.Seccion = datos.DatosDomicilio.Seccion;

                contrato.Pagos = datos.Pagos.ToArray();

                return new InformacionContrato
                {
                    Contrato = contrato,
                    Adeudo = Adeudos.Calcular(contrato.Pagos.Last().Hasta, contrato.TipoContrato, tarifas),
                    UltimoPago = contrato.Pagos.Last()
                };
            }).ToArray();

            return new OpcionesPago(usuario, informacionContratos, tarifas);
        }
    }
}

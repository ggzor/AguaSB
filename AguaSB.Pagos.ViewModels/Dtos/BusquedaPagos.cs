using System;
using System.Collections.Generic;
using System.Linq;
using AguaSB.Nucleo;
using AguaSB.Utilerias;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class BusquedaPagos : Notificante
    {
        private bool buscando;
        private string error;

        public bool Buscando
        {
            get { return buscando; }
            set { N.Set(ref buscando, value); }
        }

        public bool TieneErrores => !string.IsNullOrWhiteSpace(error);

        public string Error
        {
            get { return error; }
            set { N.Set(ref error, value); }
        }

        public ResultadoBusquedaPagos Ejecutar(IQueryable<Pago> datos)
        {
            try
            {
                Buscando = true;

                return new ResultadoBusquedaPagos(Buscar(datos));
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                return new ResultadoBusquedaPagos(null);
            }
            finally
            {
                Buscando = false;
            }
        }

        private IEnumerable<Pago> Buscar(IQueryable<Pago> datos)
        {
            var esteMes = ((Fecha.EsteMes.Month - 1) * 12) + Fecha.EsteMes.Year;
            var solicitud = from pago in datos
                            let fechaPago = pago.FechaPago
                            where ((fechaPago.Month - 1) * 12) + fechaPago.Year == esteMes
                            where pago.Monto > 0 && pago.CantidadPagada > 0
                            orderby fechaPago
                            select pago;

            var valores = from Pago in solicitud
                          let Contrato = Pago.Contrato
                          let Domicilio = Contrato.Domicilio
                          let DatosDomicilio = new { Domicilio, Domicilio.Calle, Domicilio.Calle.Seccion }
                          let DatosContrato = new { Contrato, Contrato.Usuario, Contrato.TipoContrato, DatosDomicilio }
                          select new { Pago, DatosContrato };

            return valores.ToArray().Select(p =>
            {
                var pago = p.Pago;

                pago.Contrato = p.DatosContrato.Contrato;

                pago.Contrato.Usuario = p.DatosContrato.Usuario;

                pago.Contrato.Domicilio = p.DatosContrato.DatosDomicilio.Domicilio;
                pago.Contrato.Domicilio.Calle = p.DatosContrato.DatosDomicilio.Calle;
                pago.Contrato.Domicilio.Calle.Seccion = p.DatosContrato.DatosDomicilio.Seccion;

                pago.Contrato.TipoContrato = p.DatosContrato.TipoContrato;

                return p.Pago;
            }).ToArray();
        }
    }
}

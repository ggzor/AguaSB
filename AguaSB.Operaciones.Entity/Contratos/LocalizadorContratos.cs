using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Contratos.Entity
{
    public class LocalizadorContratos : OperacionesEntity, ILocalizadorContratos
    {
        public LocalizadorContratos(Mehdime.Entity.IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public IReadOnlyCollection<Contrato> ObtenerContratos(Usuario usuario)
        {
            var solicitud = from Contrato in BaseDeDatos.Contratos
                            where Contrato.Usuario.Id == usuario.Id
                            let Domicilio = Contrato.Domicilio
                            let DatosDomicilio = new { Domicilio, Domicilio.Calle, Domicilio.Calle.Seccion }
                            let TipoContrato = Contrato.TipoContrato
                            select new { Contrato, DatosDomicilio, TipoContrato };
            
            return solicitud.ToArray().Select(datos =>
            {
                var contrato = datos.Contrato;

                var domicilio = datos.DatosDomicilio.Domicilio;
                domicilio.Calle = datos.DatosDomicilio.Calle;
                domicilio.Calle.Seccion = datos.DatosDomicilio.Seccion;

                contrato.Domicilio = domicilio;
                contrato.TipoContrato = datos.TipoContrato;

                contrato.Usuario = usuario;

                return contrato;
            }).ToArray();
        }
    }
}

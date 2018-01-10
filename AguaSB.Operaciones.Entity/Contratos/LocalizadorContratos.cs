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
            var solicitud = from contrato in BaseDeDatos.Contratos
                                .Include(nameof(Contrato.TipoContrato))
                                .Include($"{nameof(Contrato.Domicilio)}.{nameof(Domicilio.Calle)}.{nameof(Calle.Seccion)}")
                            where contrato.Usuario.Id == usuario.Id
                            let domicilio = contrato.Domicilio
                            orderby domicilio.Calle.Seccion.Orden, domicilio.Calle.Nombre, domicilio.Numero
                            select contrato;

            return solicitud.ToArray();
        }
    }
}

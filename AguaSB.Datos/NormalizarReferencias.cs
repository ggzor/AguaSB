using AguaSB.Nucleo;
using System.Linq;

namespace AguaSB.Datos
{
    public static class NormalizarReferencias
    {
        public static void Contrato(Contrato contrato, IRepositorio<Usuario> usuarios, IRepositorio<TipoContrato> tiposContrato, IRepositorio<Calle> calles)
        {
            var usuarioId = contrato.Usuario.Id;
            var usuario = usuarios.Datos.Single(_ => _.Id == usuarioId);
            usuario.Contratos.Add(contrato);
            contrato.Usuario = usuario;

            var tipoContratoId = contrato.TipoContrato.Id;
            contrato.TipoContrato = tiposContrato.Datos.Single(_ => _.Id == tipoContratoId);

            Domicilio(contrato.Domicilio, calles);
        }

        public static void Domicilio(Domicilio domicilio, IRepositorio<Calle> calles)
        {
            var calleId = domicilio.Calle.Id;
            domicilio.Calle = calles.Datos.Single(_ => _.Id == calleId);
        }
    }
}

using System.Collections.Generic;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Contratos
{
    public interface ILocalizadorContratos
    {
        IReadOnlyCollection<Contrato> ObtenerContratos(Usuario usuario);
    }
}

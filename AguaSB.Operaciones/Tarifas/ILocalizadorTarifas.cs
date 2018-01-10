using System.Collections.Generic;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Tarifas
{
    public interface ILocalizadorTarifas
    {
        IReadOnlyCollection<Tarifa> Obtener();
    }
}

using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Tarifas.Entity
{
    public class LocalizadorTarifas : OperacionesEntity, ILocalizadorTarifas
    {
        public LocalizadorTarifas(Mehdime.Entity.IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public IReadOnlyCollection<Tarifa> Tarifas { get; set; }

        public IReadOnlyCollection<Tarifa> Obtener()
        {
            if (Tarifas == null)
                Tarifas = BaseDeDatos.Tarifas.OrderBy(t => t.Inicio).ToArray();

            return Tarifas;
        }
    }
}

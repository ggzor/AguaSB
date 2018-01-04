using System;
using Mehdime.Entity;

namespace AguaSB.Operaciones.Entity.Ambitos
{
    public class AmbitoSoloLectura : IAmbitoSoloLectura
    {
        private IDbContextReadOnlyScope AmbitoBaseDeDatos { get; }

        public AmbitoSoloLectura(IDbContextReadOnlyScope ambitoBaseDeDatos)
        {
            AmbitoBaseDeDatos = ambitoBaseDeDatos ?? throw new ArgumentNullException(nameof(ambitoBaseDeDatos));
        }

        public void Dispose() => AmbitoBaseDeDatos.Dispose();
    }
}

using System;
using Mehdime.Entity;

namespace AguaSB.Operaciones.Entity.Ambitos
{
    public class ProveedorAmbito : IProveedorAmbito
    {
        public IDbContextScopeFactory Ambito { get; }

        public ProveedorAmbito(IDbContextScopeFactory ambito)
        {
            Ambito = ambito ?? throw new ArgumentNullException(nameof(ambito));
        }

        public IAmbito Crear() => new Ambito(Ambito.Create());

        public IAmbitoSoloLectura CrearSoloLectura() => new AmbitoSoloLectura(Ambito.CreateReadOnly());
    }
}

using Mehdime.Entity;

namespace AguaSB.Operaciones.Entity.Ambitos
{
    public class Ambito : IAmbito
    {
        private IDbContextScope AmbitoBaseDeDatos { get; }

        public Ambito(IDbContextScope ambitoBaseDeDatos)
        {
            AmbitoBaseDeDatos = ambitoBaseDeDatos;
        }

        public void GuardarCambios() => AmbitoBaseDeDatos.SaveChanges();
        public void Dispose() => AmbitoBaseDeDatos.Dispose();
    }
}


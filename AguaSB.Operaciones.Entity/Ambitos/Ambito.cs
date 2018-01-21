using Mehdime.Entity;
using System.Transactions;

namespace AguaSB.Operaciones.Entity.Ambitos
{
    public class Ambito : IAmbito
    {
        private IDbContextScope AmbitoBaseDeDatos { get; }
        private TransactionScope AmbitoTransaccion { get; }

        public Ambito(IDbContextScope ambitoBaseDeDatos, bool crearTransaccion = false)
        {
            AmbitoBaseDeDatos = ambitoBaseDeDatos;

            if (crearTransaccion)
                AmbitoTransaccion = new TransactionScope(TransactionScopeAsyncFlowOption.Suppress);
        }

        public void GuardarCambios()
        {
            AmbitoBaseDeDatos.SaveChanges();
            AmbitoTransaccion.Complete();
        }

        public void Dispose()
        {
            try
            {
                AmbitoTransaccion.Dispose();
            }
            finally
            {
                AmbitoBaseDeDatos.Dispose();
            }
        }
    }
}

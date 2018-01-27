using Mehdime.Entity;
using System.Transactions;

namespace AguaSB.Operaciones.Entity.Ambitos
{
    public class AmbitoConTransaccion : IAmbito
    {
        private IDbContextScope AmbitoBaseDeDatos { get; }
        private TransactionScope AmbitoTransaccion { get; }

        public AmbitoConTransaccion(IDbContextScope ambitoBaseDeDatos)
        {
            AmbitoBaseDeDatos = ambitoBaseDeDatos;
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

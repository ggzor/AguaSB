using System;
using System.Transactions;

namespace AguaSB.Legado
{
    internal abstract class OperacionControlArchivo : IEnlistmentNotification
    {
        public ControlArchivo ControlArchivo { get; }
        protected readonly object Lock = new object(); // TODO: Verificar como eliminar esto

        public OperacionControlArchivo(ControlArchivo controlArchivo) =>
            ControlArchivo = controlArchivo ?? throw new ArgumentNullException(nameof(controlArchivo));

        public abstract void Prepare(PreparingEnlistment preparingEnlistment);

        public void Commit(Enlistment enlistment)
        {
            ControlArchivo.Guardar();
            enlistment.Done();
        }

        public void Rollback(Enlistment enlistment)
        {
            ControlArchivo.NoGuardar();
            enlistment.Done();
        }

        public virtual void InDoubt(Enlistment enlistment) { }
    }
}

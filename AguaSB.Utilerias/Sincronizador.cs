using System;
using System.Threading;

namespace AguaSB.Utilerias
{
    /// <summary>
    /// Esta clase encapsula la funcionalidad para realizar multiples tareas que modifican un estado compartido asincronamente, 
    /// conservando solo el estado de la ultima.
    /// </summary>
    public sealed class Sincronizador
    {
        private int idActual = int.MinValue;

        public int ObtenerId() => Interlocked.Increment(ref idActual);

        private readonly object token = new object();

        public void Intentar(int id, Action accion)
        {
            if (idActual == id)
            {
                lock (token)
                    accion();
            }
        }
    }
}

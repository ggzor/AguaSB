using System;
using System.Threading;
using System.Threading.Tasks;

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
                {
                    if (idActual == id)
                    {
                        accion();
                    }
                }
            }
        }

        public Task IntentarAsync(int id, Func<Task> tarea)
        {
            if (idActual == id)
            {
                lock (token)
                {
                    if (idActual == id)
                    {
                        return tarea();
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}

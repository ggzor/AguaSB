using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    /// <summary>
    /// Clase que maneja un nodo sin hijos, usando composición.
    /// </summary>
    public sealed class NodoHoja<T> : NodoBase<T>
    {
        /// <summary>
        /// Función que habilita <see cref="INodo.Entrar"/> usando composición.
        /// </summary>
        public Func<ColaNavegacion, Task> Entrada { get; set; }

        public override async Task Entrar(ColaNavegacion informacion)
        {
            if (Entrada != null)
                await Entrada(informacion);
        }

    }
}

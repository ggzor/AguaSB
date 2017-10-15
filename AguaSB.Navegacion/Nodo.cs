using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    /// <summary>
    /// Clase que maneja un nodo sin hijos, usando composición.
    /// </summary>
    public sealed class Nodo : INodo
    {
        /// <summary>
        /// Función llamada cuando se ingresa por primera vez a este nodo. 
        /// Precede a la primera llamada a <see cref="Entrada"/>
        /// </summary>
        public Func<Task> PrimeraEntrada { get; set; }

        /// <summary>
        /// Función que habilita <see cref="INodo.Entrar"/> usando composición.
        /// </summary>
        public Func<object, Task> Entrada { get; set; }

        private bool primeraLlamada = true;

        public async Task Entrar(object informacion)
        {
            if (primeraLlamada && PrimeraEntrada != null)
            {
                await PrimeraEntrada();

                primeraLlamada = false;
            }

            if (Entrada != null)
                await Entrada(informacion);
        }
    }
}

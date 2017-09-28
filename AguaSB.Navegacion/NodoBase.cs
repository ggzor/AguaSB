using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{

    /// <summary>
    /// Ofrece la funcionalidad <see cref="Inicializacion"/> y <see cref="Finalizacion"/> para las subclases <see cref="INodo"/>.
    /// </summary>
    public abstract class NodoBase<T> : INodo<T>
    {
        public Navegador Navegador { get; set; }

        /// <summary>
        /// Función llamada exactamente una vez en la primera entrada.
        /// </summary>
        public virtual Func<T, Task> Inicializacion { get; set; }

        /// <summary>
        /// Funcion llamada exactamente una vez cuando el navegador está finalizando. 
        /// Función que habilita <see cref="INodo.Finalizar"/> usando composición.
        /// </summary>
        public virtual Func<Task> Finalizacion { get; set; }


        public virtual async Task Inicializar(T parametro)
        {
            if (Inicializacion != null)
                await Inicializacion(parametro);
        }

        public abstract Task Entrar(ColaNavegacion colaNavegacion);

        public virtual async Task Finalizar()
        {
            if (Finalizacion != null)
                await Finalizacion();
        }
    }
}

using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{

    /// <summary>
    /// Ofrece la funcionalidad <see cref="Inicializacion"/> y <see cref="Finalizacion"/> para las subclases <see cref="INodo"/>.
    /// </summary>
    public abstract class NodoBase : INodo
    {

        public Navegador Navegador { get; set; }

        /// <summary>
        /// Función llamada exactamente una vez en la primera entrada. Precede al primer <see cref="INodo.Entrar"/>
        /// </summary>
        public virtual Func<Task> Inicializacion { get; set; }

        /// <summary>
        /// Funcion llamada exactamente una vez cuando el navegador está finalizando. 
        /// Función que habilita <see cref="INodo.Finalizar"/> usando composición.
        /// </summary>
        public virtual Func<Task> Finalizacion { get; set; }


        private bool primeraVez = true;

        public virtual async Task Entrar(ColaNavegacion informacion)
        {
            if (primeraVez)
            {
                if (Inicializacion != null)
                    await Inicializacion();

                primeraVez = false;
            }
        }

        public virtual async Task Finalizar()
        {
            if (Finalizacion != null)
                await Finalizacion();
        }

    }
}

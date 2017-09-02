﻿using System;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{

    /// <summary>
    /// Ofrece la funcionalidad <see cref="Inicializacion"/> y <see cref="Finalizacion"/> para las subclases <see cref="INodo"/>.
    /// </summary>
    public abstract class NodoBase : INodo
    {

        public INavegador Navegador { get; set; }

        /// <summary>
        /// Función llamada exactamente una vez en la primera entrada. Precede al primer <see cref="INodo.Entrar"/>
        /// </summary>
        public Func<Task> Inicializacion { get; set; }

        /// <summary>
        /// Funcion llamada exactamente una vez cuando el navegador está finalizando. 
        /// Función que habilita <see cref="INodo.Finalizar"/> usando composición.
        /// </summary>
        public Func<Task> Finalizacion { get; set; }


        private bool primeraVez = false;

        public virtual async Task Entrar(ColaNavegacion informacion)
        {
            if (primeraVez)
            {
                await Inicializacion?.Invoke();
                primeraVez = false;
            }
        }

        public virtual async Task Finalizar() => await Finalizacion?.Invoke();
    }
}

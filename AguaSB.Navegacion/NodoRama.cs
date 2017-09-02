﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AguaSB.Navegacion.Nodos
{
    /// <summary>
    /// Clase que representa un nodo que puede tener múltiples hijos. 
    /// Esta clase manejará las entradas y finalizaciones de los subnodos en automático.
    /// </summary>
    public class NodoRama : NodoBase
    {

        public string NodoActual { get; private set; }

        /// <summary>
        /// Llamado cuando se realiza un cambio en el nodo actual.
        /// </summary>
        public Func<string, Task> SeleccionSubnodo { get; set; }


        public IReadOnlyDictionary<string, INodo> Subnodos { get; private set; }

        public NodoRama(IReadOnlyDictionary<string, INodo> subnodos) =>
            Subnodos = subnodos ?? throw new ArgumentNullException(nameof(subnodos));


        public override async Task Entrar(ColaNavegacion informacion)
        {
            await base.Entrar(informacion);

            var nuevoNodoClave = informacion.Siguiente<string>();

            if (Subnodos.ContainsKey(nuevoNodoClave))
            {
                if (SeleccionSubnodo != null)
                    await SeleccionSubnodo(nuevoNodoClave);

                NodoActual = nuevoNodoClave;

                var nuevoNodo = Subnodos[nuevoNodoClave];
                nuevoNodo.Navegador = Navegador;

                await nuevoNodo.Entrar(informacion);
            }
        }

        public override async Task Finalizar()
        {
            foreach (var nodo in Subnodos.Values)
                await nodo.Finalizar();

            await base.Finalizar();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Navegacion
{
    public class ColaNavegacion
    {

        private Queue<object> Cola;

        public bool TieneInformacion => Cola.Any();

        public ColaNavegacion(params object[] parametros)
        {
            Cola = new Queue<object>(parametros);
        }

        public T Siguiente<T>() => Tomar<T>(Cola.Dequeue);

        public T Punta<T>() => Tomar<T>(Cola.Peek);

        private T Tomar<T>(Func<object> extractor)
        {
            var elemento = Cola.Any() ? extractor() : null;

            if (elemento != null)
                if (elemento is T)
                    return (T)elemento;

            return default(T);
        }
    }
}

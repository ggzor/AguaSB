using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace AguaSB.Navegacion
{
    public class ColaNavegacion
    {
        private Queue<object> Cola;

        public bool TieneInformacion => Cola.Any();

        public ColaNavegacion(IEnumerable<object> parametros)
        {
            Cola = new Queue<object>(parametros);
        }

        public ColaNavegacion(params object[] parametros)
        {
            Cola = new Queue<object>(parametros);
        }

        public T Siguiente<T>() => Tomar<T>(Cola.Dequeue);

        private T Tomar<T>(Func<object> extractor)
        {
            var elemento = Cola.Any() ? extractor() : null;

            if (elemento != null && elemento is T)
                return (T)elemento;

            return default(T);
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otro = obj as ColaNavegacion;
            return otro.Cola.SequenceEqual(Cola);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => Cola.GetHashCode();

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"[{string.Join(", ", Cola)}]";

        public static readonly ColaNavegacion Vacia = new ColaNavegacion();
    }
}

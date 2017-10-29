using System;
using System.Collections;

namespace AguaSB.Views.Utilerias
{
    public class FuncComparer : IComparer
    {
        public Func<object, object, int> Comparador { get; }

        public FuncComparer(Func<object, object, int> comparador) =>
            Comparador = comparador ?? throw new ArgumentNullException(nameof(comparador));

        public int Compare(object x, object y) => Comparador(x, y);
    }
}

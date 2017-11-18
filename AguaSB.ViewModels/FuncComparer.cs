using System;
using System.Collections;
using System.Collections.Generic;

namespace AguaSB.ViewModels
{
    public class FuncComparer : IComparer
    {
        public Func<object, object, int> Comparador { get; }

        public FuncComparer(Func<object, object, int> comparador) =>
            Comparador = comparador ?? throw new ArgumentNullException(nameof(comparador));

        public int Compare(object x, object y) => Comparador(x, y);

        public static readonly FuncComparer<int> Enteros = new FuncComparer<int>((x, y) => x.CompareTo(y));
    }

    public class FuncComparer<T> : IComparer<T>
    {
        public Func<T, T, int> Comparador { get; }

        public FuncComparer(Func<T, T, int> comparador) =>
            Comparador = comparador ?? throw new ArgumentNullException(nameof(comparador));

        public int Compare(T x, T y) => Comparador(x, y);
    }
}

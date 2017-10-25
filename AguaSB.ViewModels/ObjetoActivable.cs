using System;

namespace AguaSB.ViewModels
{
    public class ObjetoActivable<T> : Activable
    {
        private T valor;

        public virtual T Valor
        {
            get { return valor; }
            set { N.Set(ref valor, value); }
        }

        public Func<T, string> Formato { get; set; } = v => v?.ToString();

        public override string ToString() => Formato(Valor);
    }
}

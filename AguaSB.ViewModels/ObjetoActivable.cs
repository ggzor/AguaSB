using System;

namespace AguaSB.ViewModels
{
    public interface IObjetoActivable : IActivable
    {
        object Valor { get; set; }
    }

    public class ObjetoActivable<T> : Activable, IObjetoActivable
    {
        private T valor;

        public virtual T Valor
        {
            get { return valor; }
            set { N.Set(ref valor, value); }
        }

        object IObjetoActivable.Valor { get => Valor; set { Valor = (T)value; } }

        public Func<T, string> Formato { get; set; } = v => v?.ToString();

        public override string ToString() => Formato(Valor);
    }
}

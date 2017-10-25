namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public abstract class Filtro<T>
    {
    }

    public sealed class PorValor<T> : Filtro<T>
    {
        public T Valor { get; }

        public PorValor(T valor)
        {
            Valor = valor;
        }

        public override string ToString() => Valor?.ToString();
    }

    public sealed class Cualquiera<T> : Filtro<T>
    {
        public static Cualquiera<T> Instancia { get; } = new Cualquiera<T>();

        private Cualquiera() { }

        public override string ToString() => "Cualquiera";
    }
}

namespace AguaSB.ViewModels
{
    public sealed class ObjetoNombrado<T>
    {
        public string Nombre { get; set; }

        public T Valor { get; set; }
    }
}

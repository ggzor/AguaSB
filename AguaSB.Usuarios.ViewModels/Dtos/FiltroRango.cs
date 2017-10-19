namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class FiltroRango<T> : Filtro
    {
        private T desde;
        private T hasta;

        public T Desde
        {
            get { return desde; }
            set { N.Validate(ref desde, value); }
        }

        public T Hasta
        {
            get { return hasta; }
            set { N.Validate(ref hasta, value); }
        }
    }
}

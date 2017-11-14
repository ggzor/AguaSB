using System.ComponentModel;

namespace AguaSB.Utilerias.Solicitudes
{
    public sealed class Ordenamiento
    {
        public Propiedad Propiedad { get; set; }

        public ListSortDirection? Direccion { get; set; }

        public void Cambiar()
        {
            switch (Direccion)
            {
                case ListSortDirection.Ascending:
                    Direccion = ListSortDirection.Descending;
                    break;
                case ListSortDirection.Descending:
                    Direccion = null;
                    break;
                default:
                    Direccion = ListSortDirection.Ascending;
                    break;
            }
        }
    }
}

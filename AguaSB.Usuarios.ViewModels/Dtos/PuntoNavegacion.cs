namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class PuntoNavegacion
    {
        public string Nombre { get; set; }

        public int Indice { get; set; }

        public override string ToString() => Nombre;
    }
}

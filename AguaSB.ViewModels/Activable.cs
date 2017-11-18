using AguaSB.Utilerias;

namespace AguaSB.ViewModels
{
    public interface IActivable
    {
        bool Activo { get; set; }
    }

    public class Activable : Notificante, IActivable
    {
        private bool activo;

        public bool Activo
        {
            get { return activo; }
            set { N.Set(ref activo, value); }
        }
    }
}

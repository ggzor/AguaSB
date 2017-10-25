using AguaSB.Utilerias;

namespace AguaSB.ViewModels
{
    public class Activable : Notificante
    {
        private bool activo;

        public bool Activo
        {
            get { return activo; }
            set { N.Set(ref activo, value); }
        }
    }
}

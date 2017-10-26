using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Columnas : Notificante
    {
        private bool contratos;
        private bool fechaRegistro;
        private bool ultimoPago;
        private bool adeudo;

        public static Columnas Todas => new Columnas()
        {
            Contratos = true,
            FechaRegistro = true,
            UltimoPago = true,
            Adeudo = true
        };

        #region Propiedades
        public bool Contratos
        {
            get { return contratos; }
            set { N.Set(ref contratos, value); }
        }

        public bool FechaRegistro
        {
            get { return fechaRegistro; }
            set { N.Set(ref fechaRegistro, value); }
        }

        public bool UltimoPago
        {
            get { return ultimoPago; }
            set { N.Set(ref ultimoPago, value); }
        }

        public bool Adeudo
        {
            get { return adeudo; }
            set { N.Set(ref adeudo, value); }
        }
        #endregion
    }
}

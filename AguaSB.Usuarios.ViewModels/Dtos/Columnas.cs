using AguaSB.Utilerias;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class Columnas : Notificante
    {
        private bool contratos;
        private bool fechaRegistro;
        private bool ultimoPago;
        private bool pagadoHasta;
        private bool adeudo;
        private bool seccion;
        private bool calle;
        private bool numero;

        public static Columnas Todas => new Columnas()
        {
            Contratos = true,
            FechaRegistro = true,
            UltimoPago = true,
            PagadoHasta = true,
            Adeudo = true,
            Seccion = true,
            Calle = true,
            Numero = true
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

        public bool PagadoHasta
        {
            get { return pagadoHasta; }
            set { N.Set(ref pagadoHasta, value); }
        }

        public bool Adeudo
        {
            get { return adeudo; }
            set { N.Set(ref adeudo, value); }
        }

        public bool Seccion
        {
            get { return seccion; }
            set { N.Set(ref seccion, value); }
        }

        public bool Calle
        {
            get { return calle; }
            set { N.Set(ref calle, value); }
        }

        public bool Numero
        {
            get { return numero; }
            set { N.Set(ref numero, value); }
        }
        #endregion
    }
}

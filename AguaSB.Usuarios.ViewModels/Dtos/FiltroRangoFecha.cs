using System;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class FiltroRangoFecha : Filtro
    {
        private DateTime desde;
        private DateTime hasta;

        public DateTime Desde
        {
            get { return desde; }
            set { N.Validate(ref desde, value); }
        }

        public DateTime Hasta
        {
            get { return hasta; }
            set { N.Validate(ref hasta, value); }
        }

    }
}

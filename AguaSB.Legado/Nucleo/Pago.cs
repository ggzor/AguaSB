using System;

namespace AguaSB.Legado.Nucleo
{
    public class Pago
    {
        public int Folio { get; set; }
        public DateTime FechaPago { get; set; }
        public Usuario Usuario { get; set; }
        public string Meses { get; set; }
        public int Cantidad { get; set; }
        public string CantidadLetra { get; set; }
    }
}

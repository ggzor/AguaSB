using AguaSB.ViewModels;
using System;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class RangoPago : Activable
    {
        public DateTime Hasta { get; set; }
        public decimal Monto { get; set; }
        public decimal AdeudoRestante { get; set; }

        public bool EsPrimeroConRestanteCero { get; set; }
        public bool NoEsPrimeroConRestanteCero => !EsPrimeroConRestanteCero;
    }
}

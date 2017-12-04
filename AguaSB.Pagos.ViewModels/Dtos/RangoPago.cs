using System;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class RangoPago
    {
        public DateTime Hasta { get; set; }
        public decimal Monto { get; set; }
        public decimal AdeudoRestante { get; set; }

        public RangoPago()
        {

        }
    }
}

using AguaSB.Nucleo;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class InformacionContrato
    {
        public Contrato Contrato { get; set; }
        public Pago UltimoPago { get; set; }
        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0;
        public bool NoTieneAdeudo => !TieneAdeudo;

        public override string ToString() => Contrato.ToString();
    }
}

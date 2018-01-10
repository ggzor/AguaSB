using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Pagos
{
    public interface ILocalizadorPagos
    {
        Pago UltimoDe(Contrato contrato);
    }
}

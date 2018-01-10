using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Pagos.Entity
{
    public class LocalizadorPagos : OperacionesEntity, ILocalizadorPagos
    {
        public LocalizadorPagos(Mehdime.Entity.IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public Pago UltimoDe(Contrato contrato) => BaseDeDatos.Pagos.Find(contrato.Id);
    }
}

using AguaSB.Datos.Decoradores;
using AguaSB.Nucleo;

namespace AguaSB.Pagos.ViewModels.Notificaciones
{
    public class PagoRealizado : EntidadAgregada<Pago>
    {
        public PagoRealizado(Pago pago) : base(pago)
        {
            Clase = "Pagos";
            Descripcion = $"Se ha realizado el pago del contrato \"{pago.Contrato}\"";
            Titulo = $"Nuevo pago - {pago.CantidadPagada:C}";
        }
    }
}

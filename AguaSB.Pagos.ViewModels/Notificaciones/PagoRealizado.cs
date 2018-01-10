using AguaSB.Datos.Decoradores;
using AguaSB.Nucleo;

namespace AguaSB.Pagos.ViewModels.Notificaciones
{
    public class PagoRealizado : EntidadAgregada<Pago>
    {
        public PagoRealizado(Pago entidad) : base(entidad)
        {
        }

        public PagoRealizado(Pago pago, string domicilio) : base(pago)
        {
            Clase = "Pagos";
            Descripcion = $"Se ha realizado el pago del contrato \"{domicilio}\"";
            Titulo = $"Nuevo pago - {pago.Monto:C}";
        }
    }
}

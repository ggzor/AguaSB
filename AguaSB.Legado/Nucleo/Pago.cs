using System;

namespace AguaSB.Legado.Nucleo
{
    public class Pago
    {
        public int Folio { get; set; }
        public int NumeroUsuario { get; set; }
        public string Contrato { get; set; }
        public string TipoContrato { get; set; }
        public string NombreUsuario { get; set; }
        public string Domicilio { get; set; }
        public string Seccion { get; set; }
        public string Meses { get; set; }
        public DateTime FechaPago { get; set; }
        public int Cantidad { get; set; }
        public string CantidadLetra { get; set; }
    }
}

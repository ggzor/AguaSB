using System;

namespace AguaSB.Operaciones.Montos
{
    public class CambioTarifa : IDetalleMonto
    {
        public DateTime Mes { get; set; }
        public decimal Anterior { get; set; }
        public decimal Nueva { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otro = (CambioTarifa)obj;
            return Anterior == otro.Anterior && Nueva == otro.Nueva && Mes == otro.Mes;
        }

        public override int GetHashCode() => Anterior.GetHashCode() ^ Nueva.GetHashCode() ^ Mes.GetHashCode();

        public override string ToString() => $"{Mes:MMMM yyyy}: {Anterior:C} → {Nueva:C}";
    }
}

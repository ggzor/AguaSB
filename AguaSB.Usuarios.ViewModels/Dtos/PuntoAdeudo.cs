using System;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public struct PuntoAdeudo
    {
        public DateTime Fecha { get; set; }

        public decimal Adeudo { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otro = (PuntoAdeudo)obj;
            return (Fecha == otro.Fecha) && (Adeudo == otro.Adeudo);
        }

        public override int GetHashCode() => Fecha.GetHashCode() ^ Adeudo.GetHashCode();

        public static bool operator ==(PuntoAdeudo left, PuntoAdeudo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PuntoAdeudo left, PuntoAdeudo right)
        {
            return !(left == right);
        }
    }
}

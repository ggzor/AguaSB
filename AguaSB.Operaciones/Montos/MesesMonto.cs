﻿using System;

using AguaSB.Utilerias;

namespace AguaSB.Operaciones.Montos
{
    public class MesesMonto : IDetalleMonto
    {
        public DateTime Inicio { get; }
        public DateTime Fin { get; }
        public decimal TarifaMensual { get; }

        public MesesMonto(DateTime inicio, DateTime fin, decimal tarifaMensual)
        {
            if (inicio > fin)
                throw new ArgumentException("inicio > fin");

            Inicio = inicio;
            Fin = fin;
            TarifaMensual = tarifaMensual;
        }

        public long CantidadMeses => Fecha.DiferenciaMeses(Fin, Inicio) + 1;
        public decimal Monto => CantidadMeses * TarifaMensual;
        public string Meses
        {
            get
            {
                if (Inicio == Fin)
                    return $"{Inicio:MMMM yyyy}";
                else if (Inicio.Year == Fin.Year && Inicio.Month == 1 && Fin.Month == 12)
                    return $"todo {Inicio:yyyy}";
                else
                    return $"{Inicio:MMMM} - {Fin:MMMM yyyy}";
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var otro = (MesesMonto)obj;

            return Inicio == otro.Inicio && Fin == otro.Fin && TarifaMensual == otro.TarifaMensual;
        }

        public override int GetHashCode() => Inicio.GetHashCode() ^ Fin.GetHashCode() ^ Monto.GetHashCode();

        public override string ToString() => $"{{Meses: {Meses}, Monto: {Monto}}}";
    }
}

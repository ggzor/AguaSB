using System;

using AguaSB.Nucleo;

namespace AguaSB.Operaciones.Montos
{
    public interface ICalculadorMontos
    {
        Monto CalcularPara(Contrato contrato, DateTime desde, DateTime hasta);
    }
}

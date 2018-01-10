using System.Collections.Generic;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Adeudos;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public sealed class OpcionesPago
    {
        public PagoPorRangos PagoPorRangos { get; }
        public PagoPorPropiedades PagoPorPropiedades { get; }

        public OpcionesPago(Usuario usuario, IReadOnlyCollection<Adeudo> adeudosContratos, ICalculadorMontos montos)
        {
            PagoPorRangos = new PagoPorRangos(usuario, adeudosContratos, montos);
            PagoPorPropiedades = new PagoPorPropiedades(usuario, adeudosContratos, montos);
        }
    }
}

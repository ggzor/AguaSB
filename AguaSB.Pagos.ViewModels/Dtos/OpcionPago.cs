using System;
using System.Collections.Generic;

using AguaSB.Nucleo;
using AguaSB.Utilerias;

using AguaSB.Operaciones.Adeudos;
using AguaSB.Operaciones.Montos;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public abstract class OpcionPago : Notificante
    {
        public Usuario Usuario { get; }
        public IReadOnlyCollection<Adeudo> AdeudosContratos { get; }
        public ICalculadorMontos Montos { get; }

        public OpcionPago(Usuario usuario, IReadOnlyCollection<Adeudo> adeudosContratos, ICalculadorMontos montos)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            AdeudosContratos = adeudosContratos ?? throw new ArgumentNullException(nameof(adeudosContratos));
            Montos = montos ?? throw new ArgumentNullException(nameof(montos));
        }

        public abstract Pago GenerarPago();
    }
}

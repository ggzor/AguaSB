using System;
using System.Collections.Generic;

using AguaSB.Nucleo;
using AguaSB.Utilerias;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public abstract class OpcionPago : Notificante
    {
        public Usuario Usuario { get; }
        public IReadOnlyCollection<InformacionContrato> Contratos { get; }
        public Tarifa[] Tarifas { get; }

        public OpcionPago(Usuario usuario, IReadOnlyCollection<InformacionContrato> contratos, Tarifa[] tarifas)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));
            Contratos = contratos ?? throw new ArgumentNullException(nameof(contratos));
            Tarifas = tarifas ?? throw new ArgumentNullException(nameof(tarifas));
        }

        public abstract Pago GenerarPago();
    }
}

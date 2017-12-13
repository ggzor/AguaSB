using AguaSB.Nucleo;
using AguaSB.Utilerias;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class InformacionPagoUsuario : Notificante
    {
        public Usuario Usuario { get; set; }

        public InformacionPagoContrato[] Contratos { get; set; }

        public InformacionPagoUsuario(Usuario usuario, IEnumerable<InformacionPagoContrato> contratos)
        {
            Usuario = usuario ?? throw new ArgumentNullException(nameof(usuario));

            if (contratos == null)
                throw new ArgumentNullException(nameof(contratos));

            Contratos = contratos.ToArray();

            if (Contratos.Length == 0)
                throw new ArgumentException("Se requiere al menos un contrato para ser seleccionado.", nameof(contratos));

            Contratos[0].Activo = true;
            Contratos[0].Columnas[0].RangosPago[0].Activo = true;

            Contratos.Select(c => c.ToObservableProperties())
                .Merge()
                .Subscribe(e =>
                {
                    N.Change(nameof(ContratoSeleccionado));
                });
        }

        public InformacionPagoContrato ContratoSeleccionado
        {
            get
            {
                var seleccionado = Contratos.Where(c => c.Activo).FirstOrDefault() ?? Contratos[0];

                Contratos.SelectMany(c => c.Columnas.SelectMany(col => col.RangosPago))
                    .ForEach(r => r.Activo = false);

                seleccionado.Activo = true;
                seleccionado.Columnas[0].RangosPago[0].Activo = true;

                return seleccionado;
            }
        }

        public RangoPago RangoPagoSeleccionado =>
            ContratoSeleccionado.Columnas
                .SelectMany(c => c.RangosPago)
                .Single(r => r.Activo);

        public bool TieneContratoUnico => Contratos.Length == 1;

        public bool TieneMultiplesContratos => Contratos.Length > 1;
    }
}

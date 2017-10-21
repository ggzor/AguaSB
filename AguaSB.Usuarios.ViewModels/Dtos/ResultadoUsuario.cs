using System;
using System.Collections.Generic;

using AguaSB.Nucleo;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class ResultadoUsuario
    {
        public Usuario Usuario { get; set; }

        public bool EsPersona => Usuario?.GetType() == typeof(Persona);

        public bool EsNegocio => Usuario?.GetType() == typeof(Negocio);

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m;

        public bool NoTieneAdeudo => !TieneAdeudo;

        public DateTime? UltimoPago { get; set; }

        public IEnumerable<ResultadoContrato> Contratos { get; set; }
    }

    public class ResultadoContrato
    {
        public Contrato Contrato { get; set; }

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m;

        public bool NoTieneAdeudo => !TieneAdeudo;
    }
}

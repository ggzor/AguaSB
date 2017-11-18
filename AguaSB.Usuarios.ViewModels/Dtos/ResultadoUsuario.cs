using AguaSB.Nucleo;
using System;
using System.Collections.Generic;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class ResultadoUsuario
    {
        public Usuario Usuario { get; set; }

        public bool EsPersona => Usuario?.GetType() == typeof(Persona) && NoEsTitulo;

        public bool EsNegocio => Usuario?.GetType() == typeof(Negocio) && NoEsTitulo;

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m && NoEsTitulo;

        public bool NoTieneAdeudo => !TieneAdeudo && NoEsTitulo;

        public DateTime? UltimoPago { get; set; }

        public DateTime? UltimoMesPagado { get; set; }

        public Domicilio Domicilio { get; set; }

        public IEnumerable<ResultadoContrato> Contratos { get; set; }

        public string Titulo { get; set; }

        public bool EsTitulo => !string.IsNullOrWhiteSpace(Titulo);

        public bool NoEsTitulo => !EsTitulo;

        public string Subtitulo { get; set; }
    }

    public class ResultadoContrato
    {
        public Contrato Contrato { get; set; }

        public DateTime? UltimoMesPagado { get; set; }

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m;

        public bool NoTieneAdeudo => !TieneAdeudo;
    }
}

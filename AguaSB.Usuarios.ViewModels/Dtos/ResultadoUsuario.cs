using AguaSB.Nucleo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Usuarios.ViewModels.Dtos
{
    public class ResultadoUsuario
    {
        public Usuario Usuario { get; set; }

        public bool EsPersona => typeof(Persona).IsAssignableFrom(Usuario.GetType()) && NoEsTitulo;

        public bool EsNegocio => typeof(Negocio).IsAssignableFrom(Usuario.GetType()) && NoEsTitulo;

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m && NoEsTitulo;

        public bool NoTieneAdeudo => !TieneAdeudo && NoEsTitulo;

        public DateTime? UltimoPago { get; set; }

        public DateTime? UltimoMesPagado { get; set; }

        public Domicilio Domicilio { get; set; }

        public IEnumerable<ResultadoContrato> Contratos { get; set; }

        public bool TieneContratos => Contratos?.Any() ?? false;

        public bool TieneMasDeUnContrato => (Contratos?.Count() ?? 0) > 1;

        public IEnumerable<Contacto> Contactos { get; set; }

        public bool TieneContactos => Contactos?.Any() ?? false;

        public string Titulo { get; set; }

        public bool EsTitulo => !string.IsNullOrWhiteSpace(Titulo);

        public bool NoEsTitulo => !EsTitulo;

        public string Subtitulo { get; set; }
    }

    public class ResultadoContrato
    {
        public Contrato Contrato { get; set; }

        public Domicilio Domicilio { get; set; }

        public DateTime? UltimoPago { get; set; }

        public DateTime? UltimoMesPagado { get; set; }

        public decimal Adeudo { get; set; }

        public bool TieneAdeudo => Adeudo > 0m;

        public bool NoTieneAdeudo => !TieneAdeudo;
    }
}

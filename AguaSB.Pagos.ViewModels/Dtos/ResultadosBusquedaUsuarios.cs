using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;

namespace AguaSB.Pagos.ViewModels.Dtos
{
    public class ResultadosBusquedaUsuarios
    {
        public int CantidadOpciones { get; set; }

        public ResultadosBusquedaUsuarios(int cantidadOpciones)
        {
            if (cantidadOpciones < 1)
                throw new ArgumentOutOfRangeException(nameof(cantidadOpciones), "Debe ser mayor a 0.");

            CantidadOpciones = cantidadOpciones;
        }

        public IEnumerable<Usuario> Resultados { get; private set; }
        public int TotalResultados { get; private set; }

        public bool? HayResultados => Resultados == null ? (bool?)null : TotalResultados > 0;
        public bool? NoHayResultados => !HayResultados;

        public int CantidadOpcionesAdicionales => Math.Max(0, TotalResultados - CantidadOpciones);
        public bool? HayMasOpciones => Resultados == null ? (bool?)null : CantidadOpcionesAdicionales > 0;

        public int CoincidentesSinContrato { get; set; }
        public bool HayCoincidentesSinContrato => CoincidentesSinContrato > 0 && !(HayMasOpciones == true);

        public void Buscar(IQueryable<Usuario> usuarios, string nombre)
        {
            nombre = Usuario.ConvertirATextoSolicitud(nombre);

            if (int.TryParse(nombre, out var id))
            {
                Resultados = usuarios.Where(u => u.Id == id).ToArray();

                TotalResultados = Resultados.Count();
            }
            else
            {

                var coincidentes = from usuario in usuarios
                                   where usuario.NombreSolicitud.Contains(nombre)
                                   orderby usuario.NombreSolicitud
                                   select usuario;

                CoincidentesSinContrato = coincidentes.Count(u => !u.Contratos.Any());

                var coincidentesConContrato = coincidentes.Where(u => u.Contratos.Any());

                Resultados = coincidentesConContrato.Take(CantidadOpciones).ToArray();
                TotalResultados = coincidentesConContrato.Count();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

using AguaSB.Nucleo;
using AguaSB.Datos;

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

        public void Buscar(IQueryable<Usuario> usuarios, string nombre, IRepositorio<TipoNota> tiposNota)
        {
            nombre = Usuario.ConvertirATextoSolicitud(nombre);

            if (int.TryParse(nombre, out var id))
            {
                var idCadena = id.ToString();
                var notaFila = (from tipoNota in tiposNota.Datos
                                where tipoNota.Nombre == "_Usuario_NumeroUsuario"
                                from nota in tipoNota.Notas
                                where nota.Informacion == idCadena
                                select nota).SingleOrDefault();

                if (notaFila?.Referencia is int idUsuario)
                {
                    Resultados = usuarios.Where(u => u.Id == idUsuario).ToArray();
                }
                else
                {
                    Resultados = Enumerable.Empty<Usuario>();
                }

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
using AguaSB.Datos;
using AguaSB.Nucleo;
using CsvHelper;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Inicializadores
{
    public class AgregarUsuarios : IInicializador
    {
        public class UsuarioCSV
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Paterno { get; set; }
            public string Materno { get; set; }
            public int Seccion { get; set; }
            public string Calle { get; set; }
            public string Numero { get; set; }
            public string Contrato { get; set; }
            public DateTime PagadoHasta { get; set; }
        }

        public AgregarUsuarios(IRepositorio<Usuario> usuarios, IRepositorio<Contrato> contratos, IRepositorio<TipoContrato> tiposContrato,
            IRepositorio<Calle> calles, IRepositorio<Seccion> secciones, IRepositorio<Domicilio> domicilios) =>
            Llenar(usuarios, contratos, tiposContrato, calles, secciones, domicilios);

        private async void Llenar(IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo,
            IRepositorio<Calle> callesRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<Domicilio> domiciliosRepo)
        {
            Console.WriteLine("Obteniendo archivo...");
            IList<UsuarioCSV> todos = null;
            using (var reader = File.OpenText("Nombres.csv"))
            {
                var csv = new CsvReader(reader);
                todos = csv.GetRecords<UsuarioCSV>().ToList();
            }
            Console.WriteLine("Lectura terminada.");

            Console.WriteLine("Clasificando informacion");

            var callesAgrupadas = todos.Select(u => (u.Seccion, u.Calle)).ToLookup(t => t.Seccion, t => t.Calle);

            var secciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Index().Select(t => new Seccion { Nombre = t.Value, Orden = t.Key + 1 }).ToArray();

            foreach (var seccion in secciones)
                await seccionesRepo.Agregar(seccion);

            foreach (var indiceSeccion in callesAgrupadas)
            {
                foreach (var nombreCalle in indiceSeccion.Distinct())
                {
                    var seccion = secciones[indiceSeccion.Key - 1];
                    var calle = new Calle { Nombre = nombreCalle, Seccion = seccion };
                    await callesRepo.Agregar(calle);
                    seccion.Calles.Add(calle);
                }
            }

            var tiposContrato = new Dictionary<string, TipoContrato>
            {
                ["A"] = new TipoContrato { Nombre = "Convencional", Multiplicador = 1.0m, ClaseContrato = ClaseContrato.Doméstico },
                ["B"] = new TipoContrato { Nombre = "Normal", Multiplicador = 2.0m, ClaseContrato = ClaseContrato.Industrial },
                ["C"] = new TipoContrato { Nombre = "Básico", Multiplicador = 2.0m, ClaseContrato = ClaseContrato.Comercial },
                ["M"] = new TipoContrato { Nombre = "Tercera edad", Multiplicador = 0.3m, ClaseContrato = ClaseContrato.Doméstico },
                ["P"] = new TipoContrato { Nombre = "Blockera", Multiplicador = 3.0m, ClaseContrato = ClaseContrato.Industrial },
                ["D"] = new TipoContrato { Nombre = "Tienda", Multiplicador = 1.5m, ClaseContrato = ClaseContrato.Comercial }
            };

            foreach (var tipoContrato in tiposContrato.Values)
                await tiposContratoRepo.Agregar(tipoContrato);

            var r = new Random();

            foreach (var usuariocsv in todos)
            {
                var usuario = new Persona
                {
                    Nombre = usuariocsv.Nombre,
                    ApellidoPaterno = usuariocsv.Paterno,
                    ApellidoMaterno = usuariocsv.Materno,
                    FechaRegistro = new DateTime(r.Next(2005, 2010), r.Next(1, 13), r.Next(1, 29))
                };

                var domicilio = new Domicilio
                {
                    Numero = usuariocsv.Numero,
                    Calle = callesRepo.Datos.Single(c => c.Nombre == usuariocsv.Calle && c.Seccion.Orden == usuariocsv.Seccion)
                };

                var contrato = new Contrato
                {
                    MedidaToma = "1/2",
                    Usuario = usuario,
                    TipoContrato = tiposContrato[usuariocsv.Contrato],
                    Domicilio = domicilio
                };
                usuario.Contratos.Add(contrato);

                await usuariosRepo.Agregar(usuario);
                await domiciliosRepo.Agregar(domicilio);
                await contratosRepo.Agregar(contrato);
            }
        }
    }
}

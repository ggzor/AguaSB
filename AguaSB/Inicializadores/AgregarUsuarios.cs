using AguaSB.Datos;
using AguaSB.Datos.Entity;
using AguaSB.Nucleo;
using CsvHelper;
using Mehdime.Entity;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            public DateTime UltimoPago { get; set; }
        }

        public AgregarUsuarios(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<Contrato> contratosRepo, IRepositorio<TipoContrato> tiposContratoRepo, IRepositorio<Pago> pagosRepo,
            IRepositorio<Calle> callesRepo, IRepositorio<Seccion> seccionesRepo, IRepositorio<Domicilio> domiciliosRepo, IRepositorio<Ajustador> ajustadoresRepo)
        {
            IList<UsuarioCSV> usuariosLeidos = null;

            Console.WriteLine("Leyendo usuarios...");
            using (var reader = File.OpenText("Nombres.csv"))
            {
                var csv = new CsvReader(reader);
                usuariosLeidos = csv.GetRecords<UsuarioCSV>().ToList();
            }
            Console.WriteLine("Lectura terminada.");

            Console.WriteLine("Registrando secciones...");
            using (var baseDeDatos = ambito.Create())
            {
                if (seccionesRepo.Datos.Count() > 0)
                {
                    Console.WriteLine("No se requirió registrar los domicilios y usuarios.");
                    return;
                }

                var secciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Index()
                    .Select(t => new Seccion { Nombre = t.Value, Orden = t.Key + 1 });

                foreach (var seccion in secciones)
                    seccionesRepo.Agregar(seccion);

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");

            Console.WriteLine("Agregando calles...");
            var callesAgrupadas = usuariosLeidos.Select(u => (u.Seccion, u.Calle))
                .ToLookup(t => t.Seccion, t => t.Calle);

            using (var baseDeDatos = ambito.Create())
            {
                var secciones = baseDeDatos.DbContexts.Get<EntidadesDbContext>().Secciones.OrderBy(_ => _.Orden).ToArray();

                foreach (var indiceSeccion in callesAgrupadas)
                {
                    foreach (var nombreCalle in indiceSeccion.Distinct())
                    {
                        var seccion = secciones[indiceSeccion.Key - 1];
                        var calle = new Calle { Nombre = nombreCalle, Seccion = seccion };

                        callesRepo.Agregar(calle);
                    }
                }

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");

            Console.WriteLine("Registrando tipos de contrato...");
            using (var baseDeDatos = ambito.Create())
            {
                var tiposContrato = new[]
                {
                    new TipoContrato { Nombre = "Convencional", Multiplicador = 1.0m, ClaseContrato = ClaseContrato.Doméstico },
                    new TipoContrato { Nombre = "Normal", Multiplicador = 2.0m, ClaseContrato = ClaseContrato.Industrial },
                    new TipoContrato { Nombre = "Básico", Multiplicador = 2.0m, ClaseContrato = ClaseContrato.Comercial },
                    new TipoContrato { Nombre = "Tercera edad", Multiplicador = 0.3m, ClaseContrato = ClaseContrato.Doméstico },
                    new TipoContrato { Nombre = "Blockera", Multiplicador = 3.0m, ClaseContrato = ClaseContrato.Industrial },
                    new TipoContrato { Nombre = "Tienda", Multiplicador = 1.5m, ClaseContrato = ClaseContrato.Comercial }
                };

                foreach (var tipoContrato in tiposContrato)
                    tiposContratoRepo.Agregar(tipoContrato);

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");

            Console.WriteLine("Registrando ajustador de registro...");
            using (var baseDeDatos = ambito.Create())
            {
                var ajustadorRegistro = new Ajustador
                {
                    Nombre = "Registro",
                    Multiplicador = 1
                };

                ajustadoresRepo.Agregar(ajustadorRegistro);
                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");

            var r = new Random();

            Console.WriteLine("Registrando usuarios...");
            using (var baseDeDatos = ambito.Create())
            {
                var contexto = baseDeDatos.DbContexts.Get<EntidadesDbContext>();

                var calles = contexto.Calles.Include(nameof(Calle.Seccion)).ToArray();
                var ajustadorRegistro = contexto.Ajustadores.Single(_ => _.Nombre == "Registro");

                var mapeos = new Dictionary<string, string>
                {
                    ["Convencional"] = "A",
                    ["Normal"] = "B",
                    ["Básico"] = "C",
                    ["Tercera edad"] = "M",
                    ["Blockera"] = "P",
                    ["Tienda"] = "D",
                };

                var tiposContrato = contexto.TiposContrato.ToDictionary(_ => mapeos[_.Nombre]);

                var nombres = new HashSet<string>();

                foreach (var usuariocsv in usuariosLeidos)
                {
                    var usuario = new Persona
                    {
                        Nombre = usuariocsv.Nombre,
                        ApellidoPaterno = usuariocsv.Paterno,
                        ApellidoMaterno = usuariocsv.Materno,
                        FechaRegistro = new DateTime(r.Next(2010, 2014), r.Next(1, 13), r.Next(1, 29))
                    };

                    if (nombres.Contains(usuario.NombreSolicitud))
                    {
                        Console.WriteLine($"Se repite {usuario.NombreCompleto}");
                        continue;
                    }
                    else
                    {
                        nombres.Add(usuario.NombreSolicitud);
                    }

                    if (usuario.HasErrors)
                        continue;

                    var domicilio = new Domicilio
                    {
                        Numero = usuariocsv.Numero,
                        Calle = calles.Single(c => c.Nombre == usuariocsv.Calle && c.Seccion.Orden == usuariocsv.Seccion)
                    };

                    var contrato = new Contrato
                    {
                        MedidaToma = "1/2",
                        Usuario = usuario,
                        TipoContrato = tiposContrato[usuariocsv.Contrato],
                        Domicilio = domicilio,
                        FechaRegistro = usuario.FechaRegistro.AddMinutes(30)
                    };

                    usuario.Contratos.Add(contrato);

                    usuariosRepo.Agregar(usuario);
                    domiciliosRepo.Agregar(domicilio);
                    contratosRepo.Agregar(contrato);

                    var pago = new Pago
                    {
                        Contrato = contrato,
                        Desde = usuariocsv.PagadoHasta,
                        Hasta = usuariocsv.PagadoHasta,
                        Ajustador = ajustadorRegistro,
                        FechaRegistro = contrato.FechaRegistro,
                        MontoParcial = 0m
                    };

                    pago.Coercer();

                    contrato.Pagos.Add(pago);
                    pagosRepo.Agregar(pago);
                }

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");
        }
    }
}


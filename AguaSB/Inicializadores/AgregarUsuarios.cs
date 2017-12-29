using AguaSB.Datos;
using AguaSB.Nucleo;
using AguaSB.Utilerias;
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
        public AgregarUsuarios(IDbContextScopeFactory ambito, IRepositorio<Usuario> usuariosRepo, IRepositorio<TipoContrato> tiposContratoRepo, IRepositorio<Nota> notasRepo)
        {
            var parejasNotas = new LinkedList<(Contrato Contrato, int Fila)>();

            using (var baseDeDatos = ambito.Create())
            {
                Console.WriteLine("Registrando usuarios....");

                if (usuariosRepo.Datos.Any())
                {
                    Console.WriteLine("Ya se han registrado los usuarios.");
                    return;
                }

                var tiposContrato = tiposContratoRepo.Datos.ToDictionary(t => t.Clave);

                var usuarios = LeerUsuarios();
                Console.WriteLine($"Se registrarán {usuarios.Count} usuarios.");

                var secciones = (from valor in new[] { "", "Primera", "Segunda", "Tercera", "Cuarta" }.Index()
                                 select new Seccion { Nombre = valor.Value, Orden = valor.Key })
                                .ToArray();

                var calles = (from u in usuarios
                              let calle = u.Contratos.Single().Domicilio.Calle
                              select (Seccion: calle.Seccion.Orden, Nombre: calle.Nombre.Trim()))
                             .Distinct()
                             .Select(t => new Calle { Nombre = t.Nombre, Seccion = secciones[t.Seccion] })
                             .ToArray();

                usuarios = NormalizarDuplicados(usuarios);

                foreach (Persona persona in usuarios)
                {
                    var usuario = new Persona
                    {
                        Nombre = persona.Nombre,
                        ApellidoPaterno = persona.ApellidoPaterno,
                        ApellidoMaterno = persona.ApellidoMaterno,
                        FechaRegistro = Fecha.Ahora
                    };

                    foreach (var c in persona.Contratos)
                    {
                        var contrato = new Contrato
                        {
                            TipoContrato = tiposContrato[AgregarTiposContrato.TiposContrato[c.TipoContrato.Nombre].Clave],
                            Domicilio = new Domicilio
                            {
                                Numero = c.Domicilio.Numero.Take(20).ToDelimitedString(""),
                                Calle = calles.Single(calle => calle.Seccion.Orden == c.Domicilio.Calle.Seccion.Orden && calle.Nombre == c.Domicilio.Calle.Nombre),
                            },
                            FechaRegistro = Fecha.Ahora,
                            MedidaToma = c.MedidaToma,
                        };

                        var p = c.Pagos.Single();

                        var pago = new Pago
                        {
                            CantidadPagada = 0,
                            Monto = 0,
                            Desde = p.Desde,
                            Hasta = p.Hasta,
                            FechaPago = p.FechaPago,
                            FechaRegistro = Fecha.Ahora,
                        };

                        contrato.Pagos.Add(pago);
                        usuario.Contratos.Add(contrato);

                        parejasNotas.AddLast((contrato, c.Domicilio.Id));
                    }
                    usuariosRepo.Agregar(usuario);
                }
                baseDeDatos.SaveChanges();
                Console.WriteLine("Listo.");
            }

            if (parejasNotas.Any())
            {
                using (var baseDeDatos = ambito.Create())
                {
                    Console.WriteLine("Registrando notas de referencias...");

                    var tipoNota = new TipoNota { FechaRegistro = Fecha.Ahora, Nombre = "_Contrato_Fila" };

                    parejasNotas.Select(p => new Nota
                    {
                        FechaRegistro = Fecha.Ahora,
                        Informacion = p.Fila.ToString(),
                        Referencia = p.Contrato.Id,
                        Tipo = tipoNota
                    }).ForEach(n => notasRepo.Agregar(n));

                    baseDeDatos.SaveChanges();
                    Console.WriteLine("Listo.");
                }
            }
        }

        private IReadOnlyList<Usuario> NormalizarDuplicados(IReadOnlyList<Usuario> usuarios)
        {
            var agrupados = usuarios.GroupBy(u => u.NombreCompleto);

            var contratoUnico = agrupados.Where(g => g.Count() == 1).Select(g => g.Single());

            var contratoMultiple = agrupados.Where(g => g.Count() > 1).Select(g => g.ToArray()).Select(g =>
            {
                var primero = g[0];

                g.Skip(1).ForEach(u => primero.Contratos.Add(u.Contratos.Single()));

                return primero;
            });

            return contratoUnico.Concat(contratoMultiple).OrderBy(u => u.Id).ToList();
        }

        private IReadOnlyList<Usuario> LeerUsuarios()
        {
            var archivo = new FileInfo("Usuarios.json");

            using (var flujo = new StreamReader(archivo.OpenRead()))
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<Persona>>(flujo.ReadToEnd());
        }

    }
}

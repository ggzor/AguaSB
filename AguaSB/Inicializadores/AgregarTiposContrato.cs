using AguaSB.Datos;
using AguaSB.Nucleo;
using Mehdime.Entity;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AguaSB.Inicializadores
{
    public class AgregarTiposContrato : IInicializador
    {
        private static readonly TipoContrato MedioServicio = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 0.5m, Nombre = "Medio servicio", Clave = "AB" };
        private static readonly TipoContrato Baja = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 0.0m, Nombre = "Baja", Clave = "BJ" };

        public static readonly IReadOnlyDictionary<string, TipoContrato> TiposContrato = new Dictionary<string, TipoContrato>
        {
            ["A"] = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 1.0m, Nombre = "Normal", Clave = "A" },
            ["A1"] = MedioServicio,
            ["AB"] = MedioServicio,
            ["B"] = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 0.5m, Nombre = "Tercera edad", Clave = "B" },
            ["B1"] = new TipoContrato { ClaseContrato = ClaseContrato.Doméstico, Multiplicador = 0.25m, Nombre = "Persona mayor", Clave = "B1" },
            ["BT"] = Baja,
            ["BD"] = Baja,
            ["C"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 2.0m, Nombre = "Normal", Clave = "C" },
            ["C1"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 3.0m, Nombre = "Extra", Clave = "C1" },
            ["C2"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 8.5m, Nombre = "Hotel", Clave = "C2" },
            ["C3"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 3.0m, Nombre = "Hotel Rigel", Clave = "C3" },
            ["C4"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 5.0m, Nombre = "Hotel Chimalpa", Clave = "C4" },
            ["C5"] = new TipoContrato { ClaseContrato = ClaseContrato.Comercial, Multiplicador = 4.0m, Nombre = "Doble", Clave = "C5" },
            ["D"] = new TipoContrato { ClaseContrato = ClaseContrato.Industrial, Multiplicador = 8m, Nombre = "Normal", Clave = "D" },
            ["D2"] = new TipoContrato { ClaseContrato = ClaseContrato.Industrial, Multiplicador = 8.5m, Nombre = "Blockera", Clave = "D2" }
        };

        public AgregarTiposContrato(IDbContextScopeFactory ambito, IRepositorio<TipoContrato> tiposContratoRepo)
        {
            using (var baseDeDatos = ambito.Create())
            {
                Console.WriteLine("Agregando tipos de contrato...");

                if (tiposContratoRepo.Datos.Any())
                {
                    Console.WriteLine("Ya se han registrado los tipos de contrato.");
                    return;
                }

                TiposContrato.Values.Distinct().ForEach(t => tiposContratoRepo.Agregar(t));
                baseDeDatos.SaveChanges();
            }
        }
    }
}

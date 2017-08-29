using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AguaSB.Configuracion.Tests.Integracion
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuracion = new ConfiguracionGeneral()
            {
                Nombre = "Una configuracion y ya.",
                UID = 456782L,
                Extensiones = new List<string>()
                {
                    "E1", "E2"
                }
            };
            Console.WriteLine("Se guardara la siguiente configuracion:");
            ImprimirConfiguracion(configuracion);
            Console.WriteLine("Guardando configuracion...");

            Configuracion.Guardar(configuracion, subdirectorio: "Configuracion/General/tmp/test/", indentar: true);

            Console.WriteLine("Volviendo a leer...");
            var leida = Configuracion.Cargar<ConfiguracionGeneral>(subdirectorio: "Configuracion/General/tmp/test/");

            Console.WriteLine();
            Console.WriteLine("Se leyó:");
            ImprimirConfiguracion(leida);

            Console.ReadLine();
        }

        private static void ImprimirConfiguracion(ConfiguracionGeneral configuracion)
        {
            Console.WriteLine(configuracion.Nombre);
            Console.WriteLine(configuracion.UID);
            Console.WriteLine(string.Join(",", configuracion.Extensiones));
        }
    }

    public class ConfiguracionGeneral
    {
        public string Nombre { get; set; }

        public long UID { get; set; }

        public List<string> Extensiones { get; set; }

        public ConfiguracionGeneral()
        {
            Extensiones = new List<string>();
        }
    }
}

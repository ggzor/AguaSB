using System;
using System.Linq;

using AguaSB.Datos;
using AguaSB.Nucleo;
using Mehdime.Entity;

namespace AguaSB.Inicializadores
{
    public class Tarifas : IInicializador
    {
        public Tarifas(IDbContextScopeFactory ambito, IRepositorio<Tarifa> tarifasRepo)
        {
            Console.WriteLine("Registrando tarifas...");
            using (var baseDeDatos = ambito.Create())
            {
                if (tarifasRepo.Datos.Count() > 0)
                {
                    Console.WriteLine("No se requirió registrar las tarifas");
                    return;
                }

                var tarifas = new[]
                {
                    new Tarifa { Monto = 60, FechaRegistro = new DateTime(2016, 06, 01) },
                    new Tarifa { Monto = 70, FechaRegistro = new DateTime(2017, 07, 01) }
                };

                foreach (var tarifa in tarifas)
                    tarifasRepo.Agregar(tarifa);

                baseDeDatos.SaveChanges();
            }
            Console.WriteLine("Listo.");
        }
    }
}

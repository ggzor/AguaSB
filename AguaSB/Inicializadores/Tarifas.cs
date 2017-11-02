using System;

using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Inicializadores
{
    public class Tarifas : IInicializador
    {
        public Tarifas(IRepositorio<Tarifa> tarifasRepo) => Inicializar(tarifasRepo);

        private async void Inicializar(IRepositorio<Tarifa> tarifasRepo)
        {
            var tarifas = new[]
            {
                new Tarifa { Monto = 60, FechaRegistro = new DateTime(2016, 06, 01) },
                new Tarifa { Monto = 70, FechaRegistro = new DateTime(2017, 07, 01) }
            };

            foreach (var tarifa in tarifas)
                await tarifasRepo.Agregar(tarifa);
        }
    }
}

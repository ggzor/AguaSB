using AguaSB.Datos;
using AguaSB.Nucleo;
using System;
using System.Linq;

namespace AguaSB.Inicializadores
{
    public class LlenarTiposContrato : IInicializador
    {
        public LlenarTiposContrato(IRepositorio<TipoContrato> tiposContrato) => Cargar(tiposContrato);

        private async void Cargar(IRepositorio<TipoContrato> tiposContrato)
        {
            Console.WriteLine("Agregando tipos de contrato...");
            var tipos = new(string, ClaseContrato, decimal)[]
            {
                ("Convencional", ClaseContrato.Doméstico, 1.0M),
                ("Madre soltera", ClaseContrato.Doméstico, 0.75M),
                ("Tercera edad", ClaseContrato.Doméstico, 0.6M),
                ("Blockera", ClaseContrato.Industrial, 3.0M),
                ("Estándar", ClaseContrato.Industrial, 2.0M),
                ("Normal", ClaseContrato.Comercial, 3.0M),
                ("Tienda", ClaseContrato.Comercial, 1.5M),
                ("Hotel de hasta 10 habitaciones", ClaseContrato.Comercial, 2.0M),
                ("Hotel de hasta 20 habitaciones", ClaseContrato.Comercial, 3.0M)
            }.Select(t => new TipoContrato() { Nombre = t.Item1, ClaseContrato = t.Item2, Multiplicador = t.Item3 });

            foreach (var tipo in tipos)
                await tiposContrato.Agregar(tipo);
        }
    }
}

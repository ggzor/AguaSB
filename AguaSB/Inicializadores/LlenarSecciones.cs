using AguaSB.Datos;
using AguaSB.Nucleo;
using MoreLinq;
using System;
using System.Linq;

namespace AguaSB.Inicializadores
{
    public class LlenarSecciones : IInicializador
    {
        public LlenarSecciones(IRepositorio<Seccion> secciones, IRepositorio<Calle> calles) => Cargar(secciones, calles);

        private async void Cargar(IRepositorio<Seccion> seccionesRepo, IRepositorio<Calle> callesRepo)
        {
            Console.WriteLine("Agregando secciones...");

            var nuevasSecciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Index()
                .Select(s => new Seccion() { Nombre = s.Value, Orden = s.Key }).ToArray();

            foreach (var seccion in nuevasSecciones)
                await seccionesRepo.Agregar(seccion);

            Console.WriteLine("Agregando calles...");
            var calles = (from grupoCalles in Calles.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Batch(8).Take(4).Index()
                         let indice = grupoCalles.Key
                         let grupo = grupoCalles.Value
                         from calle in grupo
                         select new Calle() { Nombre = calle.Trim(), Seccion = nuevasSecciones[indice] })
                         .ToArray();

            foreach (var calle in calles)
            {
                calle.Seccion.Calles.Add(calle);
                await callesRepo.Agregar(calle);
            }
        }

        private const string Calles =
            @"Aguascalientes
            Baja California
            Baja California Sur
            Campeche
            Chiapas
            Chihuahua
            Ciudad de México
            Coahuila
            Colima
            Durango
            Estado de México
            Guanajuato
            Guerrero
            Hidalgo
            Jalisco
            Michoacán
            Morelos
            Nayarit
            Nuevo León
            Oaxaca
            Puebla
            Querétaro
            Quintana Roo
            San Luis Potosí
            Sinaloa
            Sonora
            Tabasco
            Tamaulipas
            Tlaxcala
            Veracruz
            Yucatán
            Zacatecas";
    }
}

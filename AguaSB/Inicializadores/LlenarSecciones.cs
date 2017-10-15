using System;
using System.Linq;
using System.Threading.Tasks;

using MoreLinq;

using AguaSB.Datos;
using AguaSB.Nucleo;

namespace AguaSB.Inicializadores
{
    public class LlenarSecciones : IInicializador
    {
        public LlenarSecciones(IRepositorio<Seccion> secciones, IRepositorio<Calle> calles) => Cargar(secciones, calles);

        private async void Cargar(IRepositorio<Seccion> seccionesRepo, IRepositorio<Calle> callesRepo)
        {
            Seccion[] nuevasSecciones = await AgregarSecciones(seccionesRepo);

            await AgregarCalles(callesRepo, nuevasSecciones);
        }

        private static async Task<Seccion[]> AgregarSecciones(IRepositorio<Seccion> seccionesRepo)
        {
            Console.WriteLine("Agregando secciones...");

            var nuevasSecciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Index()
                .Select(s => new Seccion() { Nombre = s.Value, Orden = s.Key })
                .ToArray();

            foreach (var seccion in nuevasSecciones)
                await seccionesRepo.Agregar(seccion);
            return nuevasSecciones;
        }

        private static async Task AgregarCalles(IRepositorio<Calle> callesRepo, Seccion[] nuevasSecciones)
        {
            Console.WriteLine("Agregando calles...");

            var gruposDeCalles = Calles.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Batch(8)
                .Take(4)
                .Index();

            var nuevasCalles = (from grupoCalles in gruposDeCalles
                                let indice = grupoCalles.Key
                                let calles = grupoCalles.Value
                                from calle in calles
                                select new Calle()
                                {
                                    Nombre = calle.Trim(),
                                    Seccion = nuevasSecciones[indice]
                                })
                         .ToArray();

            foreach (var calle in nuevasCalles)
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

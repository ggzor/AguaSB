using AguaSB.Datos;
using AguaSB.Nucleo;
using MoreLinq;
using System.Linq;

namespace AguaSB.Inicializadores
{
    public class LlenarSecciones : IInicializador
    {
        public LlenarSecciones(IRepositorio<Seccion> secciones) => Cargar(secciones);

        private async void Cargar(IRepositorio<Seccion> secciones)
        {
            System.Console.WriteLine("Agregando secciones...");

            var nuevasSecciones = new[] { "Primera", "Segunda", "Tercera", "Cuarta" }.Index()
                .Select(s => new Seccion() { Nombre = s.Value, Orden = s.Key });

            foreach (var seccion in nuevasSecciones)
                await secciones.Agregar(seccion);
        }
    }
}

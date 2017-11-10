using System.Linq;

using AguaSB.Datos;

namespace AguaSB.Nucleo.Datos
{
    public static class Tarifas
    {
        public static Tarifa[] Obtener(IRepositorio<Tarifa> tarifasRepo) =>
            tarifasRepo.Datos.OrderBy(_ => _.FechaRegistro).ToArray();
    }
}

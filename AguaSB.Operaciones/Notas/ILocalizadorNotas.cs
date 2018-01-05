using AguaSB.Nucleo;
using System.Linq;

namespace AguaSB.Operaciones.Notas
{
    public interface ILocalizadorNotas
    {
        IQueryable<Nota> DelTipo(string nombreTipoNota);
    }
}

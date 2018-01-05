using System.Linq;

using Mehdime.Entity;

using AguaSB.Nucleo;
using AguaSB.Operaciones.Entity;

namespace AguaSB.Operaciones.Notas.Entity
{
    public class LocalizadorNotas : OperacionesEntity, ILocalizadorNotas
    {
        public LocalizadorNotas(IAmbientDbContextLocator localizador) : base(localizador)
        {
        }

        public IQueryable<Nota> DelTipo(string nombreTipoNota) =>
            from tipoNota in BaseDeDatos.TiposNota
            where tipoNota.Nombre == nombreTipoNota
            from nota in tipoNota.Notas
            select nota;
    }
}

using System.ComponentModel;
using System.Linq;

namespace AguaSB.Utilerias
{
    public static class UtileriasErrores
    {
        public static bool AlgunoTieneErrores(params INotifyDataErrorInfo[] objetos) => objetos.Any(o => o.HasErrors);

        public static bool NingunoTieneErrores(params INotifyDataErrorInfo[] objetos) => !AlgunoTieneErrores(objetos);
    }
}

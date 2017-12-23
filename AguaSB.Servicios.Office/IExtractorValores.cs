using System.Collections.Generic;

namespace AguaSB.Servicios.Office
{
    public interface IExtractorValores<T>
    {
        IReadOnlyDictionary<string, object> Extraer(T datos);
    }
}

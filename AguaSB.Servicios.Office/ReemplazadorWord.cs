using DocumentFormat.OpenXml.Wordprocessing;
using System.Collections.Generic;

namespace AguaSB.Servicios.Office
{
    public static class ReemplazadorWord
    {
        public static void Reemplazar(Document documento, IReadOnlyDictionary<string, object> claves)
        {
            foreach (var text in documento.Descendants<Text>())
            {
                foreach (var k in claves)
                    text.Text = text.Text.Replace(k.Key, k.Value?.ToString());
            }
        }
    }
}

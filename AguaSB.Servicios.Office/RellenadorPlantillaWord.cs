using DocumentFormat.OpenXml.Packaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace AguaSB.Servicios.Office
{
    public class RellenadorPlantillaWord<T> : IPlantilla<FileInfo, T>
    {
        public FileInfo Plantilla { get; set; }
        public IExtractorValores<T> ExtractorValores { get; }
        public Func<T, string> GeneradorRutaArchivos { get; }
        public bool SobrescribirExistentes { get; set; }

        public RellenadorPlantillaWord(FileInfo plantilla, IExtractorValores<T> extractorValores, Func<T, string> generadorRutaArchivos, bool sobrescribirExistentes = true)
        {
            if (!plantilla.Exists)
                throw new FileNotFoundException("No se encontró el archivo de la plantilla.", plantilla.FullName);

            Plantilla = plantilla ?? throw new ArgumentNullException(nameof(plantilla));
            ExtractorValores = extractorValores ?? throw new ArgumentNullException(nameof(extractorValores));
            GeneradorRutaArchivos = generadorRutaArchivos ?? throw new ArgumentNullException(nameof(generadorRutaArchivos));
            SobrescribirExistentes = sobrescribirExistentes;
        }

        public FileInfo Rellenar(T datos)
        {
            var rutaSalida = GeneradorRutaArchivos(datos);

            Plantilla.CopyTo(rutaSalida, SobrescribirExistentes);

            using (var documento = WordprocessingDocument.Open(rutaSalida, true))
                SimplificarSustituir(documento, ExtractorValores.Extraer(datos));

            return new FileInfo(rutaSalida);
        }

        private void SimplificarSustituir(WordprocessingDocument documento, IReadOnlyDictionary<string, object> sustitucion)
        {
            SimplificadorWord.Simplificar(documento);

            var documentoPrincipal = documento.MainDocumentPart.Document;
            ReemplazadorWord.Reemplazar(documentoPrincipal, sustitucion);
            documentoPrincipal.Save();
        }
    }
}

using System;
using System.IO;

namespace AguaSB.Servicios.Archivos
{
    public class GeneradorRecibos<T> : IInformador<T>
    {
        public IPlantilla<FileInfo, T> Plantilla { get; }

        public IImpresor Impresor { get; }

        public GeneradorRecibos(IPlantilla<FileInfo, T> plantilla, IImpresor impresor)
        {
            Plantilla = plantilla ?? throw new ArgumentNullException(nameof(plantilla));
            Impresor = impresor ?? throw new ArgumentNullException(nameof(impresor));
        }

        public void Informar(T informacion) =>
            Impresor.Imprimir(Plantilla.Rellenar(informacion));
    }
}
